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
        // Leemos AMBOS scripts de la caja desde donde vienen las partículas
        BoteMaterial bote = other.GetComponentInParent<BoteMaterial>();
        DatosCajaPellets datosCaja = other.GetComponentInParent<DatosCajaPellets>();

        // Nos aseguramos de que encontramos los dos componentes
        if (bote != null && datosCaja != null)
        {
            // 1. Pintamos el interior de la tolva (visual)
            if (materialDentroDeTolva != null)
            {
                materialDentroDeTolva.material.color = bote.colorPellets;
            }

            // 2. Le pasamos el dato del color al cerebro de la máquina (lógica de expulsión)
            if (controladorDeLaMaquina != null)
            {
                controladorDeLaMaquina.colorActualMaterial = bote.colorPellets;
            }

            // 3. Disparamos la nueva animación de elevación (Solo una vez)
            if (scriptAnimacionTolva != null && !animacionIniciada)
            {
                animacionIniciada = true; // Ponemos el seguro
                
                // LA CORRECCIÓN: Ahora le pasamos la llave correcta con el tipo de plástico y la cantidad
                scriptAnimacionTolva.IniciarLlenado(datosCaja.tipoDePlastico, datosCaja.cantidadQueAporta);
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