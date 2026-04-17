using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorInyectora : MonoBehaviour
{
    [Header("Pantalla del Panel 📺")]
    public TextMeshProUGUI textoPantalla;

    [Header("Estado de la Máquina")]
    public bool maquinaEncendida = false;
    private bool maquinaEnUso = false;

    [Header("Configuración de Pieza")]
    public GameObject moldePrefab;
    public Transform puntoDeSalida;
    public Color colorActualMaterial = Color.white;

    [Header("Animación 🎬")]
    public Animator animadorMaquina;

    [Header("Tiempos Reales de Ciclo (Segundos)")]
    public float tiempoCierreMolde = 3f;
    public float tiempoInyeccion = 2f;
    public float tiempoCompactacion = 5f;
    public float tiempoEnfriamiento = 15f;
    public float tiempoApertura = 4f;

    [Header("Efectos de Sonido 🔊")]
    public AudioSource reproductorSonido;
    public AudioClip sonidoEncendido;
    public AudioClip sonidoMecanica;
    public AudioClip sonidoInyeccion;
    public AudioClip sonidoEnfriamiento;
    public AudioClip sonidoExpulsion;

    void ActualizarPantalla(string mensaje)
    {
        if (textoPantalla != null)
        {
            textoPantalla.text = mensaje;
        }
    }

    public void BotonPrenderApagar()
    {
        if (maquinaEnUso)
        {
            ActualizarPantalla("ERROR: CICLO EN PROGRESO");
            return;
        }

        maquinaEncendida = !maquinaEncendida;

        if (maquinaEncendida)
        {
            ActualizarPantalla("SISTEMA ENCENDIDO\nLISTO PARA INYECTAR");
            Reproducir(sonidoEncendido);
        }
        else
        {
            ActualizarPantalla(""); // Apaga la pantalla
            if (reproductorSonido != null && reproductorSonido.isPlaying) reproductorSonido.Stop();
        }
    }

    public void IniciarCicloDeInyeccion()
    {
        if (!maquinaEncendida)
        {
            ActualizarPantalla("ERROR: MAQUINA APAGADA");
            return;
        }

        if (!maquinaEnUso)
        {
            StartCoroutine(SecuenciaInyeccionReal());
        }
    }

    // --- NUEVO CRONÓMETRO: Cuenta regresiva fluida ---
    IEnumerator EsperarYContar(float tiempoTotal, string mensajeProceso)
    {
        float tiempoRestante = tiempoTotal;

        while (tiempoRestante > 0)
        {
            // ToString("F1") formatea el número para mostrar solo 1 decimal (Ej: 2.5 s)
            ActualizarPantalla(mensajeProceso + "\n" + tiempoRestante.ToString("F1") + " s");

            // Restamos el tiempo que tardó el último frame
            tiempoRestante -= Time.deltaTime;

            // Esperamos al siguiente frame para volver a actualizar
            yield return null;
        }
    }

    IEnumerator SecuenciaInyeccionReal()
    {
        maquinaEnUso = true;

        // 1. CIERRE DEL MOLDE
        Reproducir(sonidoMecanica);
        if (animadorMaquina != null) animadorMaquina.SetTrigger("CerrarMolde");
        yield return StartCoroutine(EsperarYContar(tiempoCierreMolde, "CERRANDO MOLDE..."));

        // 2. INYECCIÓN
        Reproducir(sonidoInyeccion);
        yield return StartCoroutine(EsperarYContar(tiempoInyeccion, "INYECTANDO POLIMERO..."));

        // 3. COMPACTACIÓN
        yield return StartCoroutine(EsperarYContar(tiempoCompactacion, "COMPACTANDO PIEZA..."));

        // 4. ENFRIAMIENTO
        Reproducir(sonidoEnfriamiento);
        yield return StartCoroutine(EsperarYContar(tiempoEnfriamiento, "ENFRIANDO..."));

        // 5. APERTURA Y EXPULSIÓN
        Reproducir(sonidoMecanica);
        if (animadorMaquina != null) animadorMaquina.SetTrigger("AbrirMolde");
        yield return StartCoroutine(EsperarYContar(tiempoApertura, "ABRIENDO MOLDE..."));

        // 6. PIEZA LISTA
        ActualizarPantalla("PIEZA EXPULSADA\nLISTO");
        Reproducir(sonidoExpulsion);
        ExpulsarPieza();

        maquinaEnUso = false;
    }

    void Reproducir(AudioClip clip)
    {
        if (reproductorSonido != null && clip != null)
        {
            reproductorSonido.clip = clip;
            reproductorSonido.Play();
        }
    }

    void ExpulsarPieza()
    {
        if (moldePrefab != null && puntoDeSalida != null)
        {
            GameObject nuevaPieza = Instantiate(moldePrefab, puntoDeSalida.position, puntoDeSalida.rotation);
            MeshRenderer[] todosLosRenderers = nuevaPieza.GetComponentsInChildren<MeshRenderer>();

            if (todosLosRenderers.Length > 0)
            {
                foreach (MeshRenderer rend in todosLosRenderers)
                {
                    foreach (Material mat in rend.materials) mat.color = colorActualMaterial;
                }
            }
        }
    }
}