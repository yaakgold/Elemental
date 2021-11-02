using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthWall : MonoBehaviour
{
    bool isMoving = false;
    float time = 0;

    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 lookPosition;

    float speed;
    float range;

    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    private void Update()
    {
  /*      if(isMoving)
        {
            if(Vector2.Distance(transform.position, endPosition) < range)
            {
                FindObjectOfType<EarthController>().Wall(endPosition, lookPosition);
            }

            if(Vector2.Distance(transform.position, endPosition) < range)
            {
                isMoving = false;
                time = 0;

                Destroy(this.gameObject);
            }
            else
            {
                time += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(this.gameObject.transform.position, endPosition, Mathf.SmoothStep(0f, 1f, time));
            }
        }*/
    }

    public void StartWave(float speed, Vector3 startPosition, Vector3 endPosition, float range, Vector3 lookPosition)
    {
        this.speed = speed;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.range = range;
        this.lookPosition = lookPosition;

        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag ==  "earthTest")
        {
            other.gameObject.GetComponent<Earth>().BendUp(startPosition, 3, this.transform.position, 0);
        }
    }
}
