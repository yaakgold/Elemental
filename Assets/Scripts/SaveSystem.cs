using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveCharacter(string name, string element, Transform data = null)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/Characters/{name}.Elem";

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        CharacterData charData = new CharacterData(name, element);

        formatter.Serialize(fs, charData);

        fs.Close();
    }

    public static CharacterData LoadCharacter(string name)
    {
        string path = Application.persistentDataPath + $"/Characters/{name}.Elem";
        if(!File.Exists(path))
        {
            Debug.LogError("Save file not found in " + path);

            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Open);

        CharacterData data = (CharacterData)formatter.Deserialize(fs);

        fs.Close();

        return data;
    }

    public static List<CharacterData> LoadInAllCharacters()
    {
        List<CharacterData> cd = new List<CharacterData>();



        return cd;
    }
}
