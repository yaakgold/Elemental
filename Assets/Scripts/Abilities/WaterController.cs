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

    private bool ability1Cooldown = false;
    private bool ability2Cooldown = false;

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

        GameObject whip = Instantiate(Whip, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

        whip.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), true, gameObject);

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
        if (GetComponent<PlayerController>().GetLevel() < GetComponent<PlayerController>().ability2.LvlNeeded) return;

        if (ability2Cooldown) return;

        ability2Cooldown = true;
        CallTimer(GetComponent<PlayerController>().ability2.coolDownTime, false);

        GameObject healWell = Instantiate(HealWell, transform.position + (transform.up * 0.1f) + (transform.forward * 3), transform.rotation) as GameObject;

        NetworkServer.Spawn(healWell);
    }
    #endregion

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
