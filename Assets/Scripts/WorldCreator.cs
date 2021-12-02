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
    public GameObject createNewWorldButton;

    private void Update()
    {
        createNewWorldButton.SetActive(contentHolder.transform.childCount < 3);
    }

    public void ShowWorldCreator()
    {
        worldCreator.SetActive(true);
        worldsList.SetActive(false);
    }

    public void ShowWorldList()
    {
        for (int i = 0; i < contentHolder.transform.childCount; i++)
        {
            Destroy(contentHolder.transform.GetChild(i).gameObject);
        }

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
        if (worldName.text == "") return;
        SaveSystem.SaveWorld(worldName.text, 0, new SpawnObj[0]);


        ShowWorldList();
    }

    private void OnDisable()
    {
        
    }
}
