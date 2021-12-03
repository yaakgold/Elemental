using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : NetworkBehaviour
{
    public int damage;

    bool isMovingUp = false;
    bool isForward = false;
    float time;
    float speed;
    Vector3 endPosition;
    bool isPlayer = true;

    private void Update()
    {
        if(isForward)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if(isMovingUp)
        {
            if(Vector2.Distance(transform.position, endPosition) < 0.1f)
            {
                isForward = true;
                isMovingUp = false;
                time = 0;
            } 
            else
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(this.gameObject.transform.position, endPosition, Mathf.SmoothStep(0f, 1f, time));
            }
        }
    }

    public void AbilityInitial(float speed, Vector3 endPosition, bool player)
    {
        this.speed = speed;
        this.endPosition = endPosition;

        isMovingUp = true;

        Destroy(this.gameObject, 5f);

        isPlayer = player;
    }

    private void OnCollisionEnter(Collision other)
    {
        print(other.gameObject.name + " " + isPlayer);
        if(isPlayer)
        {
            if (other.gameObject.tag == "Enemy")
            {
                other.gameObject.TryGetComponent<Health>(out Health health);
                health.GetHit(damage);
            }
        }
        else
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.TryGetComponent<Health>(out Health health);
                health.GetHit(damage);
            }
        }

        Destroy(gameObject);
    }

}
