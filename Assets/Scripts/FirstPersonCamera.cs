using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float clampAngle = 80f;

    private float verticalRotation = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
