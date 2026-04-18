using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;

    [Header("Player")]
    [SerializeField] private FirstPersonCamera firstPersonCamera;
    [SerializeField] private PlayerController playerController;

    [Header("Prompt E")]
    [SerializeField] private GameObject ePrompt;

    [Header("Panel")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public bool IsPanelOpen { get; private set; }
    public bool HasInteractable { get; private set; }

    void Update()
    {
        if (IsPanelOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ClosePanel();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                HasInteractable = true;
                ePrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                    OpenPanel(interactable);
                return;
            }
        }

        HasInteractable = false;
        ePrompt.SetActive(false);
    }

    void OpenPanel(Interactable interactable)
    {
        titleText.text = interactable.title;
        descriptionText.text = interactable.description;
        infoPanel.SetActive(true);
        IsPanelOpen = true;
        firstPersonCamera.enabled = false;
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        infoPanel.SetActive(false);
        IsPanelOpen = false;
        firstPersonCamera.enabled = true;
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
