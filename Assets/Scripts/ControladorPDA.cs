using UnityEngine;

public class ControladorPDA : MonoBehaviour
{
    [Header("Páginas de la Pantalla")]
    public GameObject paginaTemperaturas;
    public GameObject paginaTiempos;
    public GameObject paginaCapacidad; // Para los datos del tornillo

    void Start()
    {
        // Al encender el PDA, mostramos la primera por defecto
        MostrarTemperaturas();
    }

    public void MostrarTemperaturas()
    {
        ApagarTodas();
        if (paginaTemperaturas != null) paginaTemperaturas.SetActive(true);
    }

    public void MostrarTiempos()
    {
        ApagarTodas();
        if (paginaTiempos != null) paginaTiempos.SetActive(true);
    }

    public void MostrarCapacidad()
    {
        ApagarTodas();
        if (paginaCapacidad != null) paginaCapacidad.SetActive(true);
    }

    // Función auxiliar para limpiar la pantalla antes de mostrar algo nuevo
    void ApagarTodas()
    {
        if (paginaTemperaturas != null) paginaTemperaturas.SetActive(false);
        if (paginaTiempos != null) paginaTiempos.SetActive(false);
        if (paginaCapacidad != null) paginaCapacidad.SetActive(false);
    }
}