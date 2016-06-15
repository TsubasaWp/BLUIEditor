using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.UI;

public class BLUIDirectory : EditorWindow
{

    [MenuItem("Assets/EmoticonEditor")]
    public static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 300, 300);
        BLUIDirectory window =
            (BLUIDirectory) EditorWindow.GetWindowWithRect(typeof (BLUIDirectory), wr, true, "EmoticonEditorWindow");
        window.Show();

    }

    private static readonly string DefaultWorkSpaces = "ProjectSettings/";
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
        GUILayout.BeginHorizontal();
        editorPosition = GUILayout.BeginScrollView(editorPosition, GUILayout.Width(400));
        DrawEditor();

        string[] files = Directory.GetFiles(workSpaces, "*");
        foreach (string file in files)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(file, GUILayout.Width(200)))
            {
                Debug.Log(file);
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
        GUILayout.Space(30);
      
    }

    private void DrawBaseInfo()
    {

    }

    private void ChangeName(string newName)
    {
        
    }
    
}