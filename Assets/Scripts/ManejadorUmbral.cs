using UnityEngine;

public class ManejadorNPC : MonoBehaviour
{
    public Animator animador;
    public GameObject globoDialogo; // Tu Canvas de diálogo

    void Start()
    {
        // El diálogo empieza oculto
        if (globoDialogo != null) globoDialogo.SetActive(false);
    }

    public void DarBienvenida()
    {
        // 1. Inicia la animación (asegúrate de tener el Trigger 'Saludar' en el Animator)
        if (animador != null) animador.SetTrigger("Saludar");

        // 2. Muestra el diálogo
        if (globoDialogo != null) globoDialogo.SetActive(true);

        Debug.Log("????? NPC: ¡Bienvenido al simulador!");
    }
}