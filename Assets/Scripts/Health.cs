using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    #region Public Props
    [SerializeField, SyncVar]
    private int currentHealth;
    #endregion

    #region Public Vars
    public UnityEvent OnDeath = new UnityEvent();
    #endregion

    #region Private Vars
    [SerializeField]
    private int maxHealth = 100;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        currentHealth = maxHealth;
    }
    #endregion

    /// <summary>
    /// This functions will deal damage (or give health) to the object that this script is on. It takes into consideration any damage effectors, and maxHealth for the obj
    /// </summary>
    /// <param name="amt">Amount of damage to do (Negative will heal)</param>
    /// <returns>The amount of damage dealt (negative will be heal)</returns>
    public int GetHit(int amt)
    {
        CmdGetHit(amt);

        //TODO: Do I do an effect here to show the damage? Or would that get done somewhere else?

        return amt;
    }

    [Command(requiresAuthority = false)]
    private void CmdGetHit(int amt)
    {
        int cHealth = currentHealth;

        currentHealth = Mathf.Clamp(cHealth - amt, 0, maxHealth);

        //Check for death
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
