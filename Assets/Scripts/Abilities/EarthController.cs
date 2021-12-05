using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EarthController : NetworkBehaviour
{
    [SerializeField]
    float power = 500;
    [SerializeField]
    float range = 1f;
    [SerializeField]
    float wallSpeed = 2f;
    [SerializeField]
    GameObject WallObj;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject Rock;

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

        AttackAnim(0);
    }

    public void ActivateAbility1()
    {
        CmdSpawnAbility1();
    }

    [Command]
    private void CmdSpawnAbility1()
    {
        GameObject rock = Instantiate(Rock, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

        rock.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), true, gameObject);

        NetworkServer.Spawn(rock);
    }
    #endregion

    #region Ability2
    public void OnAbility2()
    {
        if (!hasAuthority) return;

        Vector3 position = transform.forward + transform.position + new Vector3(0, 1, transform.localPosition.z + 5);
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        position = hit.point;
        startPosition = position;

        CmdWallStart();
    }

    private Vector3 position, startPosition;

    [Command]
    public void CmdWallStart()
    {
        if (GetComponent<PlayerController>().GetLevel() < GetComponent<PlayerController>().ability2.LvlNeeded) return;

        if (ability2Cooldown) return;

        ability2Cooldown = true;
        CallTimer(GetComponent<PlayerController>().ability2.coolDownTime, false);

        AttackAnim(1);
    }

    public void ActivateAbility2()
    {
        CmdSpawnAbility();
    }

    [Command]
    private void CmdSpawnAbility()
    {
        Vector3 lookPosition = this.gameObject.transform.position;
        lookPosition.y += 4;

        GameObject earthWall = Instantiate(WallObj, transform.position + (transform.up) + (transform.forward * 5), transform.rotation) as GameObject;

        earthWall.GetComponent<EarthWall>().StartWave(0.2f, startPosition, position, range, lookPosition);

        NetworkServer.Spawn(earthWall);
    }
    #endregion

    [ClientRpc]
    private void AttackAnim(int type)
    {
        GetComponent<Animator>().SetTrigger("Attack");
        GetComponent<Animator>().SetFloat("AttackType", type);
    }

    public void Wall(Vector3 position, Vector3 lookPosition)
    {
        Collider[] earthCollider = Physics.OverlapSphere(position, range);
        foreach (Collider collider in earthCollider)
        {
            if (collider.gameObject.tag == "earthTest")
            {
                collider.gameObject.GetComponent<Earth>().BendUp(lookPosition, power, position, wallSpeed);
            }
        }
    }

    [ClientRpc]
    private void CallTimer(float seconds, bool isAbility1)
    {
        StartCoroutine(AbilityTimer(seconds, isAbility1));
    }

    private IEnumerator AbilityTimer(float seconds, bool isAbility1)
    {
        float normalizedTime = 1;

        if(TryGetComponent(out PlayerController pc) && pc.enabled)
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
