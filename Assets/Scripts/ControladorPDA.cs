using UnityEngine;
using TMPro;

public class ControladorPDA : MonoBehaviour
{
    [Header("Conexión a la Máquina 🧠")]
    public ControladorInyectora inyectora;

    [Header("Páginas de la Pantalla (GameObjects) 📁")]
    public GameObject paginaTemperaturas;
    public GameObject paginaTiempos;
    public GameObject paginaCapacidad;
    public GameObject paginaCantidades;
    public GameObject paginaTiposPellets;
    public GameObject paginaRecetas;

    [Header("Texto Dinámico (Solo para Recetas) 📋")]
    [Tooltip("Arrastra aquí únicamente el componente TextMeshPro que está dentro de Pagina_Recetas")]
    public TextMeshProUGUI cuerpoTextoRecetas;

    void Start()
    {
        // 1. Solo generamos el texto dinámico para la página de recetas
        GenerarContenidoRecetas();

        // 2. Mostramos la primera página por defecto (Las demás conservan su texto de Unity intacto)
        MostrarTemperaturas();
    }

    void GenerarContenidoRecetas()
    {
        if (inyectora == null || cuerpoTextoRecetas == null) return;

        string contenido = "<b><color=#5A9BD5>RECETAS DISPONIBLES EN SISTEMA</color></b>\n\n";

        for (int i = 0; i < inyectora.recetasDisponibles.Length; i++)
        {
            RecetaInyeccion r = inyectora.recetasDisponibles[i];
            if (!string.IsNullOrEmpty(r.nombreReceta))
            {
                contenido += $"<b>[Receta:] - {r.nombreReceta.ToUpper()}</b>\n";
                contenido += $"• Material Requerido: <b>{r.tipoPelletRequerido}</b>\n";
                contenido += $"• Temperatura: <b>{r.temperaturaObjetivo}°C</b> (±{r.toleranciaTemperatura}°C)\n";
                contenido += $"• Volumen de Carga: <b>{r.cantidadPelletsRequerida} u.</b>\n\n";
            }
        }

        cuerpoTextoRecetas.text = contenido;
    }

    // --- CONTROL DE PESTAÑAS (TU LÓGICA ORIGINAL) ---

    public void MostrarTemperaturas() { ApagarTodas(); if (paginaTemperaturas != null) paginaTemperaturas.SetActive(true); }
    public void MostrarTiempos() { ApagarTodas(); if (paginaTiempos != null) paginaTiempos.SetActive(true); }
    public void MostrarCapacidad() { ApagarTodas(); if (paginaCapacidad != null) paginaCapacidad.SetActive(true); }
    public void MostrarCantidades() { ApagarTodas(); if (paginaCantidades != null) paginaCantidades.SetActive(true); }
    public void MostrarTiposPellets() { ApagarTodas(); if (paginaTiposPellets != null) paginaTiposPellets.SetActive(true); }
    public void MostrarRecetas() { ApagarTodas(); if (paginaRecetas != null) paginaRecetas.SetActive(true); }

    void ApagarTodas()
    {
        if (paginaTemperaturas != null) paginaTemperaturas.SetActive(false);
        if (paginaTiempos != null) paginaTiempos.SetActive(false);
        if (paginaCapacidad != null) paginaCapacidad.SetActive(false);
        if (paginaCantidades != null) paginaCantidades.SetActive(false);
        if (paginaTiposPellets != null) paginaTiposPellets.SetActive(false);
        if (paginaRecetas != null) paginaRecetas.SetActive(false);
    }
} 