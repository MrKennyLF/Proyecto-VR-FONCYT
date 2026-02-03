using UnityEngine;
using TMPro;
using System; // <--- ¡OJO! Esta línea es vital para que entienda qué es el texto

public class MotorInyectora : MonoBehaviour
{
    public float velocidad = 200f;
    public bool encendido = false;
    public AudioSource miAudio;
    public TextMeshPro textoBoton; // <--- Nueva casilla para arrastrar el texto
    private bool encendida;
    private bool procesoEnCurso;
    private string nombreMaterial;
    private Renderer luzEstado;

    void Start()
    {
        // Al iniciar, nos aseguramos que diga "ENCENDER" (o lo que tú quieras)
        if (textoBoton != null) textoBoton.text = "ENCENDER";
    }

    void Update()
    {
        if (encendido)
        {
            // Girar tornillo (Eje Z, según lo que te funcionó)
            transform.Rotate(velocidad * Time.deltaTime, 0, 0);
        }
    }

    public void AlternarMotor()
    {
        encendido = !encendido; // Invierte estado

        if (encendido)
        {
            // ESTADO: PRENDIDO
            if (miAudio != null) miAudio.Play();
            if (textoBoton != null) textoBoton.text = "APAGAR"; // Cambia el mensaje
        }
        else
        {
            // ESTADO: APAGADO
            if (miAudio != null) miAudio.Stop();
            if (textoBoton != null) textoBoton.text = "ENCENDER"; // Regresa al mensaje original
        }
    }
    // --- AGREGA ESTAS LÍNEAS EN CORCHETES ---

    [ContextMenu("Simular Boton Encender")] // <--- ESTO
    public void BotonEncender()
    {
        encendida = !encendida;
        Debug.Log("Máquina Encendida: " + encendida);
        if (luzEstado != null)
        {
            luzEstado.material.color = encendida ? Color.green : Color.black;
        }
    }

    [ContextMenu("Simular Carga Material")] // <--- ESTO
    public void TestCargarMaterial()
    {
        // Función de prueba para no necesitar botones
        CargarPellets("Plástico TEST", Color.blue);
    }

    private void CargarPellets(string v, Color blue)
    {
        throw new NotImplementedException();
    }

    [ContextMenu("Simular Boton Iniciar")] // <--- ESTO
    public void BotonIniciarCiclo()
    {
        if (encendida && !procesoEnCurso && nombreMaterial != "Ninguno")
        {
            StartCoroutine(ProcesoInyeccion());
        }
        else
        {
            Debug.Log("?? Error: Enciende la máquina o carga material primero.");
        }
    }

    private string ProcesoInyeccion()
    {
        throw new NotImplementedException();
    }
}