using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;

    [SerializeField]
    GameObject FireBall;

    [SerializeField]
    GameObject Meteor;

    public void OnAbility1()
    {
        GameObject fireball = Instantiate(FireBall, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        fireball.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
    }

    public void OnAbility2()
    {
        GameObject meteor = Instantiate(Meteor , transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        meteor.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
    }
}
