using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using BLPage; 
using BLEntity;
using UnityEditor;
using UnityEngine.UI;
using Component = BLPage.Component;
using Entity = BLEntity.Entity;
using Text = BLEntity.Text;

namespace BLUIEditor
{

    public class BLUIParser : Editor
    {
        // public
        public Transform Canvas;
        public String RootPath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\";
        public String OutputPath = "Output";

        // private
        private Page page;
        private Page modifidePage;
        private string currentFile;
        private List<Entity> entitiList = new List<Entity>();
        private List<GameObject> layerObjects = new List<GameObject>();
    
        public void ShowPage(string filePath)
        {
            GameObject stage = GameObject.Find("BLPageStage");
            Canvas = stage.transform;
            foreach (GameObject child in layerObjects)
            {
                GameObject.DestroyImmediate(child);
            }
            layerObjects.Clear();
            foreach (Transform child in Canvas)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }  

            if (!File.Exists(filePath))
            {
                currentFile = "";
                return;
            }

            currentFile = filePath;
            page = XmlLoader.LoadPage(filePath);
            modifidePage = null;
            RenderUI();
        }

        public void SavePage()
        {
            if ( modifidePage == null)
            { 
                return;
            }
            string savePath = OutputPath + "\\" + page.id + ".xml";
            XmlLoader.SavePage(savePath, modifidePage);
        }

        public void Refresh()
        {
            if (page == null)
            {
                return;
            }
            if (modifidePage == null)
            {
                modifidePage = new Page();
                modifidePage.Components = new List<Component>();
                modifidePage.id = page.id;
                modifidePage.alpha = page.alpha;
            }
            else
            {
                modifidePage.Components.Clear();
            }

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            for (int i = allObjects.Length -1; i >= 0; i--)
            {
                GameObject child = allObjects[i];
                BLUiComponent script = child.gameObject.GetComponent<BLUiComponent>();
                
                if (script && script.Component != null)
                {
                    script.OnRefresh();
                    modifidePage.Components.Add(script.Component);
                }
            }
        }

        public void CopyComponent(GameObject obj)
        {
            GameObject newObj = GameObject.Instantiate(obj);
            newObj.name = obj.name;
            RenameComponent(newObj);
            newObj.transform.SetParent(obj.transform.parent.transform);
            CopyScript(obj, newObj);
        }
        
        // 递归的重命名以及深拷贝BLUiComponent
        public void CopyScript(GameObject origin, GameObject target)
        {
            RenameComponent(target);
            // Deep Copy
            BLUiComponent script = target.GetComponent<BLUiComponent>();
            script.Component = origin.GetComponent<BLUiComponent>().Component.Copy();

            // 复制子节点
            foreach (Transform child1 in origin.GetComponentsInChildren<Transform>())
            {
                if (child1.gameObject.name == origin.name) continue;
                foreach (Transform child2 in target.GetComponentsInChildren<Transform>())
                {
                    if (child2.gameObject.name == target.name) continue;
                    if (child1.gameObject.name == child2.gameObject.name)
                    {
                        CopyScript(child1.gameObject, child2.gameObject);
                    }
                }
            }
        }

        // 重命名,防止重名
        void RenameComponent(GameObject obj)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            bool nameUsed = false;
            int index = 1;
            string newName;
            // 防止重名
            do
            {
                nameUsed = false;
                newName = obj.name + '_' + index;
                foreach (GameObject child in allObjects)
                {
                    if (newName == child.name)
                    {
                        nameUsed = true;
                        index++;
                        break;
                    }
                }
            } while (nameUsed);
            obj.name = newName;
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

            string entityPath = RootPath + "entity\\entities_" + package + ".xml";
            Entity entity = XmlLoader.LoadEntity(entityPath);
            entitiList.Add(entity);
            return entity;

