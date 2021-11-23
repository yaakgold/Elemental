using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] private RawImage profileImage;
    [SerializeField] private TMP_Text displayNameTxt;

    private CSteamID friendID;
    private SteamLobby sl;
    
    public void Setup(string _name, Texture2D image, CSteamID _friendID)
    {
        displayNameTxt.text = _name;
        profileImage.texture = image;
        friendID = _friendID;

        GetComponent<Button>().onClick.AddListener(SelectWorld);

        sl = ElemNetworkManager.singleton.GetComponent<SteamLobby>();
    }

    private void SelectWorld()
    {
        sl.AttemptConnection(friendID);
    }

}
