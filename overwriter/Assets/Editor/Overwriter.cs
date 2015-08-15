using UnityEditor;
using UnityEngine;

public class Overwriter : AssetPostprocessor
{

    const string message = "\"{0}.{1}\"という名前のアセットが既にこの場所にあります。アセットを置き換えますか？";

    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        if (Event.current == null || Event.current.type != EventType.DragPerform)
            return;

        foreach (var assetPath in importedAssets) {

            var asset = new OverwriteAsset (assetPath);

            if (asset.exists) {

                var overwriteMessage = string.Format (message, asset.filename, asset.extension);

                var result = EditorUtility.DisplayDialogComplex (asset.originalAssetPath, overwriteMessage, "置き換える", "両方とも残す", "中止");

                if (result == 0) {
                    asset.Overwrite ();
                } else if (result == 2) {
                    asset.Delete ();
                }

            }
        }
    }
}