            return null;
        }

        public Text FindTextFromPackage(string package, string id)
        {
            Entity entity = LoadEntityPackage(package);
            if (entity == null)
            {
                return null;
            }

            foreach (G_TextView item in entity.textViews)
            {
                if (item.id == id)
                {
                    if (item.member != null &&
                        item.member.text != null)
                    {
                        return item.member.text;
                    }
                }
            }
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
                if (item.id == id)
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

            foreach (G_Toggle item in entity.toggles)
            {
                if (item.id == id)
                {
                    if (item.button.textview != null &&
                        item.button.textview.view != null &&
                        item.button.textview.view.members != null &&
                        item.button.textview.view.members.texture != null)
                    {
                        return item.button.textview.view.members.texture.id;
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

                GameObject layer = new GameObject(component.id);
                BLUiComponent script = layer.AddComponent<BLUiComponent>();
                script.Component = component;

                RectTransform rectTransform = layer.AddComponent<RectTransform>();
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.SetParent(Canvas);
                rectTransform.localPosition = Vector3.zero;
                RectTransform parentTransform = Canvas.GetComponent<RectTransform>();

                // texture
                string pngPath = FindTextureFromPackage(component.entity.package, component.entity.id);
                if (pngPath == null)
                {
                    pngPath = "Assets\\Resource\\demo.png";
                }
                else
                {
                    pngPath = RootPath + "texture\\" + pngPath.Replace("\\", "\\\\") + ".png";
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
                    for (int i = layerObjects.Count - 1; i >= 0; i--)
                    {
                        GameObject obj = layerObjects[i];
                        if (obj.name == component.param.rules.rule.anchor)
                        {
                            anchorObject = obj;
                        }
                    }
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
                    img.sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)),
                        Vector2.zero);
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

                // set component script
                script.Node = rectTransform;
                script.Anchor = anchorTransform;
                script.Parent = parentTransform;
                layerObjects.Add(layer);
            }

            foreach (GameObject layer in layerObjects)
            {
                BLUiComponent script = layer.GetComponent<BLUiComponent>();
                if (script && script.Component.visible == "false")
                {
                    layer.SetActive(false);
                }
            }
        }

        BLPage.Component FindComponentByID(string id)
        {
            foreach (BLPage.Component component in page.Components)
            {
                if (component.id.Equals(id)) return component;
            }
            return null;
        }

        void SetNodePosition(RectTransform node, RectTransform anchor, RectTransform parent, string alignStr,
            List<int> margin)
        {
            int x = 0;
            int y = 0;
            // x
            if (alignStr.Contains("toright"))
            {
                if (anchor == parent)
                {
                    x = (int)(anchor.rect.xMax
                               - node.rect.xMin
                               + margin[0]);
                }
                else
                {
                    x = (int)(anchor.anchoredPosition.x
                               + anchor.rect.xMax
                               - node.rect.xMin
                               + margin[0]);
                }
            }
            else if (alignStr.Contains("toleft"))
            {
                if (anchor == parent)
                {
                    x = (int)(anchor.rect.xMin
                               - node.rect.xMax
                               - margin[2]);
                }
                else
                {
                    x = (int) (anchor.anchoredPosition.x
                               + anchor.rect.xMin
                               - node.rect.xMax
                               - margin[2]);
                }
            }
            else if (alignStr.Contains("left"))
            {
                if (anchor == parent)
                {
                    x = (int)(anchor.rect.xMin
                           - node.rect.xMin
                           + margin[0]);   
                }
                else
                {
                    x = (int) (anchor.anchoredPosition.x
                           + anchor.rect.xMin
                           - node.rect.xMin
                           + margin[0]);                   
                }
            }
            else if (alignStr.Contains("right"))
            {
                if (anchor == parent)
                {
                    x = (int)(anchor.rect.xMax
                               - node.rect.xMax
                               - margin[2]);
                }
                else
                {
                    x = (int) (anchor.anchoredPosition.x
                               + anchor.rect.xMax
                               - node.rect.xMax
                               - margin[2]);
                }
            }
            else if (alignStr.Contains("hcenter"))
            {
                if (anchor == parent)
                {
                    x = margin[0];
                }
                else
                {
                    x = (int) (anchor.anchoredPosition.x + margin[0]);
                }
            }
            // y
            if (alignStr.Contains("top"))
            {
                if (anchor == parent)
                {
                    y = (int)(anchor.rect.yMax
                               - node.rect.yMax
                               - margin[1]);
                }
                else
                {
                    y = (int) (anchor.anchoredPosition.y
                               + anchor.rect.yMax
                               - node.rect.yMax
                               - margin[1]);
                }
            }
            else if (alignStr.Contains("bottom"))
            {
                if (anchor == parent)
                {
                    y = (int)(anchor.rect.yMin
                               - node.rect.yMin
                               + margin[3]);
                }
                else
                {
                    y = (int) (anchor.anchoredPosition.y
                               + anchor.rect.yMin
                               - node.rect.yMin
                               + margin[3]);
                }
            }
            else if (alignStr.Contains("above"))
            {
                if (anchor == parent)
                {
                    y = (int)(anchor.rect.yMax
                               - node.rect.yMin
                               + margin[3]);
                }
                else
                {
                    y = (int) (anchor.anchoredPosition.y
                               + anchor.rect.yMax
                               - node.rect.yMin
                               + margin[3]);
                }
            }
            else if (alignStr.Contains("below"))
            {
                if (anchor == parent)
                {
                    y = (int)(anchor.rect.yMin
                               - node.rect.yMax
                               - margin[1]);
                }
                else
                {
                    y = (int) (anchor.anchoredPosition.y
                               + anchor.rect.yMin
                               - node.rect.yMax
                               - margin[1]);
                }
            }
            else if (alignStr.Contains("vcenter"))
            {
                if (anchor == parent)
                {
                    y = -margin[1];
                }
                else
                {
                    y = (int) (anchor.anchoredPosition.y - margin[1]);
                }
            }


            node.anchoredPosition = new Vector2(x, y);
        }


    }
}