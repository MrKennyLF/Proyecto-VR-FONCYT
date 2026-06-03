using System.Collections;
using UnityEngine;

public class GeneradorCajas : MonoBehaviour
{
    [Header("Configuración del Generador ??")]
    [Tooltip("Arrastra aquí el PREFAB de tu Caja_Empaque desde la carpeta Project")]
    public GameObject prefabCaja;

    [Tooltip("Segundos de espera antes de que aparezca la nueva caja")]
    public float tiempoEspera = 10f;

    void Start()
    {
        // Generamos la primera caja al iniciar el juego al instante
        if (prefabCaja != null)
        {
            Instantiate(prefabCaja, transform.position, transform.rotation);
        }
    }

    // Esta función la llamará la caja actual justo cuando se llene
    public void IniciarConteoParaNuevaCaja()
    {
        StartCoroutine(CrearNuevaCaja());
    }

    IEnumerator CrearNuevaCaja()
    {
        Debug.Log($"? Generando nueva caja en {tiempoEspera} segundos...");
        yield return new WaitForSeconds(tiempoEspera);

        if (prefabCaja != null)
        {
            Instantiate(prefabCaja, transform.position, transform.rotation);
            Debug.Log("?? ¡Nueva caja generada en la banda!");
        }
    }
}