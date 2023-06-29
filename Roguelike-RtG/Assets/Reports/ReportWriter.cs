using UnityEditor;
using UnityEngine;
using System.IO;

public static class ReportWriter
{
    public static void FileReport(string fileName, string contents)
    {
        string path = "Assets/Reports/Files/" + fileName + ".txt";

        //if (!File.Exists(path)) File.WriteAllText(path, "New Report \n\n");
        File.WriteAllText(path, contents);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //AssetDatabase.CreateAsset(text, path);
    }
}
