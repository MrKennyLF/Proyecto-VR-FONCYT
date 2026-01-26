using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Si usas un panel en vez de una escena para Ajustes, puedes usar este:
    public GameObject settingsPanel;

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    public void OnSettingsButtonPressed()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // Activa un panel de ajustes en la misma escena
        }
        else
        {
            SceneManager.LoadScene("SettingsMenu"); // Alternativamente, carga una escena aparte
        }
    }

    public void OnExitButtonPressed()
    {
        Debug.Log("Saliendo de la aplicación...");
        Application.Quit();
    }
}
