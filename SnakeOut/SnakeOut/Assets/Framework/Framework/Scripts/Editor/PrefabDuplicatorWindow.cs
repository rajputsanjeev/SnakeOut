using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Place this file inside any folder named "Editor" in your Unity project.
/// Open via: Tools > Prefab Duplicator
/// </summary>
public class PrefabDuplicatorWindow : EditorWindow
{
    // ─── Shared ───────────────────────────────────────────────────────────────
    private enum Mode { EntireFolder, SinglePrefabs }
    private Mode _mode = Mode.EntireFolder;

    private string _destinationFolder = "Assets/DuplicatedPrefabs";

    // ─── Entire-Folder mode ───────────────────────────────────────────────────
    private DefaultAsset _sourceFolder;

    // ─── Single-Prefab mode ───────────────────────────────────────────────────
    private List<GameObject> _prefabsToDuplicate = new List<GameObject>();
    private bool _prefabListExpanded = true;
    private Vector2 _prefabListScroll;

    // ─── Drag-and-drop state ──────────────────────────────────────────────────
    private bool _isDraggingOver = false;

    // ─── Scroll for whole window ──────────────────────────────────────────────
    private Vector2 _mainScroll;

    // ─── Styles (lazy init) ───────────────────────────────────────────────────
    private GUIStyle _headerStyle;
    private GUIStyle _dropZoneStyle;
    private GUIStyle _dropZoneActiveStyle;
    private bool _stylesInitialised;

    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem("Tools/Prefab Duplicator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabDuplicatorWindow>("Prefab Duplicator");
        window.minSize = new Vector2(420, 500);
        window.Show();
    }

    // ── Lazy style init ───────────────────────────────────────────────────────
    private void InitStyles()
    {
        if (_stylesInitialised) return;

        _headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft
        };

