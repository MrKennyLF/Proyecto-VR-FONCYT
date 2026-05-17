using UnityEngine;
using System.Collections;

public class LlenadoTolva : MonoBehaviour
{
    [Header("Conexión Lógica 🧠")]
    public ControladorInyectora inyectora; 
    // ¡Eliminamos las variables de Tipo y Cantidad de aquí!

    [Header("Objetos Visuales 👁️")]
    public Transform esferaPellets;
    public Transform puntoVacio;
    public Transform puntoLleno;

    [Header("Configuración")]
    public float tiempoDeLlenado = 4.0f;
    public AudioSource audioLlenado;

    private bool estaLlena = false;

    // Variables secretas para guardar el dato mientras dura la animación
    private TipoPellet tipoPendiente;
    private int cantidadPendiente;

    void Start()
    {
        if (esferaPellets != null && puntoVacio != null)
        {
            esferaPellets.position = puntoVacio.position;
        }
    }

    void Update()
    {
        // El modo de prueba con teclado ahora manda Virgen por defecto para no romper el código
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IniciarLlenado(TipoPellet.Virgen, 100);
        }
    }

    // AHORA ESTA FUNCIÓN EXIGE QUE LE DIGAN QUÉ PLÁSTICO ES
    public void IniciarLlenado(TipoPellet plasticoEntrante, int cantidadEntrante)
    {
        if (!estaLlena && esferaPellets != null && puntoVacio != null && puntoLleno != null)
        {
            // Guardamos los datos temporalmente
            tipoPendiente = plasticoEntrante;
            cantidadPendiente = cantidadEntrante;
            StartCoroutine(RutinaLlenado());
        }
        else if (estaLlena)
        {
            Debug.LogWarning("⚠️ La tolva ya está llena.");
        }
    }

    IEnumerator RutinaLlenado()
    {
        if (audioLlenado != null) audioLlenado.Play();
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoDeLlenado)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoDeLlenado;
            esferaPellets.position = Vector3.Lerp(puntoVacio.position, puntoLleno.position, porcentaje);
            yield return null;
        }

        esferaPellets.position = puntoLleno.position;
        estaLlena = true;
        if (audioLlenado != null) audioLlenado.Stop();

        // Al terminar, le pasamos los datos exactos a la inyectora
        if (inyectora != null)
        {
            inyectora.AgregarPelletsATolva(tipoPendiente, cantidadPendiente);
            Debug.Log($"🧠 Datos enviados: {cantidadPendiente} de {tipoPendiente}");
        }
    }
}