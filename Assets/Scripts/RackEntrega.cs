using System.Collections.Generic;
using UnityEngine;

public class RackEntrega : MonoBehaviour
{
    [Header("Efectos de Entrega ??")]
    public AudioSource reproductorRack;
    public AudioClip sonidoEntrega;

    [Header("Exhibición en Rack ???")]
    [Tooltip("Crea objetos vacíos en la repisa y arrástralos aquí")]
    public Transform[] puntosDeAlmacenamiento;
    private int cajasAlmacenadas = 0;

    // Seguro contra doble cobro
    private List<CajaRecoleccion> cajasProcesadas = new List<CajaRecoleccion>();

    void OnTriggerEnter(Collider other)
    {
        CajaRecoleccion cajaEntregada = other.GetComponentInParent<CajaRecoleccion>();

        if (cajaEntregada != null && cajaEntregada.contenidoCaja.Count == cajaEntregada.capacidadMaxima)
        {
            if (cajasProcesadas.Contains(cajaEntregada)) return;

            cajasProcesadas.Add(cajaEntregada);
            ProcesarEntrega(cajaEntregada);
        }
    }

    void ProcesarEntrega(CajaRecoleccion caja)
    {
        List<TipoPellet> inventario = caja.contenidoCaja;

        int tazasVirgen = 0;
        int tazasReciclado = 0;
        int tazasMixto = 0;

        foreach (TipoPellet material in inventario)
        {
            if (material == TipoPellet.Virgen) tazasVirgen++;
            else if (material == TipoPellet.Reciclado) tazasReciclado++;
            else if (material == TipoPellet.Mixto) tazasMixto++;
        }

        Debug.Log($"?? REPORTE DE CAJA RECIBIDA: {tazasVirgen} Virgen | {tazasReciclado} Reciclado | {tazasMixto} Mixto");

        GestorPedidos gestor = Object.FindFirstObjectByType<GestorPedidos>();
        if (gestor != null)
        {
            gestor.ValidarEntrega(tazasVirgen, tazasReciclado, tazasMixto);
        }

        if (reproductorRack != null && sonidoEntrega != null)
        {
            reproductorRack.PlayOneShot(sonidoEntrega);
        }

        // En lugar de destruir, la acomodamos visualmente
        AcomodarCaja(caja.gameObject);
    }

    void AcomodarCaja(GameObject cajaFisica)
    {
        if (cajasAlmacenadas < puntosDeAlmacenamiento.Length)
        {
            // A. Apagamos el SDK de Meta para que el jugador ya no la pueda agarrar
            Behaviour grabbable = (Behaviour)cajaFisica.GetComponent("Grabbable");
            if (grabbable != null) grabbable.enabled = false;

            Behaviour kinLocker = (Behaviour)cajaFisica.GetComponent("RigidbodyKinematicLocker");
            if (kinLocker != null) kinLocker.enabled = false;

            // B. Congelamos las físicas
            Rigidbody rb = cajaFisica.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // C. Apagamos colisionadores para evitar bugs de físicas o que estorben
            Collider[] colisionadores = cajaFisica.GetComponentsInChildren<Collider>();
            foreach (Collider col in colisionadores) col.enabled = false;

            // D. Anclaje absoluto al punto del rack
            cajaFisica.transform.SetParent(puntosDeAlmacenamiento[cajasAlmacenadas]);
            cajaFisica.transform.localPosition = Vector3.zero;
            cajaFisica.transform.localRotation = Quaternion.identity;

            // Si quieres que las cajas se escalen automáticamente al tamaño del punto, descomenta la siguiente línea:
            // cajaFisica.transform.localScale = Vector3.one;

            cajasAlmacenadas++;
        }
        else
        {
            // Si el jugador hace más cajas de las que caben visualmente en el rack, se destruyen las excedentes
            Destroy(cajaFisica);
        }
    }
}