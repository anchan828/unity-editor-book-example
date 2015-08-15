using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

public class OverwriteAsset
{
    public string originalAssetPath {
        get {
            return Path.Combine (directoryName, filename + "." + extension);
        }
    }
    
    public bool exists { get; private set; }
    
    public string filename { get; private set; }
    
    public string extension { get; private set; }
    
    private string directoryName;
    private string assetPath;
    const string pattern = "^(?<name>.*)\\s\\d+\\.(?<extension>.*)$";
    
    public OverwriteAsset (string assetPath)
    {
        this.assetPath = assetPath;
        directoryName = Path.GetDirectoryName (assetPath);
        var match = Regex.Match (Path.GetFileName (assetPath), pattern);
        
        exists = match.Success;
        
        if (exists) {
            filename = match.Groups ["name"].Value;
            extension = match.Groups ["extension"].Value;
        }
    }
    
    public void Overwrite ()
    {
        FileUtil.ReplaceFile (assetPath, originalAssetPath);
        Delete ();
        AssetDatabase.ImportAsset (originalAssetPath);
    }
    
    public void Delete ()
    {
        AssetDatabase.DeleteAsset (assetPath);
    }
}