using UnityEngine;

public class DatosCajaPellets : MonoBehaviour
{
    [Header("Conexión")]
    [Tooltip("Arrastra aquí el objeto que tiene el script LlenadoTolva")]
    public LlenadoTolva animacionTolva; 

    [Header("Contenido de la Caja")]
    public TipoPellet tipoDePlastico = TipoPellet.Virgen;
    public int cantidadQueAporta = 50; 

    public void NotificarVaciado()
    {
        if (animacionTolva != null)
        {
            // Le mandamos nuestros datos a la tolva para que inicie su animación
            animacionTolva.IniciarLlenado(tipoDePlastico, cantidadQueAporta);
        }
        else
        {
            Debug.LogWarning("La caja no tiene asignada la Tolva en el Inspector.");
        }
    }
}