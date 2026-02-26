using UnityEngine;

public class BotonVR : MonoBehaviour
{
    [Header("--- CONEXIÓN (ARRASTRA LA INYECTORA AQUÍ) ---")]
    public ControladorInyectora maquinaInyectora;

    [Header("--- LA PIEZA DE ESTE BOTÓN ---")]
    public GameObject piezaParaEsteBoton; // Arrastra el modelo/prefab que quieres que salga

    [Header("Ajustes Físicos")]
    public float distanciaPresion = 0.02f;
    public float velocidadRetorno = 5.0f;
    public string tagMano = "PlayerHand";

    private Vector3 posicionInicial;
    private bool estaPresionado = false;

    void Start() { posicionInicial = transform.localPosition; }

    void Update()
    {
        if (!estaPresionado)
            transform.localPosition = Vector3.Lerp(transform.localPosition, posicionInicial, Time.deltaTime * velocidadRetorno);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagMano) && !estaPresionado) Presionar();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagMano)) estaPresionado = false;
    }

    void Presionar()
    {
        estaPresionado = true;
        transform.localPosition = new Vector3(posicionInicial.x, posicionInicial.y - distanciaPresion, posicionInicial.z);

        // HABLAMOS DIRECTAMENTE CON LA MÁQUINA (Sin UnityEvents)
        if (maquinaInyectora != null && piezaParaEsteBoton != null)
        {
            Debug.Log("🔘 Botón presionado. Enviando modelo: " + piezaParaEsteBoton.name);
            maquinaInyectora.IniciarCicloConPieza(piezaParaEsteBoton);
        }
        else
        {
            Debug.LogError("❌ ¡Falta arrastrar la Inyectora o la Pieza en el Inspector de este botón!");
        }
    }

    // Para tus pruebas con el mouse
    private void OnMouseDown() { Presionar(); }
}