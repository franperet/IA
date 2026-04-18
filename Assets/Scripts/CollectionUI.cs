using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    [Header("Counter")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private int totalObjects = 10;

    [Header("Victory")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private FirstPersonCamera firstPersonCamera;
    [SerializeField] private PlayerController playerController;

    private int collected = 0;

    void Start()
    {
        victoryPanel.SetActive(false);
        UpdateText();
    }

    public void AddObject()
    {
        collected++;
        UpdateText();

        if (collected >= totalObjects)
            ShowVictory();
    }

    void UpdateText()
    {
        counterText.text = $"{collected}/{totalObjects}";
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
        firstPersonCamera.enabled = false;
        playerController.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
