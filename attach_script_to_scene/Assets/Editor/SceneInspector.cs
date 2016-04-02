using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(SceneAsset))]
public class SceneInspector : Editor
{
    GameObject scenePrefab;

    Dictionary<Editor, bool> activeEditors = new Dictionary<Editor, bool>();

    void OnEnable()
    {
        var assetPath = AssetDatabase.GetAssetPath(target);

        scenePrefab = ScenePrefabUtility.GetScenePrefab(assetPath);

        if (scenePrefab == null)
            scenePrefab = ScenePrefabUtility.CreateScenePrefab(assetPath);

        InitActiveEditors();
        Undo.undoRedoPerformed += InitActiveEditors;
    }
    
    void OnDisable()
    {
        ClearActiveEditors();
        Undo.undoRedoPerformed -= InitActiveEditors;
    }

    void ClearActiveEditors()
    {
        foreach (var activeEditor in activeEditors)
        {
            DestroyImmediate(activeEditor.Key);
        }
        activeEditors.Clear();
    }

    void InitActiveEditors()
    {
        ClearActiveEditors();

        foreach (var component in scenePrefab.GetComponents<Component>())
        {
            if (component is Transform || component is RectTransform)
                continue;
            activeEditors.Add(Editor.CreateEditor(component), true);
        }

    }


    public override void OnInspectorGUI()
    {
        GUI.enabled = true;

        var editors = new List<Editor>(activeEditors.Keys);

        foreach (var editor in editors)
        {

            DrawInspectorTitlebar(editor);

            GUILayout.Space(-5f);

            if (activeEditors[editor] && editor.target != null)
                editor.OnInspectorGUI();

            DrawLine();
        }


        if (editors.All(e => e.target != null) == false)
        {
            InitActiveEditors();
            Repaint();
        }


        Rect dragAndDropRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.MinHeight(200));

        switch (Event.current.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                if (dragAndDropRect.Contains(Event.current.mousePosition) == false)
                    break;


                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    var components = DragAndDrop.objectReferences
                        .Where(x => x.GetType() == typeof(MonoScript))
                        .OfType<MonoScript>()
                        .Select(m => m.GetClass());

                    foreach (var component in components)
                    {
                        Undo.AddComponent(scenePrefab, component);
                    }
                    InitActiveEditors();
                }
                break;
        }


        GUI.Label(dragAndDropRect, "");
    }

    void DrawInspectorTitlebar(Editor editor)
    {
        var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(20));
        rect.x = 0;
        rect.y -= 5;
        rect.width += 20;
        activeEditors[editor] = EditorGUI.InspectorTitlebar(rect, activeEditors[editor], new Object[] { editor.target }, true);
    }

    void DrawLine()
    {
        EditorGUILayout.Space();
        var lineRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(2));
        lineRect.y -= 3;
        lineRect.width += 20;
        Handles.color = Color.black;
        Handles.DrawLine(new Vector2(0, lineRect.y), new Vector2(lineRect.width, lineRect.y));
    }
}