using System.Collections.Generic;
using UnityEngine;

public class CajaRecoleccion : MonoBehaviour
{
    [Header("Modelos Visuales ??")]
    public GameObject modeloAbierta;
    public GameObject modeloCerrada;

    [Header("Interacción VR (Meta) ???")]
    public Behaviour componenteAgarre;

    [Header("Posiciones de Tazas ??")]
    [Tooltip("Arrastra aquí los 4 objetos vacíos (Punto_1, Punto_2...)")]
    public Transform[] puntosDeAnclaje;

    [Header("Lógica de Empaque")]
    public int capacidadMaxima = 4;

    public List<TipoPellet> contenidoCaja = new List<TipoPellet>();

    private List<GameObject> tazasFisicas = new List<GameObject>();
    private bool cajaLlena = false;

    void Start()
    {
        if (modeloAbierta != null) modeloAbierta.SetActive(true);
        if (modeloCerrada != null) modeloCerrada.SetActive(false);
        if (componenteAgarre != null) componenteAgarre.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (cajaLlena) return;

        if (other.CompareTag("PiezaPlastico"))
        {
            // Evitamos contar la misma taza dos veces
            if (tazasFisicas.Contains(other.gameObject)) return;

            // Leemos el ADN
            DatosPieza datosTaza = other.GetComponent<DatosPieza>();
            TipoPellet materialTaza = datosTaza != null ? datosTaza.materialDeLaPieza : TipoPellet.Ninguno;
            contenidoCaja.Add(materialTaza);

            int indiceHueco = contenidoCaja.Count - 1;

            if (indiceHueco < puntosDeAnclaje.Length)
            {
                // A. Apagamos el SDK de Meta para que no interfiera
                Behaviour grabbable = (Behaviour)other.GetComponent("Grabbable");
                if (grabbable != null) grabbable.enabled = false;

                Behaviour kinLocker = (Behaviour)other.GetComponent("RigidbodyKinematicLocker");
                if (kinLocker != null) kinLocker.enabled = false;

                // B. Congelamos las físicas
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                // C. Apagamos los colisionadores
                Collider[] colisionadores = other.GetComponentsInChildren<Collider>();
                foreach (Collider col in colisionadores) col.enabled = false;

                // D. Anclaje absoluto y herencia de escala (para que se adapte al punto)
                other.transform.SetParent(puntosDeAnclaje[indiceHueco]);
                other.transform.localPosition = Vector3.zero;
                other.transform.localRotation = Quaternion.identity;
                other.transform.localScale = Vector3.one;

                tazasFisicas.Add(other.gameObject);
            }

            Debug.Log($"Taza colocada: {materialTaza}. Total: {contenidoCaja.Count}/4");

            // Si llegamos a la capacidad, llamamos a la función de abajo
            if (contenidoCaja.Count >= capacidadMaxima)
            {
                CerrarCaja();
            }
        }
    }

    // Esta función ahora está correctamente afuera del OnTriggerEnter
    void CerrarCaja()
    {
        cajaLlena = true;

        // Destruimos las mallas de las tazas para ahorrar RAM
        foreach (GameObject taza in tazasFisicas)
        {
            if (taza != null) Destroy(taza);
        }

        // Intercambio de modelos y encendido de agarre
        if (modeloAbierta != null) modeloAbierta.SetActive(false);
        if (modeloCerrada != null) modeloCerrada.SetActive(true);
        if (componenteAgarre != null) componenteAgarre.enabled = true;

        Debug.Log("?? ¡Caja sellada y lista para el rack!");

        // Llamamos al Spawner para que mande otra caja
        GeneradorCajas generador = Object.FindFirstObjectByType<GeneradorCajas>();
        if (generador != null) generador.IniciarConteoParaNuevaCaja();
    }
}