using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] private RawImage profileImage;
    [SerializeField] private TMP_Text displayNameTxt;
    
    public void Setup(string _name, Texture2D image)
    {
        displayNameTxt.text = _name;
        profileImage.texture = image;
    }

}
