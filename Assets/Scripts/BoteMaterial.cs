using UnityEngine;

public class BoteMaterial : MonoBehaviour
{
    [Header("Tipo de Plástico")]
    public Color colorPellets = Color.yellow; // El color de este bote
    [Header("Configuración Visual")]
    public ParticleSystem particulasPellets;
    public Transform masaPellets; // El cubo de adentro

    [Header("Ajustes de Vaciado")]
    public float velocidadVaciado = 0.05f;
    public float anguloParaVerter = 0.2f; // Detecta cuando se inclina más de 90 grados

    private bool estaVertiendo = false;

    void Update()
    {
        // transform.up.y lee hacia dónde apunta "arriba" la caja.
        // Si es menor a 0.2, significa que está inclinada hacia abajo.
        if (transform.up.y < anguloParaVerter && masaPellets.localScale.y > 0)
        {
            if (!estaVertiendo)
            {
                particulasPellets.Play();
                estaVertiendo = true;
            }

            // Reducir el cubo visualmente para simular que se vacía
            Vector3 nuevaEscala = masaPellets.localScale;
            nuevaEscala.y -= velocidadVaciado * Time.deltaTime;

            // Evitar que se vuelva negativo
            if (nuevaEscala.y < 0) nuevaEscala.y = 0;

            masaPellets.localScale = nuevaEscala;
        }
        else
        {
            // Si la enderezas o se vacía, detén las partículas
            if (estaVertiendo)
            {
                particulasPellets.Stop();
                estaVertiendo = false;
            }
        }
    }
}