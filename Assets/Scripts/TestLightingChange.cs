using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLightingChange : MonoBehaviour
{
    public Animator anim;
    public Texture skyboxStart;
    public Texture skyboxEnd;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            anim.Play("LightDown");
            RenderSettings.skybox.mainTexture = skyboxEnd;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.Play("LightUp");
            RenderSettings.skybox.mainTexture = skyboxStart;
        }
    }
}
