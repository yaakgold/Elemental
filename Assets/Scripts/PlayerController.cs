using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private LevelAnimation lvlAnimation;
    public Abilities ability1;
    public Abilities ability2;
    public GameObject ability1UI;
    public GameObject ability2UI;

    [SyncVar]
    public string steamName;

    [SerializeField]
    private Transform camFollow = null;
    [SerializeField]
    private GameObject playerUIPref;

    private bool setupPlayer = false;
    private bool setupHealth = false;

    private void Start()
    {
        name = steamName;

        if (GameManager.Instance == null) return;
        var go = Instantiate(playerUIPref, GameManager.Instance.playerHealthPanel.transform);
        if (hasAuthority)
            go.transform.parent.SetAsFirstSibling();
        else
            go.transform.parent.SetAsLastSibling();

        GetComponent<Health>().healthBar = go.GetComponentsInChildren<Image>()[1];
        go.GetComponentInChildren<TMP_Text>().text = name;
        if(hasAuthority)
            GetComponent<Health>().OnDeath.AddListener(Death);
        setupHealth = true;

        if(isServer && hasAuthority)
        {
            print(name);
            GameManager.Instance.SpawnEnemies();
            GameManager.Instance.exitAndSaveBtn.SetActive(true);
        }
    }

    private void Update()
    {
        if(!setupHealth && GameManager.Instance != null)
        {
            var go = Instantiate(playerUIPref, GameManager.Instance.playerHealthPanel.transform);
            if (hasAuthority)
                go.transform.parent.SetAsFirstSibling();
            else
                go.transform.parent.SetAsLastSibling();

            GetComponent<Health>().healthBar = go.GetComponentsInChildren<Image>()[1];
            go.GetComponentInChildren<TMP_Text>().text = name;
            if (hasAuthority)
                GetComponent<Health>().OnDeath.AddListener(Death);
            setupHealth = true;
        }

        if (setupPlayer) return;
        if (!hasAuthority)
        {
            enabled = false;
            //GameObject.FindGameObjectWithTag("CinemachineCam").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
            return;
        }
        GameObject.FindGameObjectWithTag("CinemachineCam").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = camFollow;
        
        //Setup Abilities
        ability1UI = GameObject.FindGameObjectsWithTag("AbilityUI")[0];
        ability2UI = GameObject.FindGameObjectsWithTag("AbilityUI")[1];

        ability1UI.GetComponentInChildren<TMP_Text>().text = ability1.name;
        ability1UI.GetComponentInChildren<Image>().sprite = ability1.sprite;

        ability2UI.GetComponentInChildren<TMP_Text>().text = ability2.name;
        ability2UI.GetComponentInChildren<Image>().sprite = ability2.sprite;

        ElemNetworkManager.playerObjs.Add(gameObject);

        setupPlayer = true;
    }

    public void SetLvlSystemAnim(LevelAnimation lvlAnimation)
    {
        this.lvlAnimation = lvlAnimation;

        lvlAnimation.OnLvlChange += LvlSystem_OnLvlChange;
    }

    private void LvlSystem_OnLvlChange(object sender, System.EventArgs e)
    {
        //Implement level up ding or particle effect.
        print("Level Up");
    }

    public void Death()
    {
        //Lower exp level
        //Do animation
        //After anim teleport player to spawnPosition or something like that
        CmdMovePlayer();

        //Reset health
        GetComponent<Health>().ResetHealth();
        print("I'M DEAD");
    }

    [Command]
    private void CmdMovePlayer()
    {
        MovePlayer();
    }

    [ClientRpc]
    private void MovePlayer()
    {
        print(ElemNetworkManager.spawnPoints.Length);
        GetComponent<CharacterController>().enabled = false;
        transform.position = ElemNetworkManager.spawnPoints[Random.Range(0, 4)].transform.position;
        GetComponent<CharacterController>().enabled = true;
    }
}