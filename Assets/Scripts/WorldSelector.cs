using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldSelector : MonoBehaviour
{
    public TMP_Text txtName;
    public TMP_Text txtComplete;

    public void Setup(string _name, float completionPercentage)
    {
        txtName.text = _name;
        txtComplete.text = completionPercentage + "%";
    }
}