        _dropZoneStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Italic,
            normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
        };

        _dropZoneActiveStyle = new GUIStyle(_dropZoneStyle)
        {
            normal =
            {
                textColor   = new Color(0.2f, 0.7f, 1f),
                background  = MakeTex(2, 2, new Color(0.2f, 0.7f, 1f, 0.15f))
            }
        };

        _stylesInitialised = true;
    }

    // ── GUI ───────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        InitStyles();

        _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);

        DrawHeader();
        GUILayout.Space(6);

        DrawModeSelector();
        GUILayout.Space(8);

        DrawDestinationFolder();
        GUILayout.Space(8);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (_mode == Mode.EntireFolder)
            DrawEntireFolderMode();
        else
            DrawSinglePrefabMode();

        GUILayout.Space(10);
        DrawDuplicateButton();
        GUILayout.Space(6);

        EditorGUILayout.EndScrollView();

        HandleGlobalDragAndDrop();
    }

    // ── Header ────────────────────────────────────────────────────────────────
    private void DrawHeader()
    {
        EditorGUILayout.Space(6);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(4);
            EditorGUILayout.LabelField("🗂  Prefab Duplicator", _headerStyle, GUILayout.Height(26));
        }
    }

    // ── Mode toggle ───────────────────────────────────────────────────────────
    private void DrawModeSelector()
    {
        EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Toggle(_mode == Mode.EntireFolder, "Entire Folder",
                    EditorStyles.miniButtonLeft, GUILayout.Height(24)))
                _mode = Mode.EntireFolder;

            if (GUILayout.Toggle(_mode == Mode.SinglePrefabs, "Pick Prefabs",
                    EditorStyles.miniButtonRight, GUILayout.Height(24)))
                _mode = Mode.SinglePrefabs;
        }
    }

    // ── Destination folder ────────────────────────────────────────────────────
    private void DrawDestinationFolder()
    {
        EditorGUILayout.LabelField("Destination Folder", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            _destinationFolder = EditorGUILayout.TextField(_destinationFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string picked = EditorUtility.OpenFolderPanel(
                    "Select Destination Folder", "Assets", "");
                if (!string.IsNullOrEmpty(picked))
                {
                    // Convert absolute path → relative Assets/… path
                    if (picked.StartsWith(Application.dataPath))
                        picked = "Assets" + picked.Substring(Application.dataPath.Length);
                    _destinationFolder = picked;
                    GUI.FocusControl(null);
                }
            }
        }

        EditorGUILayout.HelpBox(
            "Destination folder will be created automatically if it does not exist.",
            MessageType.Info);
    }

    // ── Entire-folder mode ────────────────────────────────────────────────────
    private void DrawEntireFolderMode()
    {
        EditorGUILayout.LabelField("Source Folder", EditorStyles.boldLabel);

        _sourceFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Folder", _sourceFolder, typeof(DefaultAsset), false);

        if (_sourceFolder != null)
        {
            string path = AssetDatabase.GetAssetPath(_sourceFolder);
            if (!AssetDatabase.IsValidFolder(path))
            {
                EditorGUILayout.HelpBox("Selected asset is not a folder.", MessageType.Warning);
                _sourceFolder = null;
            }
            else
            {
                int count = CountPrefabsInFolder(path);
                EditorGUILayout.HelpBox(
                    $"Found {count} prefab(s) in \"{path}\".", MessageType.None);
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Drag a folder from the Project window into the field above.",
                MessageType.None);
        }
    }

    // ── Single-prefab mode ────────────────────────────────────────────────────
    private void DrawSinglePrefabMode()
    {
        // Drop zone
        Rect dropRect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
        GUIStyle zoneStyle = _isDraggingOver ? _dropZoneActiveStyle : _dropZoneStyle;
        GUI.Box(dropRect, _isDraggingOver
            ? "Release to add prefabs!"
            : "⬇  Drag prefabs here  ⬇", zoneStyle);

        HandleDropZoneDragAndDrop(dropRect);

        GUILayout.Space(6);

        // Prefab list
        _prefabListExpanded = EditorGUILayout.Foldout(
            _prefabListExpanded,
            $"Prefabs to Duplicate  [{_prefabsToDuplicate.Count}]",
            true, EditorStyles.foldoutHeader);

        if (_prefabListExpanded)
        {
            _prefabListScroll = EditorGUILayout.BeginScrollView(
                _prefabListScroll, GUILayout.MaxHeight(200));

            int removeIndex = -1;
            for (int i = 0; i < _prefabsToDuplicate.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _prefabsToDuplicate[i] = (GameObject)EditorGUILayout.ObjectField(
                        _prefabsToDuplicate[i], typeof(GameObject), false);

                    if (GUILayout.Button("✕", GUILayout.Width(22), GUILayout.Height(18)))
                        removeIndex = i;
                }
            }

            if (removeIndex >= 0)
                _prefabsToDuplicate.RemoveAt(removeIndex);

            // Inline "Add Slot" button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+ Add Slot", GUILayout.Width(90)))
                    _prefabsToDuplicate.Add(null);
            }

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            if (EditorUtility.DisplayDialog("Clear list",
                    "Remove all prefabs from the list?", "Yes", "Cancel"))
                _prefabsToDuplicate.Clear();
        }
    }

    // ── Duplicate button ──────────────────────────────────────────────────────
    private void DrawDuplicateButton()
    {
        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.35f, 0.8f, 0.45f);
        if (GUILayout.Button("⧉  Duplicate Prefabs", GUILayout.Height(36)))
            ExecuteDuplicate();
        GUI.backgroundColor = prev;
    }

    // ── Core duplicate logic ──────────────────────────────────────────────────
    private void ExecuteDuplicate()
    {
        if (string.IsNullOrWhiteSpace(_destinationFolder))
        {
            EditorUtility.DisplayDialog("Error", "Please specify a destination folder.", "OK");
            return;
        }

        EnsureFolderExists(_destinationFolder);

        if (_mode == Mode.EntireFolder)
            DuplicateEntireFolder();
        else
            DuplicateSelectedPrefabs();
    }

    private void DuplicateEntireFolder()
    {
        if (_sourceFolder == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a source folder.", "OK");
            return;
        }

        string sourcePath = AssetDatabase.GetAssetPath(_sourceFolder);
        if (!AssetDatabase.IsValidFolder(sourcePath))
        {
            EditorUtility.DisplayDialog("Error", "Selected asset is not a folder.", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { sourcePath });
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Nothing to duplicate",
                "No prefabs found in the selected folder.", "OK");
            return;
        }

        int success = 0, skip = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (string guid in guids)
            {
                string src = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(src);
                string dest = AssetDatabase.GenerateUniqueAssetPath(
                    Path.Combine(_destinationFolder, fileName));

                if (AssetDatabase.CopyAsset(src, dest)) success++;
                else skip++;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Done",
            $"Duplicated {success} prefab(s) to \"{_destinationFolder}\"." +
            (skip > 0 ? $"\n{skip} file(s) could not be copied." : ""), "OK");
    }

    private void DuplicateSelectedPrefabs()
    {
        var valid = _prefabsToDuplicate.FindAll(p =>
            p != null && PrefabUtility.IsPartOfPrefabAsset(p));

        if (valid.Count == 0)
        {
            EditorUtility.DisplayDialog("Error",
                "No valid prefab assets in the list. " +
                "Make sure you drag prefabs from the Project window (not the Hierarchy).", "OK");
            return;
        }

        int success = 0, skip = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (GameObject prefab in valid)
            {
                string src = AssetDatabase.GetAssetPath(prefab);
                string fileName = Path.GetFileName(src);
                string dest = AssetDatabase.GenerateUniqueAssetPath(
                    Path.Combine(_destinationFolder, fileName));

                if (AssetDatabase.CopyAsset(src, dest)) success++;
                else skip++;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Done",
            $"Duplicated {success} prefab(s) to \"{_destinationFolder}\"." +
            (skip > 0 ? $"\n{skip} file(s) could not be copied." : ""), "OK");
    }

    // ── Drag & drop: drop zone (Single mode) ──────────────────────────────────
    private void HandleDropZoneDragAndDrop(Rect dropRect)
    {
        Event e = Event.current;
        if (!dropRect.Contains(e.mousePosition)) return;

        switch (e.type)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = HasPrefabs(DragAndDrop.objectReferences)
                    ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;
                _isDraggingOver = DragAndDrop.visualMode == DragAndDropVisualMode.Copy;
                e.Use();
                break;

            case EventType.DragPerform:
                DragAndDrop.AcceptDrag();
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                        if (!_prefabsToDuplicate.Contains(go))
                            _prefabsToDuplicate.Add(go);
                }
                _isDraggingOver = false;
                e.Use();
                break;

            case EventType.DragExited:
                _isDraggingOver = false;
                Repaint();
                break;
        }
    }

    // ── Drag & drop: global (catch drops anywhere on window) ─────────────────
    private void HandleGlobalDragAndDrop()
    {
        Event e = Event.current;
        if (_mode != Mode.SinglePrefabs) return;
        if (e.type != EventType.DragUpdated && e.type != EventType.DragPerform) return;
        // Only handle outside the drop zone (already handled above)
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static int CountPrefabsInFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        return guids.Length;
    }

    private static bool HasPrefabs(Object[] objects)
    {
        foreach (Object obj in objects)
            if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                return true;
        return false;
    }

    private static void EnsureFolderExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string[] parts = path.Split('/');
        string current = parts[0]; // "Assets"
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static Texture2D MakeTex(int w, int h, Color col)
    {
        Color[] pix = new Color[w * h];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        var tex = new Texture2D(w, h);
        tex.SetPixels(pix);
        tex.Apply();
        return tex;
    }
}
