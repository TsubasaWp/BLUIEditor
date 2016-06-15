using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BLPage; 
using BLEntity;
using UnityEngine.UI;
using Entity = BLEntity.Entity;

public class UIEditor : MonoBehaviour {

	// public
    public Transform Canvas;
    // private
    private Page page;
    private BLEntity.Entity entity;
    private List<Entity> entitiList = new List<Entity>(); 
    private List<GameObject> layerObjects = new List<GameObject>(); 

	void Start () {
        string filePath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\page\\LittleWorldMapPage.xml";
        //string filePath = "D:\\Git\\BLUIEditor\\Assets\\Resource\\page_test.xml";
        if (!File.Exists(filePath))
	    {
	        return;
	    }

	    page = XmlLoader.LoadPage(filePath);
        Debug.Log(page.Components.Count);
        //string entityPath = "D:\\Git\\BLUIEditor\\Assets\\Resource\\entity_test.xml";

        RenderUI();
	}

    public Entity LoadEntityPackage(string package)
    {
        bool needLoad = true;
        foreach (Entity ent in entitiList)
        {
            if (ent.id == package)
            {
                return ent;
            }
        }
        
        string entityPath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\entity\\entities_" + package + ".xml";
        entity = XmlLoader.LoadEntity(entityPath);
        entitiList.Add(entity);
        return entity;

        return null;
    }

    public string FindTextureFromPackage(string package, string id)
    {
        Entity entity = LoadEntityPackage(package);
        if (entity == null)
        {
            return null;
        }

        foreach (G_View item in entity.views)
        {
            if (item.id == id && item.member.texture != null)
            {
                return item.member.texture.id;
            }
        }
        
        foreach (G_Button item in entity.buttons)
        {
            if (item.id == id)
            {
                if (item.textview != null &&
                    item.textview.view != null &&
                    item.textview.view.members != null &&
                    item.textview.view.members.texture != null)
                {
                    return item.textview.view.members.texture.id;
                }
            }
        }

        foreach (G_RelativeLayout item in entity.relativeLayouts)
        {
            if (item.id == id )
            {
                if (item.viewGroup != null &&
                    item.viewGroup.view != null &&
                    item.viewGroup.view.member != null &&
                    item.viewGroup.view.member.texture != null)
                {
                    return item.viewGroup.view.member.texture.id;
                }
            }
        }
        return null;
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    void RenderUI()
    {
        if (Canvas == null) return;

        foreach (BLPage.Component component in page.Components)
        {
            if (component.entity == null)
                continue;
            
            var layer = new GameObject(component.id);
            UIComponent script = layer.AddComponent<UIComponent>();
            script.component = component;

            RectTransform rectTransform = layer.AddComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f,0.5f);
            rectTransform.SetParent(Canvas);
            rectTransform.localPosition = Vector3.zero;
            RectTransform parentTransform = Canvas.GetComponent<RectTransform>();

            // texture
            string pngPath = FindTextureFromPackage(component.entity.package, component.entity.id);
            if (pngPath == null)
            {
                pngPath = "F:\\Project\\BL_UIEditor\\BL_UIEdigor\\Assets\\Resource\\demo.png";
            }
            else
            {
                pngPath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\texture\\" 
                    + pngPath.Replace("\\", "\\\\") + ".png";
            }
            
            // Parent
            if (component.parent != null && component.parent.id != String.Empty)
            {
                GameObject parent = GameObject.Find(component.parent.id);
                if (parent != null)
                {
                    layer.transform.SetParent(parent.transform);
                    parentTransform = parent.GetComponent<RectTransform>();
                }
            }

            // Anchor 
            GameObject anchorObject = null;
            if (component.param.rules != null &&
                component.param.rules.rule != null &&
                component.param.rules.rule.anchor != String.Empty)
            {
                anchorObject = GameObject.Find(component.param.rules.rule.anchor);
            }
            if (anchorObject == null)
            {
                anchorObject = parentTransform.gameObject;
            }
            RectTransform anchorTransform = anchorObject.GetComponent<RectTransform>();

            // size
            int w = 0;
            int h = 0;
            Texture2D tex = LoadPNG(pngPath);
            if (tex != null)
            {
                Image img = layer.AddComponent<Image>();
                img.sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.zero);
                w = tex.width;
                h = tex.height;
            }

            if (component.param != null)
            {
                if (component.param.width.Equals("fill"))
                {
                    w = (int) parentTransform.rect.width;
                }
                else if (!component.param.width.Equals("content"))
                {
                    w = Int16.Parse(component.param.width);
                }

                if (component.param.height.Equals("fill"))
                {
                    h = (int) parentTransform.rect.height;
                }
                else if (!component.param.height.Equals("content"))
                {
                    h = Int16.Parse(component.param.height);
                }
            }
           
            rectTransform.sizeDelta = new Vector2(w, h);

            // position
            string marginStr = component.param.margin;
            List<int> margin = new List<int>();
            if (marginStr == null)
            {
                marginStr = "0,0,0,0";
            }
            string[] list = marginStr.Split(',');
            for (int i = 0; i < 4; i++)
            {
                if (i < list.Length)
                {
                    margin.Add(Int32.Parse(list[i]));
                }
                else
                {
                    margin.Add(0);
                }
            }
            
            // align
            string alignStr = "hcenter vcenter";
            if (component.param != null &&
                component.param.rules != null &&
                component.param.rules.rule != null &&
                component.param.rules.rule.align != null &&
                component.param.rules.rule.align != String.Empty)
            {
                alignStr = component.param.rules.rule.align;
            }

            SetNodePosition(rectTransform, anchorTransform, parentTransform, alignStr, margin);

            layerObjects.Add(layer);
        }

