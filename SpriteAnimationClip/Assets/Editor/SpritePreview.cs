using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPreview (typeof(AnimationClip))]
public class SpritePreview : ObjectPreview
{
	private bool hasSprites;


	private GUIContent previewTitle = new GUIContent ("Sprites");

	public override bool HasPreviewGUI ()
	{
		return true;
	}

	public override GUIContent GetPreviewTitle ()
	{
		return previewTitle;
	}

	private Sprite[] GetSprites (AnimationClip animationClip)
	{
		var sprites = new Sprite[0];
        
		if (animationClip != null) {
			var editorCurveBinding = EditorCurveBinding.PPtrCurve ("", typeof(SpriteRenderer), "m_Sprite");

			var objectReferenceKeyframes = AnimationUtility.GetObjectReferenceCurve (animationClip, editorCurveBinding);

			sprites = objectReferenceKeyframes
               .Select (objectReferenceKeyframe => objectReferenceKeyframe.value)
               .OfType<Sprite> ()
               .ToArray ();
		}

		return sprites;
	}

	public override void Initialize (Object[] targets)
	{
		base.Initialize (targets);

		var sprites = new Object[0];

		foreach (AnimationClip animationClip in targets) {
			ArrayUtility.AddRange (ref sprites, GetSprites (animationClip));
		}

		foreach (var sprite in sprites) {
			AssetPreview.GetAssetPreview (sprite);
		}

		m_Targets = sprites;
	}

	public override void OnPreviewGUI (Rect r, GUIStyle background)
	{
		var previewTexture = AssetPreview.GetAssetPreview (target);
		EditorGUI.DrawTextureTransparent (r, previewTexture);
	}
}