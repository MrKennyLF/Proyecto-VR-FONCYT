using UnityEngine;
using TMPro;

// Esta clase define cómo es un "Pedido"
[System.Serializable]
public class Pedido
{
    public string nombreCliente;
    public int tazasVirgenRequeridas;
    public int tazasRecicladoRequeridas;
    public int tazasMixtoRequeridas;
}

public class GestorPedidos : MonoBehaviour
{
    [Header("Lista de Misiones ??")]
    [Tooltip("Agrega aquí los pedidos que el jugador debe cumplir en orden")]
    public Pedido[] pedidosDisponibles;

    private Pedido pedidoActual;
    private int indicePedidoActual = 0;

    [Header("Interfaz de Usuario ??")]
    public TextMeshProUGUI pantallaPedidos;

    [Header("Retroalimentación ??")]
    public AudioSource audioPedidos;
    public AudioClip sonidoExito;
    public AudioClip sonidoError;

    void Start()
    {
        // Al iniciar la escena, cargamos la primera misión
        if (pedidosDisponibles.Length > 0) CargarPedido(0);
    }

    void CargarPedido(int indice)
    {
        pedidoActual = pedidosDisponibles[indice];
        ActualizarPantalla();
    }

    // El Rack llamará a esta función para entregarle las cuentas
    public void ValidarEntrega(int virgenEntregado, int recicladoEntregado, int mixtoEntregado)
    {
        if (pedidoActual == null) return;

        // Comparamos lo que pide la misión vs lo que traía la caja
        if (virgenEntregado == pedidoActual.tazasVirgenRequeridas &&
            recicladoEntregado == pedidoActual.tazasRecicladoRequeridas &&
            mixtoEntregado == pedidoActual.tazasMixtoRequeridas)
        {
            Debug.Log("? ¡PEDIDO COMPLETADO CON ÉXITO!");
            if (audioPedidos != null && sonidoExito != null) audioPedidos.PlayOneShot(sonidoExito);

            // --- NUEVO: Mensaje de ÉXITO a la PDA ---
            if (GestorSeguridadMaster.Instancia != null)
            {
                GestorSeguridadMaster.Instancia.MostrarFeedback("? [LOGÍSTICA]: Caja aceptada. Excelente trabajo.", Color.green);
            }

            AvanzarSiguientePedido();
        }
        else
        {
            Debug.LogWarning("? ERROR DE CALIDAD: LA CAJA NO COINCIDE CON EL PEDIDO.");
            if (audioPedidos != null && sonidoError != null) audioPedidos.PlayOneShot(sonidoError);

            // --- NUEVO: Mensaje de ERROR a la PDA ---
            if (GestorSeguridadMaster.Instancia != null)
            {
                GestorSeguridadMaster.Instancia.MostrarFeedback("? [RECHAZO]: La caja no cumple la receta del cliente.", Color.red);
            }
        }
    }

    void AvanzarSiguientePedido()
    {
        indicePedidoActual++;

        if (indicePedidoActual < pedidosDisponibles.Length)
        {
            CargarPedido(indicePedidoActual);
        }
        else
        {
            // Si ya no hay más pedidos en la lista
            pedidoActual = null;
            if (pantallaPedidos != null)
            {
                pantallaPedidos.text = "<color=#70AD47><b>¡TURNO TERMINADO!</b>\nTodas las entregas completadas.\nReporte de seguridad guardado.</color>";
            }

            // --- EL CIERRE DE TURNO ---
            if (GestorSeguridadMaster.Instancia != null)
            {
                // Llama a la función que ya tenías programada para crear el TXT
                GestorSeguridadMaster.Instancia.GenerarReporteFinal();

                // Opcional: Sonido de victoria usando la bocina del gestor
                if (sonidoExito != null && audioPedidos != null)
                {
                    audioPedidos.PlayOneShot(sonidoExito);
                }

                Debug.Log("?? ¡Juego terminado! Reporte guardado en la computadora.");
            }
        }
    }
    void ActualizarPantalla()
    {
        if (pantallaPedidos != null && pedidoActual != null)
        {
            pantallaPedidos.text = $"<b>CLIENTE: {pedidoActual.nombreCliente}</b>\n\n" +
                                   $"Requisitos de Empaque (4 u.):\n" +
                                   $"- Material Virgen: <b>{pedidoActual.tazasVirgenRequeridas}</b>\n" +
                                   $"- Material Reciclado: <b>{pedidoActual.tazasRecicladoRequeridas}</b>\n" +
                                   $"- Material Mixto: <b>{pedidoActual.tazasMixtoRequeridas}</b>";
        }
    }
}