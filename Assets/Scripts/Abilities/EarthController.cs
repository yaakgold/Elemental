using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EarthController : MonoBehaviour
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

    public void OnAbility1()
    {
        GameObject rock = Instantiate(Rock, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

        rock.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
    }

    public void OnAbility2()
    {
        Vector3 position = transform.forward + transform.position + new Vector3(0, 1, transform.localPosition.z + 5);
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        WallStart(hit.point, position);
    }

    public void WallStart(Vector3 position, Vector3 startPosition)
    {
        Vector3 lookPosition = this.gameObject.transform.position;
        lookPosition.y += 4;

        GameObject earthWall = Instantiate(WallObj, transform.position + (transform.up) + (transform.forward * 5), transform.rotation) as GameObject;

        earthWall.GetComponent<EarthWall>().StartWave(0.2f, startPosition, position, range, lookPosition);
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
}
