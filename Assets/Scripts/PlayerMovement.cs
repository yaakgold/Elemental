using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Public Vars
    public float speed;
    #endregion

    #region Private Vars
    private Vector3 movement;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        movement = Vector3.zero;
    }

    private void Update()
    {
        transform.position += movement * speed * Time.deltaTime;
    }
    #endregion

    #region Input System
    public void OnMove(InputValue value)
    {
        movement.x = value.Get<Vector2>().x;
        movement.z = value.Get<Vector2>().y;
    }
    #endregion

}
