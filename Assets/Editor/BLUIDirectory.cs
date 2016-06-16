using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using BLUIEditor;

public class BLUIDirectory : EditorWindow
{

    [MenuItem("BLUIEditor/Open")]
    public static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0,400, 800);
        BLUIDirectory window =
            (BLUIDirectory) EditorWindow.GetWindowWithRect(typeof (BLUIDirectory), wr, true, "EmoticonEditorWindow");
        window.Show();

    }

    private static readonly string DefaultWorkSpaces = "F:\\Project\\BL\\clienthd\\data\\data_hd\\ui\\page";
    private string workSpaces = DefaultWorkSpaces;
    private Vector2 infoPosition = Vector2.zero;
    private Vector2 listPosition = new Vector2(400, 0);
    private Vector2 editorPosition = new Vector2(800, 0);
    private Vector2 editorPlayerPosition = Vector2.zero;
    private GUILayoutOption infoWidth = GUILayout.Width(360);
    private GUILayoutOption listWidth = GUILayout.Width(360);
    private GUILayoutOption editorWidth = GUILayout.Width(360);
    private Sprite creatNewEmoticonSprite;
    private string creatNewEmoticonName;
    private Sprite addSprite;
    private string addNewName;

    private UIEditor pageEditor = new UIEditor();

    // search
    private string fileSearchKey;


    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
    }

    private void Update()
    {

    }

    private void Init()
    {

        InitEditor();
    }

    private void InitEditor()
    {
    }

    void OnGUI()
    {
        DrawEditor();

        GUILayout.BeginHorizontal();
        editorPosition = GUILayout.BeginScrollView(editorPosition, GUILayout.Width(350), GUILayout.Height(700));
        

        string[] files = Directory.GetFiles(workSpaces, "*");
        foreach (string file in files)
        {
            GUILayout.BeginHorizontal();
            string[] filepath = file.Split('\\');
            if (fileSearchKey == null || 
                (fileSearchKey != null && filepath.Last().ToLower().Contains(fileSearchKey.ToLower())))
            {
                if (GUILayout.Button(filepath.Last(), GUILayout.Width(300)))
                {
                    Debug.Log(file);
                    pageEditor.ShowPage(file);
                }  
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }

    private void DrawInfo()
    {
        GUILayout.Space(10);
        DrawBaseInfo();
    }

    private void DrawEditor()
    {
        GUILayout.BeginHorizontal();
        addNewName = EditorGUILayout.TextField(addNewName);
        if (GUILayout.Button("changeName", GUILayout.Width(100)))
        {
            ChangeName(addNewName);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        addSprite = EditorGUILayout.ObjectField("AddSprite:", addSprite, typeof(Sprite), true) as Sprite;
        if (GUILayout.Button("AddSprite", GUILayout.Width(100)))
        {
            if (addSprite != null)
            {
                addSprite = null;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        fileSearchKey = EditorGUILayout.TextField(fileSearchKey);
       
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void DrawBaseInfo()
    {

    }

    private void ChangeName(string newName)
    {
        
    }
    
}