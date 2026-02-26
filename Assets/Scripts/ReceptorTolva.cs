using UnityEngine;

public class ReceptorTolva : MonoBehaviour
{
    [Header("Conexiones")]
    public MeshRenderer materialDentroDeTolva;
    public ControladorInyectora controladorDeLaMaquina; // <-- Nueva conexión al cerebro

    void OnParticleCollision(GameObject other)
    {
        BoteMaterial bote = other.GetComponentInParent<BoteMaterial>();

        if (bote != null)
        {
            // 1. Pintamos el interior de la tolva (visual)
            materialDentroDeTolva.material.color = bote.colorPellets;

            // 2. Le pasamos el dato al cerebro de la máquina (lógica)
            if (controladorDeLaMaquina != null)
            {
                controladorDeLaMaquina.colorActualMaterial = bote.colorPellets;
            }
        }
    }
}