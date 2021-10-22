using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
    Vector3 startScale;
    Vector3 endScale;

    bool isBending = false;
    bool isBendDown = false;
    bool StartedBending = false;

    float time = 0;
    float speed;

    void Awake()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (isBending)
        {
            if (Mathf.Abs((endScale - transform.localScale).y) < 0.01f)
            {
                isBending = false;
                time = 0;

                StartedBending = false;

                if (isBendDown)
                    BendBack();
            }
            else
            {
                time += Time.deltaTime * speed;
                gameObject.transform.localScale = Vector3.Lerp(transform.localScale, endScale, Mathf.SmoothStep(0f, 1f, time));
            }
        }
    }

    public void BendUp(Vector3 Player, float BendingSpeed)
    {
        float newScale = Vector3.Distance(Player, transform.position) / 3f;

        endScale = startScale;
        endScale.y += newScale;
        this.speed = BendingSpeed;

        isBendDown = true;
        StartCoroutine(StartBending(newScale / 5, Mathf.Floor(newScale), Player));
    }

    public void BendUp(Vector3 Player, float Power, Vector3 Pos, float BendingSpeed)
    {
        float newScale = Vector3.Distance(Pos, transform.position) * 2;

        endScale = startScale;
        endScale.y += Power - newScale;
        this.speed = BendingSpeed;

        isBendDown = false;
        if (!StartedBending)
        {
            StartedBending = true;
            StartCoroutine(StartBending(0, 5f, Player));
        }
    }

    IEnumerator StartBending(float Time, float Speed, Vector3 P)
    {
        yield return new WaitForSeconds(Time);

        speed = Speed;

        transform.LookAt(P);

        isBending = true;
    }

    public void BendBack()
    {
        endScale = startScale;
        isBending = true;
        isBendDown = false;
    }
}