using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    [SerializeField]
    private float _mouseSensitivity = 100f;

    private float _xRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis(Axis.MouseX) * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(Axis.MouseY) * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
