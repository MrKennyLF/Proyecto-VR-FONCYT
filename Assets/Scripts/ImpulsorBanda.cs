using UnityEngine;

public class ImpulsorBanda : MonoBehaviour
{
    [Header("Configuración de la Banda")]
    [Tooltip("Qué tan rápido se mueve el producto")]
    public float velocidad = 1.5f;
    
    [Tooltip("Eje local hacia donde empuja (Normalmente Z o X dependiendo de cómo exportaron de Blender)")]
    public Vector3 direccionLocal = Vector3.forward; // Vector3.forward es el eje Z azul

    void OnTriggerStay(Collider otro)
    {
        // Solo empuja objetos que tengan la etiqueta correcta para no empujar al jugador
        if (otro.CompareTag("PiezaPlastico"))
        {
            Rigidbody rb = otro.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calcula la dirección hacia donde empujar basado en la rotación de este trigger
                Vector3 direccionEmpuje = transform.TransformDirection(direccionLocal);
                
                // Aplicamos la velocidad, pero respetamos la gravedad en Y para que no floten
                Vector3 nuevaVelocidad = direccionEmpuje * velocidad;
                nuevaVelocidad.y = rb.linearVelocity.y; 
                
                rb.linearVelocity = nuevaVelocidad;
            }
        }
    }
}