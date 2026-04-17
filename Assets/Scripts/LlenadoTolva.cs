using UnityEngine;
using System.Collections;

public class LlenadoTolva : MonoBehaviour
{
    [Header("Configuración Visual")]
    [Tooltip("Arrastra aquí el objeto 3D que simula la masa de pellets")]
    public Transform masaDePellets;

    [Tooltip("¿Cuántos segundos tarda en llenarse?")]
    public float tiempoDeLlenado = 4.0f;

    [Header("Sonido (Opcional)")]
    public AudioSource audioLlenado; // Sonido de piedritas cayendo

    private Vector3 escalaInicial;
    private bool estaLlena = false;

    void Start()
    {
        if (masaDePellets != null)
        {
            // 1. Guardamos el tamaño original (la tolva llena)
            escalaInicial = masaDePellets.localScale;

            // 2. Vaciamos la tolva al inicio aplastando la escala Y a 0
            masaDePellets.localScale = new Vector3(escalaInicial.x, 0, escalaInicial.z);
        }
    }

    // Esta es la función que conectarás a tu botón de "Llenar Tolva"
    public void IniciarLlenado()
    {
        if (!estaLlena && masaDePellets != null)
        {
            StartCoroutine(RutinaLlenado());
        }
    }

    IEnumerator RutinaLlenado()
    {
        if (audioLlenado != null) audioLlenado.Play();
        Debug.Log("? Llenando tolva...");

        float tiempoPasado = 0f;
        Vector3 escalaVacia = new Vector3(escalaInicial.x, 0, escalaInicial.z);

        // El ciclo while hace que crezca frame por frame
        while (tiempoPasado < tiempoDeLlenado)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoDeLlenado;

            // Lerp calcula el punto intermedio exacto entre vacío y lleno
            masaDePellets.localScale = Vector3.Lerp(escalaVacia, escalaInicial, porcentaje);

            yield return null; // Espera al siguiente frame
        }

        // Aseguramos que termine exactamente en el tamaño 100%
        masaDePellets.localScale = escalaInicial;
        estaLlena = true;

        if (audioLlenado != null) audioLlenado.Stop();
        Debug.Log("? Tolva llena al 100%");
    }
}