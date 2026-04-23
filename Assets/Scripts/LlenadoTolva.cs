using UnityEngine;
using System.Collections;

public class LlenadoTolva : MonoBehaviour
{
    [Header("Objetos")]
    public Transform esferaPellets;
    public Transform puntoVacio;
    public Transform puntoLleno;

    [Header("Configuración")]
    public float tiempoDeLlenado = 4.0f;
    public AudioSource audioLlenado;

    private bool estaLlena = false;

    void Start()
    {
        if (esferaPellets != null && puntoVacio != null)
        {
            esferaPellets.position = puntoVacio.position;
            Debug.Log("🏁 Inicio: Esfera posicionada en el fondo.");
        }
    }

    void Update()
    {
        // MODO PRUEBA: Presiona la BARRA ESPACIADORA en tu teclado para probar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("⌨️ Tecla ESPACIO presionada. Forzando llenado...");
            IniciarLlenado();
        }
    }

    public void IniciarLlenado()
    {
        Debug.Log("⚙️ Intentando iniciar llenado...");

        if (!estaLlena && esferaPellets != null && puntoVacio != null && puntoLleno != null)
        {
            StartCoroutine(RutinaLlenado());
        }
        else if (estaLlena)
        {
            Debug.LogWarning("⚠️ La tolva ya está llena.");
        }
        else
        {
            Debug.LogError("❌ ERROR: Faltan asignar la esfera o los puntos Vacio/Lleno en el Inspector.");
        }
    }

    IEnumerator RutinaLlenado()
    {
        Debug.Log("⏳ INICIANDO ANIMACIÓN DE LLENADO...");
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
        Debug.Log("✅ Tolva llena al 100%");
    }
}