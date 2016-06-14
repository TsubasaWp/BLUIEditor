using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BLTexture;
using BLPage;
using BLEntity;
using UnityEngine.UI;

public class UIEditor : MonoBehaviour {

	// public
    public Transform Canvas;
    // private
    private Page page;
    private BLEntity.Entity entity;

	void Start () {
        //string filePath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\page\\SrpgEntrancePage.xml";
        string filePath = "D:\\Git\\BLUIEditor\\Assets\\Resource\\page_test.xml";
        if (!File.Exists(filePath))
	    {
	        return;
	    }

	    page = XmlLoader.LoadPage(filePath);
        Debug.Log(page.Components.Count);
        string entityPath = "D:\\Git\\BLUIEditor\\Assets\\Resource\\entity_test.xml";
//        string entityPath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\entity\\entities_ActivityPage.xml";
	    entity = XmlLoader.LoadEntity(entityPath);
        Debug.Log(entity.buttons.Count);

        RenderUI();
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
            var layer = new GameObject(component.id);
            Texture2D tex = LoadPNG("D:\\Git\\BLUIEditor\\Assets\\Resource\\demo.png");
            RectTransform rectTransform = layer.AddComponent<RectTransform>();
            rectTransform.SetParent(Canvas);
            rectTransform.localPosition = Vector3.zero;
            Image img = layer.AddComponent<Image>();
            img.sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.zero);

            // Parent
            if (component.parent != null && component.parent.id != String.Empty)
            {
                GameObject parent = GameObject.Find(component.parent.id);
                if (parent != null)
                {
                    layer.transform.SetParent(parent.transform);
                }
            }

            // size
            if (component.param != null &&
                component.param.width.Equals("fill") &&
                component.param.height.Equals("fill"))
            {
                rectTransform.sizeDelta = new Vector2(960, 640);
            }
            else if (component.param != null &&
                !component.param.width.Equals("content") &&
                !component.param.height.Equals("content"))
            {
                int w = Int16.Parse(component.param.width);
                int h = Int16.Parse(component.param.height);
                rectTransform.sizeDelta = new Vector2(w,h);
            }

            // position
            string marginStr = component.param.margin;
            if (marginStr != null)
            {
                string[] list = marginStr.Split(',');
                List<int> margin = new List<int>();
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
                rectTransform.localPosition = new Vector3(margin[0], margin[1], 1);
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
	
	// Update is called once per frame
	void Update () {
	    
	}
}
