using UnityEngine;
using System.Collections;
using TMPro;

// 1. DEFINICIONES DE DATOS
public enum TipoPellet { Ninguno, Virgen, Reciclado, Mixto }

[System.Serializable]
public class RecetaInyeccion
{
    public string nombreReceta;
    public TipoPellet tipoPelletRequerido;
    public float temperaturaObjetivo;
    public int cantidadPelletsRequerida;
    [Tooltip("Margen de error permitido para la temperatura (+/- grados)")]
    public float toleranciaTemperatura = 5f;
}

public class ControladorInyectora : MonoBehaviour
{
    [Header("Pantallas del Panel 📺")]
    public TextMeshProUGUI textoPantalla;
    public TextMeshProUGUI pantallaTemperatura; 

    [Header("Estado General de la Máquina")]
    public bool maquinaEncendida = false;
    private bool maquinaEnUso = false;

    [Header("Sistema de Calor 🔥")]
    public bool resistenciasEncendidas = false;
    public float temperaturaAmbiente = 25f;
    public float temperaturaMaximaCañon = 250f;
    [Tooltip("Grados por segundo. Bájalo a 1 o 1.5 si va muy rápido")]
    public float velocidadCalentamiento = 5f; 
    public float velocidadEnfriamiento = 2f;  

    [Header("Estabilidad Térmica ⏱️")]
    public float[] mesetasDeTemperatura = { 160f, 180f, 200f };
    public float tiempoEstabilizacion = 5f;
    private bool enPausaTermica = false;
    private int indiceMesetaActual = 0;

    [Header("Configuración de Pieza 📦")]
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

    [Header("Sistema de Recetas 📋")]
    public RecetaInyeccion[] recetasDisponibles = new RecetaInyeccion[3];
    private RecetaInyeccion recetaActual;
    private bool recetaCargada = false;

    [Header("Sensores de la Máquina (Estado Actual)")]
    public TipoPellet pelletEnTolvaActual = TipoPellet.Ninguno;
    public float temperaturaActual = 25f; 
    public int cantidadEnTolvaActual = 0;


    void Start()
    {
        temperaturaActual = temperaturaAmbiente;
        ActualizarPantalla("SISTEMA APAGADO");
    }

    void Update()
    {
        // --- CÁLCULO FÍSICO DE LA TEMPERATURA ---
        if (resistenciasEncendidas)
        {
            // Solo calienta si NO está en una pausa de estabilización
            if (!enPausaTermica)
            {
                if (temperaturaActual < temperaturaMaximaCañon)
                {
                    temperaturaActual += velocidadCalentamiento * Time.deltaTime;

                    // Revisamos si cruzamos la próxima meseta térmica
                    if (indiceMesetaActual < mesetasDeTemperatura.Length)
                    {
                        if (temperaturaActual >= mesetasDeTemperatura[indiceMesetaActual])
                        {
                            // Fijamos la temperatura exactamente en la meseta
                            temperaturaActual = mesetasDeTemperatura[indiceMesetaActual]; 
                            StartCoroutine(RutinaEstabilizacion());
                        }
                    }
                }
            }
        }
        else
        {
            // Si se apagan las resistencias, cancelamos pausas y enfriamos
            enPausaTermica = false;
            
            if (temperaturaActual > temperaturaAmbiente)
            {
                temperaturaActual -= velocidadEnfriamiento * Time.deltaTime;
            }
        }

        if (temperaturaActual < temperaturaAmbiente)
        {
            temperaturaActual = temperaturaAmbiente;
        }

        // --- ACTUALIZACIÓN VISUAL DEL TERMÓMETRO ---
        if (pantallaTemperatura != null)
        {
            pantallaTemperatura.text = temperaturaActual.ToString("F0") + " °C";

            if (temperaturaActual > 50f)
                pantallaTemperatura.color = Color.red;
            else
                pantallaTemperatura.color = Color.white;
        }
    }

