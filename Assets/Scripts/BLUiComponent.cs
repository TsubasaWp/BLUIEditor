using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BLPage;
using UnityEditor;
using Component = BLPage.Component;

[Serializable]
public class BLUiComponent : MonoBehaviour
{
    public Component Component;
    public RectTransform Node;
    public RectTransform Parent;
    public RectTransform Anchor;

    private Vector2 currentPosition;

	// Update is called once per frame
    void Start()
    {
        currentPosition = Node.anchoredPosition;
    }
    public BLUiComponent Copy()
    {
        return GenericCopier<BLUiComponent>.DeepCopy(this);
    }

    public void OnRefresh()
    {
        if (Component == null) return;

        Component.id = Node.gameObject.name;
        Component.param.width = Node.rect.width.ToString();
        Component.param.height = Node.rect.height.ToString();

        if (! currentPosition.Equals(Node)) 
        { 
            Component.param.margin = getMarginString();
        }
    }
    

    string getMarginString()
    {
        int left = 0, top = 0, right = 0, bottom = 0;
        int x = (int) Node.anchoredPosition.x;
        int y = (int) Node.anchoredPosition.y;
        string alignStr = Component.param.rules.rule.align;
        if (alignStr == null)
        {
            return "0,0,0,0";
        }

        // x
        if (alignStr.Contains("toright"))
        {
            if (Anchor == Parent)
            {
                left = x - (int)(Anchor.rect.xMax
                           - Node.rect.xMin);
            }
            else
            {
                left = x - (int)(Anchor.anchoredPosition.x
                           + Anchor.rect.xMax
                           - Node.rect.xMin);
            }
        }
        else if (alignStr.Contains("toleft"))
        {
            if (Anchor == Parent)
            {
                right = (int)(Anchor.rect.xMin
                           - Node.rect.xMax) - x;
            }
            else
            {
                right = (int)(Anchor.anchoredPosition.x
                           + Anchor.rect.xMin
                           - Node.rect.xMax) - x;
            }
        }
        else if (alignStr.Contains("left"))
        {
            if (Anchor == Parent)
            {
                left = x -(int)(Anchor.rect.xMin
                       - Node.rect.xMin);
            }
            else
            {
                left = x - (int)(Anchor.anchoredPosition.x
                       + Anchor.rect.xMin
                       - Node.rect.xMin);
            }
        }
        else if (alignStr.Contains("right"))
        {
            if (Anchor == Parent)
            {
                right = (int)(Anchor.rect.xMax
                           - Node.rect.xMax) - x;
            }
            else
            {
                right = (int)(Anchor.anchoredPosition.x
                           + Anchor.rect.xMax
                           - Node.rect.xMax) - x;
            }
        }
        else if (alignStr.Contains("hcenter"))
        {
            if (Anchor == Parent)
            {
                left = x;
            }
            else
            {
                left = x - (int)(Anchor.anchoredPosition.x);
            }
        }
        // y
        if (alignStr.Contains("top"))
        {
            if (Anchor == Parent)
            {
                top = (int) (Anchor.rect.yMax
                             - Node.rect.yMax) - y;
            }
            else
            {
                top = (int) (Anchor.anchoredPosition.y
                             + Anchor.rect.yMax
                             - Node.rect.yMax) - y;
            }
        }
        else if (alignStr.Contains("bottom"))
        {
            if (Anchor == Parent)
            {
                bottom = y - (int) (Anchor.rect.yMin
                                    - Node.rect.yMin);
            }
            else
            {
                bottom = y - (int) (Anchor.anchoredPosition.y
                                + Anchor.rect.yMin
                                - Node.rect.yMin);
            }
        }
        else if (alignStr.Contains("above"))
        {
            if (Anchor == Parent)
            {
                bottom = y - (int)(Anchor.rect.yMax
                           - Node.rect.yMin);
            }
            else
            {
                bottom = y - (int)(Anchor.anchoredPosition.y
                           + Anchor.rect.yMax
                           - Node.rect.yMin);
            }
        }
        else if (alignStr.Contains("below"))
        {
            if (Anchor == Parent)
            {
                top = (int)(Anchor.rect.yMin
                           - Node.rect.yMax) - y;
            }
            else
            {
                top = (int)(Anchor.anchoredPosition.y
                           + Anchor.rect.yMin
                           - Node.rect.yMax) - y;
            }
        }
        else if (alignStr.Contains("vcenter"))
        {
            if (Anchor == Parent)
            {
                top = -y;
            }
            else
            {
                top = (int)(Anchor.anchoredPosition.y) - y;
            }
        }
        return String.Format("{0},{1},{2},{3}", left, top, right, bottom);
    }
}
