using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Public Props
    [SerializeField]
    public int CurrentHealth { get; private set; }
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
        CurrentHealth = maxHealth;
    }
    #endregion

    /// <summary>
    /// This functions will deal damage (or give health) to the object that this script is on. It takes into consideration any damage effectors, and maxHealth for the obj
    /// </summary>
    /// <param name="amt">Amount of damage to do (Negative will heal)</param>
    /// <returns>The amount of damage dealt (negative will be heal)</returns>
    public int GetHit(int amt)
    {
        int cHealth = CurrentHealth;

        int damageToDo = amt; //Add more here if there is other things that will effect it

        int damageDealt = cHealth - damageToDo;

        CurrentHealth = Mathf.Clamp(damageDealt, 0, maxHealth);

        //Check for death
        if(CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

        //TODO: Do I do an effect here to show the damage? Or would that get done somewhere else?

        return damageDealt;
    }
}
