using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsList : MonoBehaviour
{
    public GameObject contentHolder;
    public GameObject friendPref;

    public void AddFriend(string _name, Texture2D image)
    {
        Instantiate(friendPref, contentHolder.transform).GetComponent<PlayerInfoDisplay>().Setup(_name, image);
    }
}
