using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealWell : MonoBehaviour
{
    public float timerCount;

    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            timerCount += Time.deltaTime;
            if (timerCount >= 1)
            {
                other.gameObject.TryGetComponent<Health>(out Health health);
                health.GetHit(-5);
                AudioManager.Instance.Play("Positive Effect 15", transform.position);
                timerCount = 0;
            }
        }
    }
}
