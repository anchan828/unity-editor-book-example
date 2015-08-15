using UnityEngine;
using System.Reflection;
using UnityEditor;


public abstract class OverrideEditor : Editor
{
    readonly static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
    readonly MethodInfo methodInfo = 
        typeof(Editor).GetMethod("OnHeaderGUI", flags);

    private Editor m_BaseEditor;
    protected Editor baseEditor
    {
        get { return m_BaseEditor ?? (m_BaseEditor = GetBaseEditor()); }
        set { m_BaseEditor = value; }
    }

    protected abstract Editor GetBaseEditor();


    public override void OnInspectorGUI()
    {
        baseEditor.OnInspectorGUI();
    }

    // ... 以下 GetInfoString、OnPreviewSettings というようにカスタムエディターで使用できるメソッド群が列挙する
    // ただし、DrawPreview、OnPreviewGUI、OnInteractivePreviewGUIをすべてオーバーライドしてしまうと挙動が変更されてしまうので注意すること
    public override string GetInfoString()
    {
        return baseEditor.GetInfoString();
    }

    public override void OnPreviewSettings()
    {
        baseEditor.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        baseEditor.ReloadPreviewInstances();
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        return baseEditor.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    protected override void OnHeaderGUI()
    {
        methodInfo.Invoke(baseEditor, new object[0]);
    }

    public override bool RequiresConstantRepaint()
    {
        return baseEditor.RequiresConstantRepaint();
    }

    public override bool UseDefaultMargins()
    {
        return baseEditor.UseDefaultMargins();
    }

    public override GUIContent GetPreviewTitle()
    {
        return baseEditor.GetPreviewTitle();
    }
}
