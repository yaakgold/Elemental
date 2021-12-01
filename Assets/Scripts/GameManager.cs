using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            print(_instance);
            Destroy(gameObject);
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public GameObject playerHealthPanel;
}
