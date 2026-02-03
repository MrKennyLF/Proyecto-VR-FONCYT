using UnityEngine;
using UnityEngine.Events; // Necesario para conectar cosas

public class BotonVR : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaPresion = 0.02f; // Cuánto se hunde el botón (2cm)
    public float velocidadRetorno = 5.0f;  // Qué tan rápido vuelve a subir
    public string tagMano = "PlayerHand";  // Etiqueta de tus manos VR

    [Header("Eventos")]
    public UnityEvent AlPresionar; // ¡Aquí arrastraremos la función de la máquina!

    private Vector3 posicionInicial;
    private bool estaPresionado = false;

    void Start()
    {
        posicionInicial = transform.localPosition; // Guardamos dónde empieza
    }

    void Update()
    {
        // Lógica visual: Si no lo tocan, vuelve a su sitio
        if (!estaPresionado)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, posicionInicial, Time.deltaTime * velocidadRetorno);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si entra algo con el tag "PlayerHand" y el botón no estaba presionado...
        if (other.CompareTag(tagMano) && !estaPresionado)
        {
            Presionar();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagMano))
        {
            estaPresionado = false; // Soltamos el botón
        }
    }

    void Presionar()
    {
        estaPresionado = true;

        // 1. Efecto visual (Hundir el botón)
        Vector3 posicionHundida = new Vector3(posicionInicial.x, posicionInicial.y - distanciaPresion, posicionInicial.z);
        transform.localPosition = posicionHundida;

        // 2. Disparar la acción
        Debug.Log("¡Botón Tocado!");
        AlPresionar.Invoke();
    }
}