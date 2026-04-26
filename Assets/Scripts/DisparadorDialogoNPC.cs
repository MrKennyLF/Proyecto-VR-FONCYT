using UnityEngine;

public class DisparadorDialogoNPC : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que tiene el script ControladorTimelineNPC")]
    public ControladorTimelineNPC controladorNPC;

    [Header("Configuración")]
    public bool activadoSoloUnaVez = true;
    private bool yaSeHablo = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador (Asegúrate que tu Camera Rig tenga el Tag "Player")
        if (other.CompareTag("Player") && !yaSeHablo)
        {
            if (controladorNPC != null)
            {
                controladorNPC.IniciarDialogo();
                
                if (activadoSoloUnaVez)
                {
                    yaSeHablo = true;
                }
            }
        }
    }
}