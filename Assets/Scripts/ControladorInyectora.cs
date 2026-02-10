using UnityEngine;
using System.Collections;

public class ControladorInyectora : MonoBehaviour
{
    [Header("--- ESTADO DE LA MÁQUINA ---")]
    public bool encendida = false;
    public bool procesoEnCurso = false;

    [Header("--- CONFIGURACIÓN (ARRASTRA AQUÍ) ---")]
    public Transform puntoDeSalida; // Un objeto vacío donde nacerá la pieza
    public GameObject moldePrefab;  // El objeto 3D de la pieza final (con Rigidbody)
    public float tiempoDeInyeccion = 4.0f; // Segundos que tarda en fabricar

    [Header("--- CONFIGURACIÓN VISUAL (OPCIONAL) ---")]
    public Light luzEstado; // La luz de la sirena
    public AudioSource audioMaquina; // Sonido de trabajo

    // --- FUNCIONES (BOTONES) ---

    [ContextMenu("TEST: Botón Power")] // Esto hace que aparezca en el menú de clic derecho
    public void BotonEncender()
    {
        encendida = !encendida; // Cambia de ON a OFF y viceversa
        Debug.Log("Inyectora Encendida: " + encendida);
        ActualizarLuces();
    }

    [ContextMenu("TEST: Botón Iniciar")]
    public void BotonIniciarCiclo()
    {
        // Solo arranca si está encendida Y no está ocupada ya
        if (encendida && !procesoEnCurso)
        {
            StartCoroutine(ProcesoInyeccion());
        }
        else
        {
            Debug.Log("⚠️ ERROR: La máquina está apagada u ocupada.");
        }
    }

    // --- LA LÓGICA INTERNA ---

    IEnumerator ProcesoInyeccion()
    {
        procesoEnCurso = true;
        Debug.Log("♻️ Iniciando ciclo de inyección...");

        // Estado visual: Trabajando (Luz Roja)
        if (luzEstado != null) luzEstado.color = Color.red;
        if (audioMaquina != null) audioMaquina.Play();

        // Esperamos el tiempo de fabricación
        yield return new WaitForSeconds(tiempoDeInyeccion);

        // Crear la pieza
        ExpulsarPieza();

        // Restaurar estado
        procesoEnCurso = false;
        ActualizarLuces();
        Debug.Log("✅ Ciclo terminado.");
    }

    void ExpulsarPieza()
    {
        if (moldePrefab != null && puntoDeSalida != null)
        {
            Instantiate(moldePrefab, puntoDeSalida.position, puntoDeSalida.rotation);
        }
        else
        {
            Debug.LogError("❌ FALTAN ASIGNAR OBJETOS EN EL INSPECTOR (Prefab o PuntoSalida)");
        }
    }

    void ActualizarLuces()
    {
        if (luzEstado != null)
        {
            if (encendida) luzEstado.color = Color.green; // Verde = Lista
            else luzEstado.color = Color.black; // Negro = Apagada
        }
    }
}