using System;
using UnityEngine;
using System.Collections;
using System.IO;
using BLTexture;
using BLPage;
using BLEntity;

public class UIEditor : MonoBehaviour {

	// public
    public Transform Canvas;
    public String title;
    // private
    private Page page;
    private BLEntity.Entity entity;

	void Start () {
        string filePath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\page\\SrpgEntrancePage.xml";
        if (!File.Exists(filePath))
	    {
	        return;
	    }

	    page = XmlLoader.LoadPage(filePath);
        Debug.Log(p.Components.Count);

        string entityPath = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\entity\\entities_ActivityPage.xml";
	    entity = XmlLoader.LoadEntity(entityPath);
        Debug.Log(entity.buttons.Count);

        RenderUI();
	}

    void RenderUI()
    {
        if (Canvas == null) return;

        foreach (BLPage.Component component in page.Components)
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
