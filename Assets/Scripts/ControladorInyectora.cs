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

    [Header("Material")]
    public Color colorActualMaterial = Color.white; // Blanco por defecto si no le echan nada

    [Header("--- CONFIGURACIÓN VISUAL (OPCIONAL) ---")]
    public Light luzEstado; // La luz de la sirena
    public AudioSource audioMaquina; // Sonido de trabajo
    private float tiempoUltimoClick = 0f; // Para evitar el rebote
    private float esperaRebote = 0.5f;    // Medio segundo de espera


    // =========================================================
    // --- NUEVA FUNCIÓN DINÁMICA PARA LOS BOTONES VR ---
    // =========================================================
    public void IniciarCicloConPieza(GameObject piezaDesdeBoton)
    {
        // 1. Verificamos anti-rebote
        if (Time.time - tiempoUltimoClick < esperaRebote) return;
        tiempoUltimoClick = Time.time;

        if (encendida && !procesoEnCurso)
        {
            // 2. ACTUALIZACIÓN DEL MOLDE
            if (piezaDesdeBoton != null)
            {
                moldePrefab = piezaDesdeBoton;
                // Mensaje en Cyan para identificar el cambio de pieza
                Debug.Log("<color=cyan>🔧 INYECTORA: Molde actualizado a -> </color>" + piezaDesdeBoton.name);
            }
            else
            {
                Debug.LogWarning("⚠️ El botón no envió ninguna pieza, usando la que estaba por defecto.");
            }

            StartCoroutine(ProcesoInyeccion());
        }
        else
        {
            Debug.Log("<color=orange>⚠️ La máquina está apagada u ocupada.</color>");
        }
    }


    // =========================================================
    // --- TUS FUNCIONES ORIGINALES (BOTONES SIMPLES) ---
    // =========================================================

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
    public void BotonIniciarCiclo() // Tu función original para iniciar sin cambiar pieza
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


    // =========================================================
    // --- LA LÓGICA INTERNA ---
    // =========================================================

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
            // 1. Instanciamos la pieza y la guardamos en la variable 'nuevaPieza'
            GameObject nuevaPieza = Instantiate(moldePrefab, puntoDeSalida.position, puntoDeSalida.rotation);

            // 2. Buscamos su "pintor" (MeshRenderer) en la pieza o en sus hijos
            MeshRenderer rendererPieza = nuevaPieza.GetComponentInChildren<MeshRenderer>();

            // 3. Si lo encontramos, le aplicamos el color de los pellets
            if (rendererPieza != null)
            {
                // Para asegurarnos de crear una instancia del material y no cambiar el original del proyecto
                rendererPieza.material.color = colorActualMaterial;
                Debug.Log("🎨 Pieza pintada de color: " + colorActualMaterial);
            }
            else
            {
                Debug.LogWarning("⚠️ La pieza no tiene MeshRenderer, no se pudo pintar.");
            }
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