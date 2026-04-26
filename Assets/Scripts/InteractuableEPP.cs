using UnityEngine;

public class InteractuableEPP : MonoBehaviour
{
    [Tooltip("Arrastra aquí el ScriptableObject con la ficha técnica de este ítem")]
    public DatosEPP fichaTecnica;

    [Tooltip("El modelo que aparecerá en el cuerpo del jugador (si lo tienen)")]
    public GameObject mallaEnJugador; 

    // Esta función la llamarás desde el Pointable Unity Event Wrapper de Meta
    public void EquiparItem()
    {
        if (fichaTecnica != null)
        {
            GestorSeguridadMaster.Instancia.ProcesarSeleccionEPP(fichaTecnica);
        }

        // Activamos la malla en el jugador
        if (mallaEnJugador != null) mallaEnJugador.SetActive(true);

        // Desactivamos el objeto de la mesa para que no lo recoja dos veces
        gameObject.SetActive(false);
    }
}
