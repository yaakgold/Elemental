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

    private bool ability1Cooldown = false;
    private bool ability2Cooldown = false;

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
        if (GetComponent<PlayerController>().GetLevel() < GetComponent<PlayerController>().ability1.LvlNeeded) return;

        if (ability1Cooldown) return;

        ability1Cooldown = true;
        CallTimer(GetComponent<PlayerController>().ability1.coolDownTime, true);

        AttackAnim(0);

        PlayAudio("Wind Spell 9");
    }

    public void ActivateAbility1()
    {
        CmdSpawnAbility();
    }

    [Command]
    private void CmdSpawnAbility()
    {
        GameObject airBall = Instantiate(AirBall, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

        airBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up * 1.5f) + (transform.forward * 4), true, gameObject);

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
        if (GetComponent<PlayerController>().GetLevel() < GetComponent<PlayerController>().ability2.LvlNeeded) return;

        if (ability2Cooldown) return;

        ability2Cooldown = true;
        CallTimer(GetComponent<PlayerController>().ability2.coolDownTime, false);

        RpcCallAbility2();
        PlayAudio("Positive Effect 14");
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

    public void ActivateAbility2()
    {

    }
    #endregion

    [ClientRpc]
    private void PlayAudio(string audioName)
    {
        AudioManager.Instance.Play(audioName, transform.position);
    }

    [ClientRpc]
    private void AttackAnim(int type)
    {
        GetComponent<Animator>().SetTrigger("Attack");
        GetComponent<Animator>().SetFloat("AttackType", type);
    }

    [ClientRpc]
    private void CallTimer(float seconds, bool isAbility1)
    {
        StartCoroutine(AbilityTimer(seconds, isAbility1));
    }

    private IEnumerator AbilityTimer(float seconds, bool isAbility1)
    {
        float normalizedTime = 1;

        if (TryGetComponent(out PlayerController pc) && pc.enabled)
        {
            if (isAbility1)
                pc.ability1UITimer.enabled = true;
            else
                pc.ability2UITimer.enabled = true;
        }

        while (normalizedTime >= 0f)
        {
            if (TryGetComponent(out PlayerController pc2) && pc2.enabled)
            {
                if (isAbility1)
                    pc2.ability1UITimer.fillAmount = normalizedTime;
                else
                    pc2.ability2UITimer.fillAmount = normalizedTime;
            }

            normalizedTime -= Time.deltaTime / seconds;
            //print(normalizedTime);
            yield return null;
        }

        if (isAbility1)
        {
            ability1Cooldown = false;
            if (GetComponent<PlayerController>().enabled)
            {
                GetComponent<PlayerController>().ability1UITimer.fillAmount = 1;
                GetComponent<PlayerController>().ability1UITimer.enabled = false;
            }
        }
        else
        {
            ability2Cooldown = false;
            if (GetComponent<PlayerController>().enabled)
            {
                GetComponent<PlayerController>().ability2UITimer.fillAmount = 1;
                GetComponent<PlayerController>().ability2UITimer.enabled = false;
            }
        }
    }
}
