using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    public Animator loadingAnimator;        // Animator de la máquina inyectora
    public string vrSceneName = "VRESCENE"; // Nombre de la escena a cargar
    public float maxLoadTime = 8f;          // Tiempo máximo de carga

    private bool animationStarted = false;
    private bool animationComplete = false;
    private bool sceneReady = false;
    private AsyncOperation asyncLoad;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(ForceLoadAfterTimeout());
    }

    IEnumerator LoadSceneAsync()
    {
        asyncLoad = SceneManager.LoadSceneAsync(vrSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f && !sceneReady)
            {
                sceneReady = true;

                // Inicia la animación solo una vez
                if (!animationStarted && loadingAnimator != null)
                {
                    loadingAnimator.Play("Scene"); // Asegúrate que se llama exactamente así en el Animator
                    animationStarted = true;
                }

                // Espera a que la animación termine y añade un segundo extra
                StartCoroutine(WaitForAnimationAndProceed());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator WaitForAnimationAndProceed()
    {
        // Espera hasta que la animación complete
        while (!animationComplete)
        {
            yield return null;
        }

        // Espera 1 segundo extra tras terminar la animación
        yield return new WaitForSeconds(1f);

        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator ForceLoadAfterTimeout()
    {
        yield return new WaitForSeconds(maxLoadTime);
        animationComplete = true;
        sceneReady = true;
        TryProceedToScene();
    }

    // Llamado desde un evento en la animación justo al final
    public void OnAnimationComplete()
    {
        animationComplete = true;
    }

    void TryProceedToScene()
    {
        if (sceneReady && animationComplete)
        {
            if (asyncLoad != null)
                asyncLoad.allowSceneActivation = true;
            else
                SceneManager.LoadScene(vrSceneName);
        }
    }
}
