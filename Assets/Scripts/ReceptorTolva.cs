using UnityEngine;

public class ReceptorTolva : MonoBehaviour
{
    [Header("Conexiones Originales")]
    public MeshRenderer materialDentroDeTolva;
    public ControladorInyectora controladorDeLaMaquina;

    [Header("Conexión Animación (Nueva)")]
    [Tooltip("Arrastra aquí el objeto que tiene el script LlenadoTolva")]
    public LlenadoTolva scriptAnimacionTolva;

    // Este seguro evita que la animación se dispare 100 veces por segundo con las partículas
    private bool animacionIniciada = false;

    void OnParticleCollision(GameObject other)
    {
        BoteMaterial bote = other.GetComponentInParent<BoteMaterial>();

        if (bote != null)
        {
            // 1. Pintamos el interior de la tolva (visual)
            if (materialDentroDeTolva != null)
            {
                materialDentroDeTolva.material.color = bote.colorPellets;
            }

            // 2. Le pasamos el dato al cerebro de la máquina (lógica)
            if (controladorDeLaMaquina != null)
            {
                controladorDeLaMaquina.colorActualMaterial = bote.colorPellets;
            }

            // 3. Disparamos la nueva animación de elevación (Solo una vez)
            if (scriptAnimacionTolva != null && !animacionIniciada)
            {
                animacionIniciada = true; // Ponemos el seguro
                scriptAnimacionTolva.IniciarLlenado();
            }
        }
    }

    // Llama a esta función desde otro script si en algún momento "vacías" la máquina 
    // y necesitas que la tolva pueda volver a llenarse en el futuro.
    public void ResetearSensor()
    {
        animacionIniciada = false;
    }
}