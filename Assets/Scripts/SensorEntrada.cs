using UnityEngine;
using UnityEngine.Events; // Para que sea modular

public class SensorEntrada : MonoBehaviour
{
    [Header("Configuración")]
    public string tagJugador = "Player"; // Asegúrate que tu CameraRig tenga este tag

    [Header("Eventos a Disparar")]
    public UnityEvent alEntrarJugador;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        // Solo se activa si es el jugador y no se ha activado antes
        if (other.CompareTag(tagJugador) && !activado)
        {
            activado = true; // Evita que se repita el saludo cada vez que pase
            Debug.Log("?? Jugador detectado en el umbral.");
            alEntrarJugador.Invoke();
        }
    }
}