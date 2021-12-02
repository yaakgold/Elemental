using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    public GameObject worldsList;
    public GameObject worldCreator;
    public GameObject contentHolder;
    public GameObject worldSelectPref;
    public TMP_InputField worldName;

    public void ShowWorldCreator()
    {
        worldCreator.SetActive(true);
        worldsList.SetActive(false);
    }

    public void ShowWorldList()
    {
        worldsList.SetActive(true);
        worldCreator.SetActive(false);

        worldName.text = "";

        foreach (var world in SaveSystem.LoadInAllWorlds())
        {
            var btn = Instantiate(worldSelectPref, contentHolder.transform);
            btn.GetComponent<WorldSelector>().Setup(world.worldName, world.completionPercentage);
        }
    }   

    public void CreateWorld()
    {
        //TODO: Save off the world here
        SaveSystem.SaveWorld(worldName.text, 0, new SpawnObj[0]);


        ShowWorldList();
    }

    private void OnDisable()
    {
        for (int i = 0; i < contentHolder.transform.childCount; i++)
        {
            Destroy(contentHolder.transform.GetChild(i).gameObject);
        }
    }
}
