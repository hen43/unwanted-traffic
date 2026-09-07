using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject ResetMenu;
    [SerializeField] private GameObject ShopMenu;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameManager.GameState newState)
    {
        if (PauseMenu != null) PauseMenu.SetActive(false);
        if (ResetMenu != null) ResetMenu.SetActive(false);
        if (ShopMenu != null) ShopMenu.SetActive(false);

        switch (newState)
        {
            case GameManager.GameState.Pause:
                if (PauseMenu != null) PauseMenu.SetActive(true);
                break;

            case GameManager.GameState.Dead:
                if (ResetMenu != null) ResetMenu.SetActive(true);
                break;

            case GameManager.GameState.Shop:
                if (ShopMenu != null) ShopMenu.SetActive(true);
                break;

            case GameManager.GameState.Play:
                break;
        }
    }
}