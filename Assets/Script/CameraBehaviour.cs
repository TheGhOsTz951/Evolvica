using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float Sensitivity = 1;
    public float velocity = 7;
    public float boost = 3;

    float horizontalAxis;
    float verticalAxis;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void FixedUpdate()
    {
        horizontalAxis += Sensitivity * Input.GetAxis("Mouse X");
        verticalAxis += Sensitivity * Input.GetAxis("Mouse Y");

        verticalAxis = Mathf.Clamp(verticalAxis, -90f, 90f);

        transform.eulerAngles = new Vector3(-verticalAxis, horizontalAxis, 0f);

        Vector3 input = GetBaseInput();
        if (!input.Equals(Vector3.zero))
        {
            transform.position += input * (velocity/10f);
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += transform.right;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            p_Velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            p_Velocity -= transform.up;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            p_Velocity *= boost;
        }

        return p_Velocity;
    }
}
