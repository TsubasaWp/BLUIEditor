using UnityEngine;
using UnityEditor;
using uizh.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class EmoticonEditorWindow : EditorWindow
{

    [MenuItem("Assets/EmoticonEditor")]
    public static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 1200, 400);
        EmoticonEditorWindow window = (EmoticonEditorWindow)EditorWindow.GetWindowWithRect(typeof(EmoticonEditorWindow), wr, true, "EmoticonEditorWindow");
        window.Show();

    }

    private static readonly string DefaultWorkSpaces = "Assets/EmoticonText/Prefabs/";
    private static readonly string DefaultEmoticonDBFileName = "EmoticonDB.prefab";

    private string workSpaces = DefaultWorkSpaces;
    private EmoticonDB db;
    private List<EmoticonImage> emoticons;
    private string emoticonDBFileName = DefaultEmoticonDBFileName;
    private GameObject emoticonDBPrefab;
    private string emoticonDBName;
    private string creatNewEmoticonDBName;
    private Vector2 EmoticonListPosition = Vector2.zero;
    private Vector2 infoPosition = Vector2.zero;
    private Vector2 listPosition = new Vector2(400, 0);
    private Vector2 editorPosition = new Vector2(800, 0);
    private Vector2 editorPlayerPosition = Vector2.zero;
    private GUILayoutOption infoWidth = GUILayout.Width(360);
    private GUILayoutOption listWidth = GUILayout.Width(360);
    private GUILayoutOption editorWidth = GUILayout.Width(360);
    private Sprite creatNewEmoticonSprite;
    private string creatNewEmoticonName;

    private static EmoticonImage curEmoticon;
    private Sprite addSprite;
    private string addNewName;

    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        curEmoticon = null;
    }

    void Update()
    {
        if (curEmoticon != null && curEmoticon.sprites.Count > 0 && curEmoticon.autoPlay)
        {
            int cf = (int)(EditorApplication.timeSinceStartup * curEmoticon.framesPerSecond) % curEmoticon.sprites.Count;
            if (curEmoticon.CurFrame != cf)
            {
                curEmoticon.CurFrame = cf;
                this.Repaint();
            }
        }
    }

    private void Init()
    {
        if (EditorPrefs.HasKey("emoticonDBName"))
        {
            EmoticonDBFileName = EditorPrefs.GetString("emoticonDBName");
        }
        if (EditorPrefs.HasKey("workSpaces"))
        {
            WorkSpaces = EditorPrefs.GetString("workSpaces");
        }
        CheckManager();
        InitPrefabs();
        InitEditor();
    }

    void InitEditor()
    {
        if (curEmoticon != null)
        {
            curEmoticon.LoadAllSprites();
        }
    }

    private void CheckManager()
    {
        GameObject edbp = (GameObject)AssetDatabase.LoadAssetAtPath(WorkSpaces + EmoticonDBFileName, typeof(GameObject));
        if (edbp == null || PrefabUtility.GetPrefabType(edbp) != PrefabType.Prefab)
        {
            Reset();
            //EditorUtility.DisplayDialog("settings are wrong", "this EmoticonDBName is not correct or workSpaces folder is not exists", "OK");
        }
        else
        {
            emoticonDBPrefab = edbp;
            emoticonDBName = emoticonDBPrefab.name;
        }
    }

    private void InitPrefabs()
    {
        if (emoticonDBPrefab==null)
        {
            string prefabFile = WorkSpaces + EmoticonDBFileName;
            emoticonDBPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject));
        }
        if (emoticonDBPrefab)
        {
            db = emoticonDBPrefab.GetComponent<EmoticonDB>();
            if (db != null)
            {
                emoticons = db.emotions;
                foreach (var item in emoticons)
                {
                    item.LoadAllSprites();
                }
            }
            else
            {
                emoticons = null;
            }
        }
        else
        {
            emoticons = null;
        }
        if (emoticons == null)
        {
            emoticonDBPrefab = null;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        infoPosition = GUILayout.BeginScrollView(infoPosition, GUILayout.Width(400));
        DrawInfo();
        GUILayout.EndScrollView();
        listPosition = GUILayout.BeginScrollView(listPosition, GUILayout.Width(400));
        DrawEmoticonList();
        GUILayout.EndScrollView();
        editorPosition = GUILayout.BeginScrollView(editorPosition, GUILayout.Width(400));
        DrawEditor();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }

    private void DrawInfo()
    {
        GUILayout.Space(10);
        DrawBaseInfo();
        GUILayout.Space(40);
        DrawManager();
        GUILayout.Space(40);
        DrawAddEmotion();
    }

    private void DrawEditor()
    {
        if (curEmoticon == null)
        {
            return;
        }

        if (GUILayout.Button("remove[" + curEmoticon.name + "] prefab from DB"))
        {
            RemoveEmoticon();
            return;
        }

        GUILayout.BeginHorizontal();
        addNewName = EditorGUILayout.TextField(addNewName);
        if (GUILayout.Button("changeName", GUILayout.Width(100)))
        {
            ChangeName(addNewName);
        }
        GUILayout.EndHorizontal();

        curEmoticon.framesPerSecond = EditorGUILayout.Slider("framesPerSecond", curEmoticon.framesPerSecond, 1, 30);
        curEmoticon.synchroPlay = EditorGUILayout.Toggle("synchroPlay", curEmoticon.synchroPlay);
        curEmoticon.autoPlay = EditorGUILayout.Toggle("autoPlay", curEmoticon.autoPlay);

        GUILayout.BeginHorizontal();
        addSprite = EditorGUILayout.ObjectField("AddSprite:", addSprite, typeof(Sprite), true) as Sprite;
        if (GUILayout.Button("AddSprite", GUILayout.Width(100)))
        {
            if (addSprite != null)
            {
                if (curEmoticon != null)
                {
                    curEmoticon.AddSprite(addSprite);
                }
                addSprite = null;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        if (curEmoticon != null && curEmoticon.sprites != null && curEmoticon.sprites.Count > 0)
        {
            if (GUILayout.Button("delete selecting sprite"))
            {
                curEmoticon.DeleteSelectSprite();
                return;
            }
            editorPlayerPosition = GUILayout.BeginScrollView(editorPlayerPosition);
            DrawSprite(curEmoticon.sprites[curEmoticon.CurFrame], 180, 0);
            GUILayout.Space(10);
            curEmoticon.CheckSelector();
            for (int i = 0, size = curEmoticon.sprites.Count; i < size; i++)
            {
                DrawSprite(curEmoticon.sprites[i], (i % 8) * 50, ((int)(i / 8) + 1) * 50);
                if (i % 8 == 0)
                {
                    GUILayout.BeginVertical();
                }
                curEmoticon.Selector[i] = GUI.Toggle(new Rect((i % 8) * 50, ((int)(i / 8) + 1) * 50, 50, 50), curEmoticon.Selector[i], "");
                if (i % 8 == 0)
                {
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
        }
    }

    private void DrawAddEmotion()
    {
        creatNewEmoticonSprite = EditorGUILayout.ObjectField("Sprite:", creatNewEmoticonSprite, typeof(Sprite), true, infoWidth) as Sprite;
        creatNewEmoticonName = EditorGUILayout.TextField("Emotion Symbol Name:", creatNewEmoticonName, infoWidth);
        if (GUILayout.Button("Add new(or exists) EmoticonImage prefab", infoWidth))
        {
            if (!Regex.IsMatch("{" + creatNewEmoticonName + "}", EmoticonDB.PatternEmoticon))
            {
                EditorUtility.DisplayDialog("Emotion Symbol Name is not correct", "Emotion Symbol Name is not match EmoticonDB.PatternEmoticon", "OK");
                return;
            }

            string prefabFile = WorkSpaces + "Emoticons/" + creatNewEmoticonName + ".prefab";
            GameObject ob = (GameObject)AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject));
            if (ob)
            {
                EmoticonImage i = ob.GetComponent<EmoticonImage>();
                if (i)
                {
                    foreach (var item in emoticons)
                    {
                        if (item == i)
                        {
                            return;
                        }
                    }
                    emoticons.Add(i);
                    return;
                }
                EditorUtility.DisplayDialog("prefab has exists", "the Emotion Symbol Name " + creatNewEmoticonName + " is reduplicate", "OK");
                return;
            }
            if (!string.IsNullOrEmpty(creatNewEmoticonName) && creatNewEmoticonSprite != null)
            {
                Object tempPrefab = PrefabUtility.CreateEmptyPrefab(prefabFile);
                GameObject pf = new GameObject();
                EmoticonImage emoticon = pf.AddComponent<EmoticonImage>();
                if (emoticon != null)
                {
                    emoticon.AddSprite(creatNewEmoticonSprite);
                    emoticon.ImageFileName = creatNewEmoticonSprite.texture.name;
                }
                tempPrefab = PrefabUtility.ReplacePrefab(emoticon.gameObject, tempPrefab);
                emoticon = (tempPrefab as GameObject).GetComponent<EmoticonImage>();
                if (emoticon != null)
                {
                    AddEmoticon(emoticon);
                }
                Object.DestroyImmediate(pf);
                creatNewEmoticonName = null;
                creatNewEmoticonSprite = null;
            }
            else
            {
                EditorUtility.DisplayDialog("please select Sprite or input Emotion Symbol Name", "the Emotion Symbol Name is empty or Sprite is null", "OK");
            }
        }
    }

    private void DrawEmoticonList()
    {
        EmoticonListPosition = EditorGUILayout.BeginScrollView(EmoticonListPosition);
        if (emoticons != null)
        {
            foreach (var item in emoticons)
            {
                if (DrawEmotion(item))
                {
                    break;
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawBaseInfo()
    {
        WorkSpaces = EditorGUILayout.TextField("WorkSpaces:", WorkSpaces, infoWidth);
        EmoticonDBFileName = EditorGUILayout.TextField("EmoticonDBName:", EmoticonDBFileName, infoWidth);
        GUILayout.BeginHorizontal(infoWidth);
        if (GUILayout.Button("reset all settings"))
        {
            Reset();
            saveSettings();
        }
        if (GUILayout.Button("save settings"))
        {
            saveSettings();
        }
        GUILayout.EndHorizontal();
    }

    private void ChangeName(string newName)
    {
        if (!string.IsNullOrEmpty(newName) && newName != curEmoticon.name)
        {
            string prefabFile = WorkSpaces + "Emoticons/" + newName + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)))
            {
                EditorUtility.DisplayDialog("prefab has exists", "this Emotion Symbol Name " + newName + " is reduplicate", "OK");
                addNewName = curEmoticon.name;
                return;
            }
            Object tempPrefab = PrefabUtility.CreateEmptyPrefab(prefabFile);
            tempPrefab = PrefabUtility.ReplacePrefab(curEmoticon.gameObject, tempPrefab);
            RemoveEmoticon();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(curEmoticon));
            curEmoticon = (tempPrefab as GameObject).GetComponent<EmoticonImage>();
            if (curEmoticon != null)
            {
                AddEmoticon(curEmoticon);
            }
            this.Close();
            return;
        }
    }

    private void Reset()
    {
        WorkSpaces = DefaultWorkSpaces;
        EmoticonDBFileName = DefaultEmoticonDBFileName;
    }

    /// <summary>
    /// return list changeState
    /// </summary>
    /// <param name="ei"></param>
    /// <returns></returns>
    private bool DrawEmotion(EmoticonImage ei)
    {
        if (ei == null)
        {
            emoticons.Remove(ei);
            return true;
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(ei.name))
        {
            curEmoticon = ei;
            addNewName = ei.name;
        }
        GUILayout.EndHorizontal();
        return false;
    }

    private void DrawManager()
    {
        GUILayout.Label("the prefab datebase of all emoticon ");
        if (db!=null)
        {
            db.emoticonNativeSize = EditorGUILayout.Toggle("EmoticonNativeSize",db.emoticonNativeSize);
        }
        GUILayout.BeginHorizontal(infoWidth);
        GameObject edb = EditorGUILayout.ObjectField("resource", emoticonDBPrefab, typeof(GameObject), true) as GameObject;
        if (edb == null || PrefabUtility.GetPrefabType(edb) != PrefabType.Prefab)
        {
            edb = null;
        }
        GUILayout.EndHorizontal();
        creatNewEmoticonDBName = EditorGUILayout.TextField("EmoticonDBName:", creatNewEmoticonDBName, infoWidth);
        if (GUILayout.Button("create new prefab datebase of all emoticon", infoWidth))
        {
            edb = CreateDB(creatNewEmoticonDBName);
            if (edb == null)
            {
                return;
            }
        }
        if (edb != emoticonDBPrefab)
        {
            emoticonDBPrefab = edb;
            InitPrefabs();
        }
    }

    private GameObject CreateDB(string dbName)
    {
        string prefabFile = WorkSpaces + dbName + ".prefab";
        GameObject ob = (GameObject)AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject));
        if (ob)
        {
            EditorUtility.DisplayDialog("prefab datebase has exists", "the prefab datebase " + creatNewEmoticonName + " is reduplicate", "OK");
            return null;
        }
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(WorkSpaces + dbName + ".prefab");
        GameObject go = new GameObject();
        EmoticonDB db = go.AddComponent<EmoticonDB>();
        db.emotions = emoticons;
        tempPrefab = PrefabUtility.ReplacePrefab(db.gameObject, tempPrefab);
        GameObject edb = (tempPrefab as GameObject);
        Object.DestroyImmediate(go);
        EmoticonDBFileName = dbName + ".prefab";
        saveSettings();
        return edb;
    }

    private void DrawSprite(Sprite sprite, float x, float y)
    {
        if (sprite == null)
        {
            return;
        }
        Texture t = sprite.texture;
        Rect tr = sprite.textureRect;
        Rect r = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height);
        GUI.DrawTextureWithTexCoords(new Rect(x, y, 40, 40), t, r);
    }

    public void AddEmoticon(EmoticonImage ei)
    {
        if (ei != null)
        {
            emoticons.Add(ei);
        }
    }

    public void RemoveEmoticon()
    {
        if (curEmoticon != null && emoticons.Contains(curEmoticon))
        {
            emoticons.Remove(curEmoticon);
            curEmoticon = null;
        }
    }

    private void saveSettings()
    {
        CheckManager();
        InitPrefabs();
        EditorPrefs.SetString("emoticonDBName", EmoticonDBFileName);
        EditorPrefs.SetString("workSpaces", WorkSpaces);
    }

    public string EmoticonDBFileName
    {
        get { return emoticonDBFileName; }
        set
        {
            emoticonDBFileName = value;
        }
    }

    public string WorkSpaces
    {
        get { return workSpaces; }
        set
        {
            workSpaces = value;
        }
    }

    void OnFocus()
    {

    }

    void OnLostFocus()
    {

    }

    void OnHierarchyChange()
    {

    }

    void OnProjectChange()
    {

    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    void OnDestroy()
    {

    }
}