using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private DefeatUI defeatUI;

    public void Die()
    {
        defeatUI.ShowDefeatScreen();
    }
}
