using UnityEngine;

public class BotonVR : MonoBehaviour
{
    // Esto crea un menú desplegable en el Inspector de Unity
    public enum TipoDeBoton { Encendido, IniciarCiclo }

    [Header("--- FUNCIÓN DE ESTE BOTÓN ---")]
    public TipoDeBoton queHaceEsteBoton = TipoDeBoton.IniciarCiclo;

    [Header("--- CONEXIÓN ---")]
    public ControladorInyectora maquinaInyectora;

    [Header("--- (Opcional) LA PIEZA DE ESTE BOTÓN ---")]
    [Tooltip("Solo necesitas llenar esto si es un botón de Iniciar Ciclo")]
    public GameObject piezaParaEsteBoton;

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

    // AHORA ES PÚBLICA, el Oculus Interaction SDK ya la puede ver
    public void Presionar()
    {
        estaPresionado = true;
        transform.localPosition = new Vector3(posicionInicial.x, posicionInicial.y - distanciaPresion, posicionInicial.z);

        if (maquinaInyectora != null)
        {
            // ¿Qué tipo de botón soy?
            if (queHaceEsteBoton == TipoDeBoton.Encendido)
            {
                Debug.Log("🔘 Botón de Encendido presionado.");
                maquinaInyectora.BotonPrenderApagar();
            }
            else if (queHaceEsteBoton == TipoDeBoton.IniciarCiclo)
            {
                Debug.Log("🔘 Botón de Iniciar Ciclo presionado.");
                if (piezaParaEsteBoton != null)
                {
                    maquinaInyectora.moldePrefab = piezaParaEsteBoton;
                }
                maquinaInyectora.IniciarCicloDeInyeccion();
            }
        }
    }

    private void OnMouseDown() { Presionar(); }
}