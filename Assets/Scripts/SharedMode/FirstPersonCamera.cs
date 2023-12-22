using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform target;
    public float mouseSensitivity = 10f;

    private float _verticalRotation;
    private float _horizontalRotation;

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        _verticalRotation -= mouseY * mouseSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -70f, 70f);

        _horizontalRotation += mouseX * mouseSensitivity;

        transform.rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
    }
}