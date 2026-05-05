using UnityEngine;
using System.Collections;

public class ZonaDeRiesgo : MonoBehaviour
{
    public enum TipoRiesgo { AltaTemperatura, Aplastamiento, RiesgoElectrico }
    
    [Header("Configuración del Riesgo")]
    public TipoRiesgo tipoDeRiesgo;
    public string mensajeAdvertencia = "Zona caliente detectada. Retroceda.";
    
    [Header("Penalización")]
    public float segundosParaHuir = 2.5f; // Tiempo que tiene para hacer caso
    public int puntosDeMulta = 20;

    private bool jugadorEnPeligro = false;

    private void OnTriggerEnter(Collider other)
    {
        // Detectamos si lo que entró a la zona es el jugador
        if (other.CompareTag("Player"))
        {
            jugadorEnPeligro = true;
            GestorSeguridadMaster.Instancia.MostrarAdvertenciaEspacial(mensajeAdvertencia);
            StartCoroutine(TemporizadorDePeligro());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si el jugador hace caso y sale a tiempo, cancelamos todo
        if (other.CompareTag("Player"))
        {
            jugadorEnPeligro = false;
            StopAllCoroutines();
            GestorSeguridadMaster.Instancia.LimpiarPantalla();
        }
    }

    private IEnumerator TemporizadorDePeligro()
    {
        // El código "espera" los segundos definidos
        yield return new WaitForSeconds(segundosParaHuir);

        // Si después de ese tiempo el jugador SIGUE adentro, castigo
        if (jugadorEnPeligro)
        {
            GestorSeguridadMaster.Instancia.RegistrarInfraccion($"Exposición a {tipoDeRiesgo}", puntosDeMulta);
            
            // Opcional: Reiniciamos el ciclo por si se queda ahí parado recibiendo daño continuo
            StartCoroutine(TemporizadorDePeligro());
        }
    }
}