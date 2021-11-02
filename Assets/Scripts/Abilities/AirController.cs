using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class AirController : MonoBehaviour
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
        if (isAbility2)
        {
            timer += Time.deltaTime;
            if (timer >= 5)
            {
                tpc.MoveSpeed /= moveSpeed;
                tpc.SprintSpeed /= moveSpeed;
                timer = 0;
                isAbility2 = false;
            }
        }
    }

    public void OnAbility1()
    {
        GameObject airBall = Instantiate(AirBall, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        airBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
    }

    public void OnAbility2()
    {
        tpc.MoveSpeed *= moveSpeed;
        tpc.SprintSpeed *= moveSpeed;
        isAbility2 = true;
    }

}
