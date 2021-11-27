using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour
{
    public GameObject contentHolder;
    public GameObject friendPref;

    public void AddFriend(string _name, Texture2D image, CSteamID friendID)
    {
        GameObject btn = Instantiate(friendPref, contentHolder.transform);
        btn.GetComponent<PlayerInfoDisplay>().Setup(_name, image, friendID);
    }
}
