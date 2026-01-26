using UnityEngine;
using TMPro; // <--- ¡OJO! Esta línea es vital para que entienda qué es el texto

public class MotorInyectora : MonoBehaviour
{
    public float velocidad = 200f;
    public bool encendido = false;
    public AudioSource miAudio;
    public TextMeshPro textoBoton; // <--- Nueva casilla para arrastrar el texto

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
}