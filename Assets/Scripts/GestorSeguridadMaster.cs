using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GestorSeguridadMaster : MonoBehaviour
{
    public static GestorSeguridadMaster Instancia;

    [Header("Estado Actual")]
    public int puntajeTotal = 0;
    private List<DatosEPP> equipoEquipado = new List<DatosEPP>();

    [Header("Interfaz Profesional (PDA)")]
    public TextMeshProUGUI textoPuntajePDA;
    public TextMeshProUGUI textoRetroalimentacion;

    [Header("Audio y Feedback")]
    public AudioSource fuenteDeAudio; // La bocina
    public AudioClip audioExito;      // El sonido de "ding" o correcto
    public AudioClip audioError;      // El sonido de chicharra o error

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
    }

    public void ProcesarSeleccionEPP(DatosEPP itemSeleccionado)
    {
        equipoEquipado.Add(itemSeleccionado);
        puntajeTotal += itemSeleccionado.puntosAsignados;

        ActualizarPDA();

        if (itemSeleccionado.esCorrecto)
        {
            ReproducirSonido(audioExito);
            MostrarFeedback($"[SISTEMA]: {itemSeleccionado.nombreEquipo} asegurado correctamente.", Color.green);
        }
        else
        {
            ReproducirSonido(audioError);
            VibrarMando(0.5f, 0.3f); 
            MostrarFeedback($"[ALERTA]: {itemSeleccionado.mensajeAlerta}", Color.red);
        }
    }

    private void ActualizarPDA()
    {
        if (textoPuntajePDA != null)
            textoPuntajePDA.text = $"Certificación: {puntajeTotal} pts";
    }

    private void MostrarFeedback(string mensaje, Color colorMensaje)
    {
        if (textoRetroalimentacion != null)
        {
            textoRetroalimentacion.text = mensaje;
            textoRetroalimentacion.color = colorMensaje;
        }
    }

    // --- EL MÉTODO QUE FALTABA ---
    private void ReproducirSonido(AudioClip clip)
    {
        if (fuenteDeAudio != null && clip != null)
        {
            fuenteDeAudio.PlayOneShot(clip);
        }
    }

    // --- HAPTICS ---
    void VibrarMando(float amplitud, float duracion)
    {
        OVRInput.SetControllerVibration(amplitud, amplitud, OVRInput.Controller.RTouch);
        Invoke("DetenerVibracion", duracion);
    }

    void DetenerVibracion()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    // --- REPORTE ---
    public void GenerarReporteFinal()
    {
        string ruta = Application.persistentDataPath + "/Reporte_Seguridad_VR.txt";
        
        using (StreamWriter writer = new StreamWriter(ruta, true))
        {
            writer.WriteLine("--- REPORTE DE SIMULACIÓN VR-FONCYT ---");
            writer.WriteLine("Fecha: " + System.DateTime.Now.ToString());
            writer.WriteLine("Puntaje Final de Seguridad: " + puntajeTotal);
            writer.WriteLine("Equipos utilizados:");
            foreach(var item in equipoEquipado)
            {
                writer.WriteLine("- " + item.nombreEquipo + (item.esCorrecto ? " (Correcto)" : " (Incorrecto)"));
            }
            writer.WriteLine("---------------------------------------");
        }
        Debug.Log("📂 Reporte guardado en: " + ruta);
    }
    public void RegistrarInfraccion(string motivo, int puntosPerdidos)
    {
        puntajeTotal -= puntosPerdidos; 
        ActualizarPDA();
        
        // Alarma fuerte y vibración intensa por el accidente
        ReproducirSonido(audioError);
        VibrarMando(1.0f, 0.6f); 
        MostrarFeedback($"[INFRACCIÓN CRÍTICA]: {motivo} (-{puntosPerdidos} pts)", Color.red);
    }

    public void MostrarAdvertenciaEspacial(string mensaje)
    {
        // Vibración sutil de advertencia
        VibrarMando(0.3f, 0.2f);
        MostrarFeedback($"[PRECAUCIÓN]: {mensaje}", Color.yellow);
    }

    public void LimpiarPantalla()
    {
        MostrarFeedback("[SISTEMA ESTABLE]", Color.green);
    }
}