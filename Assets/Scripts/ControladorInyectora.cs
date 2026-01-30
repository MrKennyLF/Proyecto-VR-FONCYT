using UnityEngine;
using System.Collections;

public class ControladorInyectora : MonoBehaviour
{
    [Header("Estado de la Máquina")]
    public bool encendida = false;
    public bool procesoEnCurso = false;

    [Header("Materiales (Pellets)")]
    // Aquí guardamos el color del material actual (Ej. Rojo para ABS, Transparente para PP)
    public Color colorMaterialActual = Color.white;
    public string nombreMaterial = "Ninguno";

    [Header("Configuración")]
    public Transform puntoDeSalida; // Dónde aparece la pieza final
    public GameObject moldePrefab;  // El objeto 3D que sale (ej. una carcasa o vaso)
    public float tiempoDeInyeccion = 5.0f; // Cuánto tarda el proceso

    [Header("Efectos (Opcional)")]
    public AudioSource sonidoMaquina;
    public Light luzEstado; // Verde = Lista, Rojo = Trabajando

    // --- FUNCIONES QUE TUS BOTONES VR VAN A LLAMAR ---

    public void BotonEncender()
    {
        encendida = !encendida; // Alternar encendido/apagado
        Debug.Log("Máquina Encendida: " + encendida);

        if (luzEstado != null) luzEstado.color = encendida ? Color.green : Color.black;
    }

    public void CargarPellets(string tipo, Color color)
    {
        if (!procesoEnCurso)
        {
            nombreMaterial = tipo;
            colorMaterialActual = color;
            Debug.Log("Material Cargado: " + tipo);
        }
    }

    public void BotonIniciarCiclo()
    {
        if (encendida && !procesoEnCurso && nombreMaterial != "Ninguno")
        {
            StartCoroutine(ProcesoInyeccion());
        }
        else
        {
            Debug.Log("Error: Máquina apagada o sin material.");
        }
    }

    // --- LA MAGIA (CORRUTINA) ---
    IEnumerator ProcesoInyeccion()
    {
        procesoEnCurso = true;
        Debug.Log("Iniciando Inyección...");

        if (luzEstado != null) luzEstado.color = Color.red; // Luz roja trabajando
        if (sonidoMaquina != null) sonidoMaquina.Play();

        // Esperamos el tiempo del proceso (Simulación)
        yield return new WaitForSeconds(tiempoDeInyeccion);

        // Crear la pieza final
        ExpulsarPieza();

        procesoEnCurso = false;
        if (luzEstado != null) luzEstado.color = Color.green; // Luz verde lista
        Debug.Log("Ciclo Terminado.");
    }

    void ExpulsarPieza()
    {
        // 1. Crear el objeto en el punto de salida
        GameObject piezaNueva = Instantiate(moldePrefab, puntoDeSalida.position, puntoDeSalida.rotation);

        // 2. Pintarla del color de los pellets
        Renderer rend = piezaNueva.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = colorMaterialActual;
        }
    }
    void Update()
    {
        // Solo para pruebas en PC
        if (Input.GetKeyDown(KeyCode.E)) BotonEncender();
        if (Input.GetKeyDown(KeyCode.M)) CargarPellets("ABS Rojo", Color.red);
        if (Input.GetKeyDown(KeyCode.Space)) BotonIniciarCiclo();
    }
}