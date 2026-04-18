using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatUI : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private FirstPersonCamera firstPersonCamera;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        defeatPanel.SetActive(false);
    }

    public void ShowDefeatScreen()
    {
        defeatPanel.SetActive(true);
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

    public void Quit()
    {
        Application.Quit();
    }
}
