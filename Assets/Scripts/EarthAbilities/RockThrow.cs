using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrow : MonoBehaviour
{
    bool isMovingUp = false;
    bool isForward = false;
    float time;
    float speed;
    Vector3 endPosition;

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

    public void RockInitial(float speed, Vector3 endPosition)
    {
        this.speed = speed;
        this.endPosition = endPosition;

        isMovingUp = true;

        Destroy(this.gameObject, 2f);
    }

}