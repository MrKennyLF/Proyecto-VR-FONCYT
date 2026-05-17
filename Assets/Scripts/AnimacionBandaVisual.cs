using UnityEngine;

public class AnimacionBandaVisual : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de deslizamiento en el eje X e Y de la textura. Juega con estos valores hasta que coincida con la velocidad del objeto físico.")]
    public Vector2 velocidadDesplazamiento = new Vector2(0f, -0.5f);

    private Renderer renderizador;
    private Material materialBanda;
    private Vector2 offsetActual;

    void Start()
    {
        // Obtenemos el componente que dibuja el modelo 3D (MeshRenderer)
        renderizador = GetComponent<Renderer>();

        if (renderizador != null)
        {
            // Clonamos y guardamos el material en memoria para no buscarlo cada fotograma (Optimización vital para VR)
            materialBanda = renderizador.material;
        }
        else
        {
            Debug.LogWarning("No se encontró un Renderer en la banda para animar la textura.");
        }
    }

    void Update()
    {
        if (materialBanda != null)
        {
            // Calculamos cuánto debe moverse la textura en este fotograma exacto
            offsetActual += velocidadDesplazamiento * Time.deltaTime;

            // Truco de optimización: Mantenemos el offset entre 0 y 1 (como un reloj). 
            // Así evitamos que la memoria intente calcular números gigantes si la máquina se deja prendida por horas.
            offsetActual.x = offsetActual.x % 1f;
            offsetActual.y = offsetActual.y % 1f;

            // Aplicamos el movimiento visual a la textura
            materialBanda.mainTextureOffset = offsetActual;
            
            // ⚠️ NOTA SI ESTÁN USANDO URP (Universal Render Pipeline):
            // Si le das Play y la textura no se mueve, es porque URP llama a las texturas de otra forma.
            // En ese caso, borra la línea de arriba y usa esta:
            // materialBanda.SetTextureOffset("_BaseMap", offsetActual);
        }
    }
}