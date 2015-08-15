using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

//[CustomEditor(typeof(AnimationClip))]
public class AnimationClipEditor : OverrideEditor
{
    private bool hasSprite;
    Type baseType = Types.GetType("UnityEditor.AnimationClipEditor", "UnityEditor.dll");
    void OnEnable()
    {
        // スプライトを格納している配列のサイズが0 == スプライトアニメーションではない
        hasSprite = GetSpritesProperty().arraySize != 0;
    }

    protected override Editor GetBaseEditor()
    {
        Editor editor = null;
        CreateCachedEditor(targets, baseType, ref editor);
        return editor;
    }

    /// <summary>
    /// スプライトをもつAnimationClipの場合はfalseを返す。
    /// これはスプライトアニメーションでは必要のないアバターを表示するデフォルトプレビューをオフにするため
    /// </summary>
    /// <returns></returns>
    public override bool HasPreviewGUI()
    {
        return hasSprite == false;
    }

    /// <summary>
    /// スプライトの配列のプロパティを取得する
    /// </summary>
    /// <returns></returns>
    private SerializedProperty GetSpritesProperty()
    {
        var serializedProperty = serializedObject.FindProperty("m_ClipBindingConstant").FindPropertyRelative("pptrCurveMapping");
        return serializedProperty;
    }


}
