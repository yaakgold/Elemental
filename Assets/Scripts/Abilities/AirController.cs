using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;

public class AirController : NetworkBehaviour
{
    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject AirBall;

    [SerializeField]
    float timer;

    public ThirdPersonController tpc;

    private bool isAbility2 = false;

    public float moveSpeed = 2;

    private void Update()
    {
        if (!hasAuthority) return;

        if (isAbility2)
        {
            timer += Time.deltaTime;
            if (timer >= 5)
            {
                CmdDeactivateA2();
            }
        }
    }

    #region Ability1
    public void OnAbility1()
    {
        if (!hasAuthority) return;

        CmdAbility1();
    }

    [Command]
    private void CmdAbility1()
    {
        GameObject airBall = Instantiate(AirBall, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

        airBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up * 1.5f) + (transform.forward * 4), true);

        NetworkServer.Spawn(airBall);
    }
    #endregion

    #region Ability2
    public void OnAbility2()
    {
        if (!hasAuthority) return;

        CmdCallAbility2();
    }

    [Command]
    private void CmdCallAbility2()
    {
        RpcCallAbility2();
    }

    [ClientRpc]
    private void RpcCallAbility2()
    {
        tpc.MoveSpeed *= moveSpeed;
        tpc.SprintSpeed *= moveSpeed;
        isAbility2 = true;
    }

    [Command]
    private void CmdDeactivateA2()
    {
        RpcDeactivateA2();
    }

    [ClientRpc]
    private void RpcDeactivateA2()
    {
        tpc.MoveSpeed /= moveSpeed;
        tpc.SprintSpeed /= moveSpeed;
        timer = 0;
        isAbility2 = false;
    }
    #endregion
}
