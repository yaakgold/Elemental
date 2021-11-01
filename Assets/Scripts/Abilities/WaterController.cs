using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject Whip;

    [SerializeField]
    GameObject HealWell;

    public void OnAbility1()
    {
        GameObject whip = Instantiate(Whip, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        whip.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
    }

    public void OnAbility2()
    {
        GameObject healWell = Instantiate(HealWell, transform.position + (transform.up * 0.1f) + (transform.forward * 3), transform.rotation) as GameObject;
    }
}