        foreach (GameObject layer in layerObjects)
        {
            UIComponent script = layer.GetComponent<UIComponent>();
            if (script && script.component.visible == "false")
            {
                layer.SetActive(false);
            }
        }
    }

    BLPage.Component FindComponentByID(string id)
    {
        foreach (BLPage.Component component in page.Components)
        {
            if(component.id.Equals(id)) return component;
        }
        return null;
    }

    void SetNodePosition(RectTransform node, RectTransform anchor,RectTransform  parent, string alignStr, List<int> margin)
    {
        int x = 0;
        int y = 0;
        // x
        if (alignStr.Contains("toright"))
        {
            x = (int)(anchor.anchoredPosition.x 
                + anchor.rect.xMax
                - node.rect.xMin
                + margin[0]);
        }
        else if (alignStr.Contains("toleft"))
        {
            x = (int)(anchor.anchoredPosition.x
                + anchor.rect.xMin
                - node.rect.xMax
                - margin[2]);
        }
        else if (alignStr.Contains("left"))
        {
            x = (int)(anchor.anchoredPosition.x
                + anchor.rect.xMin
                - node.rect.xMin 
                + margin[0]);
        }
        else if (alignStr.Contains("right"))
        {
            x = (int)(anchor.anchoredPosition.x
                + anchor.rect.xMax
                - node.rect.xMax
                - margin[2]);
        }
        else if (alignStr.Contains("hcenter"))
        {
            if (anchor == parent)
            {
                x = margin[0];
            }
            else
            {
                x = (int)(anchor.anchoredPosition.x + margin[0]);
            }
        }
        // y
        if (alignStr.Contains("top"))
        {
            y = (int)(anchor.anchoredPosition.y 
                + anchor.rect.yMax
                - node.rect.yMax
                - margin[1]);
        }
        else if (alignStr.Contains("bottom"))
        {
            y = (int)(anchor.anchoredPosition.y
                + anchor.rect.yMin
                - node.rect.yMin
                + margin[3]);
        }
        else if (alignStr.Contains("above"))
        {
            y = (int)(anchor.anchoredPosition.y
                + anchor.rect.yMax
                + node.rect.yMin
                + margin[3]);
        }
        else if (alignStr.Contains("below"))
        {
            y = (int)(anchor.anchoredPosition.y
                + anchor.rect.yMin
                - node.rect.yMax
                - margin[1]);
        }
        else if (alignStr.Contains("vcenter"))
        {
            if (anchor == parent)
            {
                y = -margin[1];
            }
            else
            {
                y = (int)(anchor.anchoredPosition.y - margin[1]);
            }
        }

        node.anchoredPosition = new Vector2(x,y);
    }


	// Update is called once per frame
	void Update () {
	    
	}
}
