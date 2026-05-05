using UnityEngine;

public class MapeoAvatarVR : MonoBehaviour
{
    [Header("Los Sensores (Meta VR)")]
    public Transform cabezaVR;        // El CenterEyeAnchor
    public Transform manoIzquierdaVR; // El LeftControllerAnchor
    public Transform manoDerechaVR;   // El RightControllerAnchor

    [Header("Los Hilos del Títere (Targets IK)")]
    public Transform targetCabezaIK;      
    public Transform seguidorManoIzq;  // Cambiamos el nombre aquí
    public Transform seguidorManoDer;  // Cambiamos el nombre aquí

    [Header("Configuración de Altura")]
    public float offsetAlturaCabeza = 0.1f; // Ajuste fino para que el cuello no se vea raro

    void Update()
    {
        // 1. Mover el cuerpo para que siga los pasos del jugador en el mundo físico
        // El cuerpo sigue la posición X y Z de la cabeza, pero se queda en el suelo (Y = 0)
        transform.position = new Vector3(cabezaVR.position.x, transform.position.y, cabezaVR.position.z);

        // 2. Rotar el cuerpo para que mire hacia donde mira el casco
        Vector3 rotacionCabeza = cabezaVR.eulerAngles;
        transform.rotation = Quaternion.Euler(0, rotacionCabeza.y, 0);

        // Pegar los Seguidores a tus manos reales
        seguidorManoIzq.position = manoIzquierdaVR.position;
        seguidorManoIzq.rotation = manoIzquierdaVR.rotation;

        seguidorManoDer.position = manoDerechaVR.position;
        seguidorManoDer.rotation = manoDerechaVR.rotation;

        // 4. Alinear la cabeza del modelo con el visor
        if (targetCabezaIK != null)
        {
            targetCabezaIK.position = cabezaVR.position - new Vector3(0, offsetAlturaCabeza, 0);
            targetCabezaIK.rotation = cabezaVR.rotation;
        }
    }
}