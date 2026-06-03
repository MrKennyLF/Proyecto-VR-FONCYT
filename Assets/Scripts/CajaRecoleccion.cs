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

    // Lista para guardar las piezas físicas y destruirlas al cerrar la caja
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
            // Evitamos que el Trigger cuente la misma taza dos veces si rebota
            if (tazasFisicas.Contains(other.gameObject)) return;

            // 1. Leemos el ADN
            DatosPieza datosTaza = other.GetComponent<DatosPieza>();
            TipoPellet materialTaza = datosTaza != null ? datosTaza.materialDeLaPieza : TipoPellet.Ninguno;
            contenidoCaja.Add(materialTaza);

            // 2. ACOMODAMOS LA TAZA VISUALMENTE (VERSIÓN NUCLEAR)
            int indiceHueco = contenidoCaja.Count - 1;

            if (indiceHueco < puntosDeAnclaje.Length)
            {
                // A. Anclaje absoluto INMEDIATO
                other.transform.SetParent(puntosDeAnclaje[indiceHueco]);
                other.transform.localPosition = Vector3.zero;
                other.transform.localRotation = Quaternion.identity;

                // B. DESTRUIMOS LA FÍSICA PARA SIEMPRE
                // Al destruir el Rigidbody, Meta ya no tiene qué empujar
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                // Destruimos el script Grabbable para que Meta suelte el objeto obligatoriamente
                Behaviour grabbable = (Behaviour)other.GetComponent("Grabbable");
                if (grabbable != null) Destroy(grabbable);

                // Destruimos los colisionadores
                Collider[] colisionadores = other.GetComponentsInChildren<Collider>();
                foreach (Collider col in colisionadores) Destroy(col);

                // Guardamos la referencia para destruirla al cerrar la caja
                tazasFisicas.Add(other.gameObject);
            }
        }
        }

    void CerrarCaja()
    {
        cajaLlena = true;

        // Como la caja ya se cerró con su tapa, destruimos las tazas interiores 
        // para ahorrar RAM y evitar que la escena se sature de objetos
        foreach (GameObject taza in tazasFisicas)
        {
            if (taza != null) Destroy(taza);
        }

        // Transición visual y encendido de agarre de Meta
        if (modeloAbierta != null) modeloAbierta.SetActive(false);
        if (modeloCerrada != null) modeloCerrada.SetActive(true);
        if (componenteAgarre != null) componenteAgarre.enabled = true;

        Debug.Log("?? ¡Caja sellada!");

        // Avisamos al generador de la banda para que mande otra
        GeneradorCajas generador = Object.FindFirstObjectByType<GeneradorCajas>();
        if (generador != null) generador.IniciarConteoParaNuevaCaja();
    }
}