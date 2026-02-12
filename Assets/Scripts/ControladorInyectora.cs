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
    private float tiempoUltimoClick = 0f; // Para evitar el rebote
    private float esperaRebote = 0.5f;    // Medio segundo de espera

    // --- FUNCIONES (BOTONES) ---

    [ContextMenu("TEST: Botón Power")]
    public void BotonEncender()
    {
        // SI ha pasado menos de medio segundo desde el último click... ¡IGNORAR!
        if (Time.time - tiempoUltimoClick < esperaRebote)
        {
            return; // Nos salimos sin hacer nada
        }

        // Si pasó el tiempo, actualizamos el reloj y ejecutamos
        tiempoUltimoClick = Time.time;

        encendida = !encendida;
        Debug.Log("Inyectora Encendida: " + encendida);
        ActualizarLuces();
    }

    [ContextMenu("TEST: Botón Iniciar")]
    public void BotonIniciarCiclo()
    {
        if (Time.time - tiempoUltimoClick < esperaRebote) return; // Anti-rebote
        tiempoUltimoClick = Time.time;

        if (encendida && !procesoEnCurso)
        {
            StartCoroutine(ProcesoInyeccion());
        }
        else
        {
            Debug.Log("⚠️ La máquina está apagada u ocupada.");
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