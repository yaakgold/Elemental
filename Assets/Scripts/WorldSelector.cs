using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelector : MonoBehaviour
{
    public TMP_Text txtName;
    public TMP_Text txtComplete;

    public void Setup(string _name, float completionPercentage)
    {
        txtName.text = _name;
        txtComplete.text = completionPercentage + "%";
        GetComponent<Button>().onClick.AddListener(WorldClick);
    }

    public void WorldClick()
    {
        NetworkManager.singleton.GetComponent<SteamLobby>().worldData.worldName = txtName.text;
        NetworkManager.singleton.GetComponent<SteamLobby>().HostLobby();
    }

    public void DeleteWorld()
    {
        string path = Application.persistentDataPath + $"/world_{txtName.text}.Elem";

        File.Delete(path);

        Destroy(gameObject);
    }
}
