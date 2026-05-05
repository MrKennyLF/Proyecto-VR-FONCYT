using UnityEngine;

public class SensorEquipamiento : MonoBehaviour
{
    [Header("Filtro de Zona")]
    [Tooltip("¿Qué parte del cuerpo es esta?")]
    public DatosEPP.Categoria zonaDelCuerpo;

    private void OnTriggerEnter(Collider other)
    {
        // Buscamos si el objeto que acaba de chocar tiene nuestro script de EPP
        InteractuableEPP itemPeligroso = other.GetComponentInParent<InteractuableEPP>();

        if (itemPeligroso != null && itemPeligroso.fichaTecnica != null)
        {
            // Verificamos que no intente ponerse un guante en la cabeza
            if (itemPeligroso.fichaTecnica.tipoDeEquipo == zonaDelCuerpo)
            {
                // ¡Es el objeto correcto! Lo equipamos
                itemPeligroso.EquiparItem();
            }
        }
    }
}