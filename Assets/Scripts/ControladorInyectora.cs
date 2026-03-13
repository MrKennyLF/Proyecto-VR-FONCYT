using UnityEngine;
using System.Collections;

public class ControladorInyectora : MonoBehaviour
{
    [Header("Estado de la Máquina")]
    public bool maquinaEncendida = false;
    private bool maquinaEnUso = false;

    [Header("Configuración de Pieza")]
    public GameObject moldePrefab;
    public Transform puntoDeSalida;
    public Color colorActualMaterial = Color.white;

    [Header("Tiempos Reales de Ciclo (Segundos)")]
    public float tiempoCierreMolde = 3f;
    public float tiempoInyeccion = 2f;
    public float tiempoCompactacion = 5f;
    public float tiempoEnfriamiento = 15f;
    public float tiempoApertura = 4f;

    [Header("Efectos de Sonido 🔊")]
    public AudioSource reproductorSonido;
    public AudioClip sonidoEncendido;        // <-- NUEVO: Para el botón de encender
    public AudioClip sonidoMecanica;
    public AudioClip sonidoInyeccion;
    public AudioClip sonidoEnfriamiento;
    public AudioClip sonidoExpulsion;

    // --- NUEVA FUNCIÓN: PARA EL BOTÓN DE ENCENDIDO ---
    public void BotonPrenderApagar()
    {
        if (maquinaEnUso)
        {
            Debug.LogWarning("⚠️ No puedes apagar la máquina a mitad de un ciclo.");
            return;
        }

        maquinaEncendida = !maquinaEncendida; // Cambia de apagado a prendido y viceversa

        if (maquinaEncendida)
        {
            Debug.Log("⚡ MÁQUINA ENCENDIDA");
            Reproducir(sonidoEncendido);
        }
        else
        {
            Debug.Log("🔌 MÁQUINA APAGADA");
            if (reproductorSonido.isPlaying) reproductorSonido.Stop(); // Silencia si se apaga
        }
    }

    // --- FUNCIÓN LIMPIA: PARA EL BOTÓN DE INICIAR CICLO ---
    public void IniciarCicloDeInyeccion()
    {
        // Seguro #1: ¿Está prendida?
        if (!maquinaEncendida)
        {
            Debug.LogWarning("❌ La máquina está apagada. Presiona el botón de encendido primero.");
            return;
        }

        // Seguro #2: ¿Ya está trabajando?
        if (!maquinaEnUso)
        {
            StartCoroutine(SecuenciaInyeccionReal());
        }
        else
        {
            Debug.Log("⏳ La máquina ya está en pleno ciclo. Espera a que termine.");
        }
    }

    IEnumerator SecuenciaInyeccionReal()
    {
        maquinaEnUso = true;
        Debug.Log("🟢 INICIANDO CICLO...");

        Reproducir(sonidoMecanica);
        Debug.Log("Cerrando molde...");
        yield return new WaitForSeconds(tiempoCierreMolde);

        Reproducir(sonidoInyeccion);
        Debug.Log("Inyectando polímero...");
        yield return new WaitForSeconds(tiempoInyeccion);

        Debug.Log("Compactando pieza...");
        yield return new WaitForSeconds(tiempoCompactacion);

        Reproducir(sonidoEnfriamiento);
        Debug.Log("Enfriando...");
        yield return new WaitForSeconds(tiempoEnfriamiento);

        Reproducir(sonidoMecanica);
        Debug.Log("Abriendo molde...");
        yield return new WaitForSeconds(tiempoApertura);

        Reproducir(sonidoExpulsion);
        ExpulsarPieza();

        maquinaEnUso = false;
        Debug.Log("✅ CICLO TERMINADO. Lista para otra pieza.");
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