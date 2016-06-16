using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using BLUIEditor;

public class BLUIDirectoryWindow : EditorWindow
{

    [MenuItem("BLUIEditor/Open")]
    public static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0,360, 900);
        BLUIDirectoryWindow window =
            (BLUIDirectoryWindow) EditorWindow.GetWindowWithRect(typeof (BLUIDirectoryWindow), wr, true, "EmoticonEditorWindow");
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

    private BLUIParser pageParser = new BLUIParser();

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
                    pageParser.ShowPage(file);
                }  
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        // update
        pageParser.Refresh();
    }

    private void DrawInfo()
    {
        GUILayout.Space(10);
        DrawBaseInfo();
    }

    private void DrawEditor()
    {
        GUILayout.Space(20);
        //========================================================
        GUILayout.Label("一定要记得保存啊!");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存", GUILayout.Width(200)))
        {
            pageParser.SavePage();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        //========================================================
        GUILayout.Label("务必使用这个按钮复制,直接在unity中操作无效");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("复制选中图层", GUILayout.Width(200)))
        {
            pageParser.CopyComponent(Selection.activeGameObject);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        //========================================================
        GUILayout.Label("搜索Page:");
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