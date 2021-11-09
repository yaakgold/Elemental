using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : NetworkBehaviour
{
    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject Whip;

    [SerializeField]
    GameObject HealWell;

    #region Ability1
    public void OnAbility1()
    {
        if (!hasAuthority) return;

        CmdAbility1();
    }

    [Command]
    private void CmdAbility1()
    {
        GameObject whip = Instantiate(Whip, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        whip.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));

        NetworkServer.Spawn(whip);
    }
    #endregion

    #region Ability2
    public void OnAbility2()
    {
        if (!hasAuthority) return;

        CmdAbility2();
    }

    [Command]
    private void CmdAbility2()
    {
        GameObject healWell = Instantiate(HealWell, transform.position + (transform.up * 0.1f) + (transform.forward * 3), transform.rotation) as GameObject;

        NetworkServer.Spawn(healWell);
    }

    #endregion
}
