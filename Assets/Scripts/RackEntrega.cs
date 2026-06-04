using System.Collections.Generic;
using UnityEngine;

public class RackEntrega : MonoBehaviour
{
    [Header("Efectos de Entrega ??")]
    public AudioSource reproductorRack;
    public AudioClip sonidoEntrega;

    // --- EL SEGURO CONTRA DOBLE COBRO ---
    // Aquí el rack anota qué cajas ya revisó
    private List<CajaRecoleccion> cajasProcesadas = new List<CajaRecoleccion>();

    void OnTriggerEnter(Collider other)
    {
        CajaRecoleccion cajaEntregada = other.GetComponentInParent<CajaRecoleccion>();

        // Si es una caja y está llena...
        if (cajaEntregada != null && cajaEntregada.contenidoCaja.Count == cajaEntregada.capacidadMaxima)
        {
            // Verificamos si ya la cobramos hace un milisegundo
            if (cajasProcesadas.Contains(cajaEntregada)) return;

            // Si es nueva, la anotamos en la lista y la procesamos
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

        // Se destruye la caja después de 1 segundo
        Destroy(caja.gameObject, 1f);
    }
}