    // --- FUNCIONES DE BOTONES (INTERFAZ VR) ---

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
            ActualizarPantalla("SISTEMA ENCENDIDO\nSELECCIONE RECETA");
            Reproducir(sonidoEncendido);
        }
        else
        {
            ActualizarPantalla("SISTEMA APAGADO");
            recetaCargada = false;
            recetaActual = null;
            resistenciasEncendidas = false; 
            enPausaTermica = false;
            if (reproductorSonido != null && reproductorSonido.isPlaying) reproductorSonido.Stop();
        }
    }

    public void ToggleCalentador()
    {
        if (!maquinaEncendida) return; 
        
        resistenciasEncendidas = !resistenciasEncendidas;
        
        if(resistenciasEncendidas)
        {
            enPausaTermica = false;
            RecalcularSiguienteMeseta();
            Debug.Log("Resistencias ENCENDIDAS");
        }
        else
        {
            Debug.Log("Resistencias APAGADAS");
        }
    }

    // Función auxiliar para saber qué meseta sigue si el calentador se apaga y se vuelve a encender a medias
    private void RecalcularSiguienteMeseta()
    {
        indiceMesetaActual = 0;
        for (int i = 0; i < mesetasDeTemperatura.Length; i++)
        {
            if (temperaturaActual < mesetasDeTemperatura[i])
            {
                indiceMesetaActual = i;
                break;
            }
            // Si la temperatura actual es mayor a la última meseta (ej. estamos a 210°C)
            if (i == mesetasDeTemperatura.Length - 1 && temperaturaActual >= mesetasDeTemperatura[i])
            {
                indiceMesetaActual = mesetasDeTemperatura.Length;
            }
        }
    }

    // --- CORRUTINA DE ESTABILIZACIÓN ---
    IEnumerator RutinaEstabilizacion()
    {
        enPausaTermica = true;
        Debug.Log("🔥 Meseta térmica alcanzada: " + temperaturaActual + "°C. Estabilizando por " + tiempoEstabilizacion + " segundos.");
        
        // Opcional: Puedes descomentar la siguiente línea si quieres que la pantalla principal avise de la estabilización
        // ActualizarPantalla("ESTABILIZANDO\nTEMPERATURA...");

        yield return new WaitForSeconds(tiempoEstabilizacion);
        
        indiceMesetaActual++;
        enPausaTermica = false;

        Debug.Log("🔥 Estabilización terminada. Retomando calentamiento hacia la siguiente fase.");
    }

    public void IntentarCargarReceta(int indiceBoton)
    {
        if (!maquinaEncendida)
        {
            ActualizarPantalla("ERROR: MAQUINA APAGADA");
            return;
        }

        if (maquinaEnUso)
        {
            ActualizarPantalla("ERROR: MAQUINA EN USO");
            return;
        }

        if (indiceBoton < 0 || indiceBoton >= recetasDisponibles.Length) return;

        RecetaInyeccion recetaSeleccionada = recetasDisponibles[indiceBoton];

        if (ValidarRequisitos(recetaSeleccionada))
        {
            recetaActual = recetaSeleccionada;
            recetaCargada = true;
            ActualizarPantalla("RECETA CARGADA:\n" + recetaActual.nombreReceta.ToUpper());
            Reproducir(sonidoEncendido);
        }
        else
        {
            recetaCargada = false;
            recetaActual = null;
            ActualizarPantalla("ERROR: REQUISITOS\nNO CUMPLIDOS");
        }
    }

    private bool ValidarRequisitos(RecetaInyeccion receta)
    {
        if (pelletEnTolvaActual != receta.tipoPelletRequerido) return false;
        if (cantidadEnTolvaActual < receta.cantidadPelletsRequerida) return false;

        float diferenciaTemp = Mathf.Abs(temperaturaActual - receta.temperaturaObjetivo);
        if (diferenciaTemp > receta.toleranciaTemperatura) return false;

        return true;
    }

    public void IniciarCicloDeInyeccion()
    {
        if (!maquinaEncendida)
        {
            ActualizarPantalla("ERROR: MAQUINA APAGADA");
            return;
        }

        if (!recetaCargada || recetaActual == null)
        {
            ActualizarPantalla("ERROR: SELECCIONE\nUNA RECETA");
            return;
        }

        if (!ValidarRequisitos(recetaActual))
        {
            ActualizarPantalla("ERROR: PARAMETROS\nFUERA DE RANGO");
            recetaCargada = false;
            recetaActual = null;
            return;
        }

        if (!maquinaEnUso)
        {
            StartCoroutine(SecuenciaInyeccionReal());
        }
    }

    IEnumerator EsperarYContar(float tiempoTotal, string mensajeProceso)
    {
        float tiempoRestante = tiempoTotal;

        while (tiempoRestante > 0)
        {
            ActualizarPantalla(mensajeProceso + "\n" + tiempoRestante.ToString("F1") + " s");
            tiempoRestante -= Time.deltaTime;
            yield return null; 
        }
    }

    IEnumerator SecuenciaInyeccionReal()
    {
        maquinaEnUso = true;
        cantidadEnTolvaActual -= recetaActual.cantidadPelletsRequerida;

        Reproducir(sonidoMecanica);
        if (animadorMaquina != null) animadorMaquina.SetTrigger("CerrarMolde");
        yield return StartCoroutine(EsperarYContar(tiempoCierreMolde, "CERRANDO MOLDE..."));

        Reproducir(sonidoInyeccion);
        yield return StartCoroutine(EsperarYContar(tiempoInyeccion, "INYECTANDO POLIMERO..."));

        yield return StartCoroutine(EsperarYContar(tiempoCompactacion, "COMPACTANDO PIEZA..."));

        Reproducir(sonidoEnfriamiento);
        yield return StartCoroutine(EsperarYContar(tiempoEnfriamiento, "ENFRIANDO..."));

        Reproducir(sonidoMecanica);
        if (animadorMaquina != null) animadorMaquina.SetTrigger("AbrirMolde");
        yield return StartCoroutine(EsperarYContar(tiempoApertura, "ABRIENDO MOLDE..."));

        ActualizarPantalla("PIEZA EXPULSADA\nLISTO");
        Reproducir(sonidoExpulsion);
        ExpulsarPieza();

        recetaCargada = false;
        recetaActual = null;
        maquinaEnUso = false;
    }

    public void AgregarPelletsATolva(TipoPellet tipoAgregado, int cantidadAgregada)
    {
        if (cantidadEnTolvaActual == 0)
        {
            pelletEnTolvaActual = tipoAgregado;
            ActualizarPantalla("TOLVA ALIMENTADA:\n" + tipoAgregado.ToString().ToUpper());
        }
        else if (pelletEnTolvaActual != tipoAgregado)
        {
            pelletEnTolvaActual = TipoPellet.Mixto;
            ActualizarPantalla("ALERTA:\nMATERIAL MIXTO");
        }

        cantidadEnTolvaActual += cantidadAgregada;
        Debug.Log("Pellets agregados. Tipo actual: " + pelletEnTolvaActual + " | Cantidad total: " + cantidadEnTolvaActual);
    }

    public void PurgarTolva()
    {
        cantidadEnTolvaActual = 0;
        pelletEnTolvaActual = TipoPellet.Ninguno;
        ActualizarPantalla("TOLVA PURGADA\nY VACIA");
    }

    void ActualizarPantalla(string mensaje)
    {
        if (textoPantalla != null) textoPantalla.text = mensaje;
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