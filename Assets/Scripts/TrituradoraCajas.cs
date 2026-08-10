using UnityEngine;

public class TrituradoraCajas : MonoBehaviour
{
    [Header("Efectos de Reciclaje ??")]
    public AudioSource reproductorTrituradora;
    public AudioClip sonidoDestruccion;

    void OnTriggerEnter(Collider other)
    {
        // Buscamos si el objeto que entró tiene el script de la caja
        // Usamos GetComponentInParent por si chocó con el colisionador de una de las tazas adentro
        CajaRecoleccion cajaBasura = other.GetComponentInParent<CajaRecoleccion>();

        if (cajaBasura != null)
        {
            // Reproducimos un sonido (ej. plástico crujiendo o máquina triturando)
            if (reproductorTrituradora != null && sonidoDestruccion != null)
            {
                reproductorTrituradora.PlayOneShot(sonidoDestruccion);
            }

            Debug.Log("?? Caja defectuosa eliminada del área de trabajo.");

            // Destruimos la caja física completa
            Destroy(cajaBasura.gameObject);
        }
    }
}