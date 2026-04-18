using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel model;
    [SerializeField] private float mouseSensitivity = 2f;

    private void Awake()
    {
        model = GetComponent<PlayerModel>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Movimiento relativo a donde mira el jugador
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction.y = 0f;
        model.Walk(direction);

        // Rotacion horizontal del cuerpo con el mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        model.RotateY(mouseX);
    }
}