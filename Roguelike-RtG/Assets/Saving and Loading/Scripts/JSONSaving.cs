using System.IO;
using UnityEngine;
using JS.WorldMap;

public class JSONSaving : MonoBehaviour
{
    private WorldSaveData playerData;

    private string path = "";
    private string persistentPath = "";

    private void Awake() => SetPaths();

    //Checks the Persistent Data path for all files within the location
    public bool CheckForExistingSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath);
        if (saves.Length == 0)
        {
            Debug.Log("No Existing Save Files");
            return false;
        }

        for (int i = 0; i < saves.Length; i++)
        {
            //This works so long as the file name contains SaveData
            if (!saves[i].Contains("SaveData")) continue;
            StreamReader reader = new StreamReader(saves[i]);
            string json = reader.ReadToEnd();

            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);
        }
        return true;
    }

    private void CreatePlayerData()
    {
        playerData = new WorldSaveData();
    }

    private void SetPaths()
    {
        //This one isn't even in use
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
    }

    public void SaveData()
    {
        string savePath = persistentPath;
        //Debug.Log("Saving Data at " + savePath);

        CreatePlayerData();
        string json = JsonUtility.ToJson(playerData, true);
        Debug.Log(json);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public void LoadData()
    {
        StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

        //Debug.Log("this isn't doing anything yet");
        SetPlayerValues(data);
    }

    private void SetPlayerValues(WorldSaveData data)
    {
        //Load back in all saved data to appropriate places
    }
}