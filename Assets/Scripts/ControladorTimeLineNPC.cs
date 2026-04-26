using UnityEngine;
using UnityEngine.Playables; // <-- NUEVO: Necesario para controlar el Timeline

public class ControladorTimelineNPC : MonoBehaviour
{
    [Header("Conexión")]
    [Tooltip("Arrastra aquí el objeto Cinematica_Dialogo_NPC")]
    public PlayableDirector directorCinematica;

    public void IniciarDialogo()
    {
        if (directorCinematica != null)
        {
            directorCinematica.Play();
            Debug.Log("🎬 Reproduciendo animación y audio perfectamente sincronizados.");
        }
    }
}