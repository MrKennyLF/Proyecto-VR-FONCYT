using UnityEngine;
using System.Collections;

public class LlenadoTolva : MonoBehaviour
{
    [Header("Objetos")]
    [Tooltip("La esfera con la textura de pellets")]
    public Transform esferaPellets; 
    
    [Tooltip("Crea un objeto vacío donde la tolva está vacía (escondido abajo)")]
    public Transform puntoVacio; 
    
    [Tooltip("Crea un objeto vacío donde la tolva está llena (arriba)")]
    public Transform puntoLleno; 

    [Header("Configuración")]
    public float tiempoDeLlenado = 4.0f; 
    public AudioSource audioLlenado;

    private bool estaLlena = false;

    void Start()
    {
        // Al iniciar, escondemos la esfera en el punto más bajo
        if (esferaPellets != null && puntoVacio != null)
        {
            esferaPellets.position = puntoVacio.position;
        }
    }

    public void IniciarLlenado()
    {
        if (!estaLlena && esferaPellets != null && puntoVacio != null && puntoLleno != null)
        {
            StartCoroutine(RutinaLlenado());
        }
        else if (estaLlena)
        {
            Debug.Log("La tolva ya está llena.");
        }
        else
        {
            Debug.LogError("Faltan asignar objetos en el inspector de LlenadoTolva.");
        }
    }

    IEnumerator RutinaLlenado()
    {
        if (audioLlenado != null) audioLlenado.Play();
        Debug.Log("⏳ Llenando tolva...");

        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoDeLlenado)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoDeLlenado;

            // En lugar de escalar, MOvemos la esfera de abajo hacia arriba
            esferaPellets.position = Vector3.Lerp(puntoVacio.position, puntoLleno.position, porcentaje);

            yield return null; 
        }

        // Aseguramos que termine exactamente en la posición final
        esferaPellets.position = puntoLleno.position;
        estaLlena = true;
        
        if (audioLlenado != null) audioLlenado.Stop();
        Debug.Log("✅ Tolva llena al 100%");
    }
}