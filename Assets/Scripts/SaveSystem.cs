using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    #region World
    public static void SaveWorld(string name, float completionPercentage, SpawnObj[] spawnObjs, PlayerDictionary[] players)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/world_{name}.Elem";

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        WorldData worldData = new WorldData(name, completionPercentage, spawnObjs, players);

        formatter.Serialize(fs, worldData);

        fs.Close();
    }

    public static WorldData LoadWorld(string name)
    {
        string path = name;
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found in " + path);

            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Open);

        WorldData data = (WorldData)formatter.Deserialize(fs);

        fs.Close();

        return data;
    }

    public static List<WorldData> LoadInAllWorlds()
    {
        List<WorldData> wd = new List<WorldData>();

        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.Elem");

        foreach (string file in files)
        {
            wd.Add(LoadWorld(file));
        }

        return wd;
    }
    #endregion
}
