using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject FireBall;

    [SerializeField]
    GameObject Meteor;

    #region Ability1
    public void OnAbility1()
    {
        if (!hasAuthority) return;

        CmdAbility1();
    }

    [Command]
    private void CmdAbility1()
    {
        GameObject fireBall = Instantiate(FireBall, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        fireBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));

        NetworkServer.Spawn(fireBall);
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
        GameObject meteor = Instantiate(Meteor, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        meteor.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));

        NetworkServer.Spawn(meteor);
    }
    #endregion
}
