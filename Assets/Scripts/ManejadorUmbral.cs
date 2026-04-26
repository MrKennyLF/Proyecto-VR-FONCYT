using UnityEngine;

public class ManejadorNPC : MonoBehaviour
{
    public Animator animador;
    public GameObject globoDialogo; // Tu Canvas de di�logo

    void Start()
    {
        // El di�logo empieza oculto
        if (globoDialogo != null) globoDialogo.SetActive(false);
    }

    public void DarBienvenida()
    {
        // 1. Inicia la animaci�n (aseg�rate de tener el Trigger 'Saludar' en el Animator)
        if (animador != null) animador.SetTrigger("Saludar");

        // 2. Muestra el di�logo
        if (globoDialogo != null) globoDialogo.SetActive(true);

        Debug.Log("????? NPC: �Bienvenido al simulador!");
    }
}