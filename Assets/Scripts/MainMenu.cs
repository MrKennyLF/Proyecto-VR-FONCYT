using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <-- NUEVO: Necesario para que Unity entienda qué es un "Slider"
using TMPro;          // <-- NUEVO: Necesario para actualizar los textos de los porcentajes

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelPrincipal;
    public GameObject panelOpciones;
    public GameObject panelCreditos;

    [Header("Configuración de Escena")]
    public string nombreEscenaSimulacion = "LaboratorioInyeccion";

    [Header("Ajustes del Sistema (Nuevos)")]
    public Slider sliderVolumen;
    public TextMeshProUGUI textoPorcentajeVolumen;
    public Slider sliderBrillo;
    public TextMeshProUGUI textoPorcentajeBrillo;

    void Start()
    {
        // Aseguramos que inicie en la pantalla correcta
        MostrarPanelPrincipal();

        // Igualamos el slider al volumen real de Unity al iniciar
        if (sliderVolumen != null)
        {
            sliderVolumen.value = AudioListener.volume * 100f;
            ActualizarVolumen(sliderVolumen.value);
        }
    }

    // --- TUS FUNCIONES ORIGINALES INTACTAS ---

    public void IniciarSimulacion()
    {
        Debug.Log("Cargando simulador VR-FONCYT...");
        SceneManager.LoadScene(nombreEscenaSimulacion);
    }

    public void MostrarPanelOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
        panelCreditos.SetActive(false);
    }

    public void MostrarPanelCreditos()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void MostrarPanelPrincipal()
    {
        panelPrincipal.SetActive(true);
        panelOpciones.SetActive(false);
        panelCreditos.SetActive(false);
    }

    public void Salir()
    {
        Debug.Log("Cerrando simulador...");
        Application.Quit();
    }

    // --- NUEVAS FUNCIONES PARA LOS SLIDERS ---

    public void ActualizarVolumen(float valor)
    {
        // El audio general de Unity va de 0 a 1. Dividimos el valor del slider (0-100) entre 100.
        AudioListener.volume = valor / 100f; 
        
        // Actualizamos el texto en pantalla
        if (textoPorcentajeVolumen != null) 
        {
            textoPorcentajeVolumen.text = "Volumen General: " + Mathf.RoundToInt(valor) + "%";
        }
    }

    public void ActualizarBrillo(float valor)
    {
        // Actualizamos el texto en pantalla
        if (textoPorcentajeBrillo != null) 
        {
            textoPorcentajeBrillo.text = "Brillo de Pantalla: " + Mathf.RoundToInt(valor) + "%";
        }

        // Nota técnica: Cambiar el brillo real dentro de las gafas VR suele requerir 
        // perfiles de Post-Processing. Por ahora, el slider actualizará el texto perfectamente.
    }

    public void ReiniciarAjustes()
    {
        // Esta función va en tu botón de "Reiniciar Interfaz"
        if (sliderVolumen != null) sliderVolumen.value = 75f;
        if (sliderBrillo != null) sliderBrillo.value = 80f;
    }
}