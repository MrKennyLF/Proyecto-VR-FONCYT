using UnityEngine;

public class DistribuidorBosque : MonoBehaviour
{
    public GameObject prefabArbol;
    public int cantidadArboles = 50;
    public float radioPlano = 45f; // Un poco menos que los 50m del borde para que no floten

    [ContextMenu("Generar Barrera")]
    public void Generar()
    {
        for (int i = 0; i < cantidadArboles; i++)
        {
            // Calculamos una posición aleatoria SOLO en los bordes
            float angulo = i * Mathf.PI * 2 / cantidadArboles;
            float x = Mathf.Cos(angulo) * radioPlano + Random.Range(-5f, 5f);
            float z = Mathf.Sin(angulo) * radioPlano + Random.Range(-5f, 5f);

            GameObject nuevoArbol = Instantiate(prefabArbol, new Vector3(x, 0, z), Quaternion.identity, transform);

            // Variamos rotación y escala para que no se vean iguales
            nuevoArbol.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            float escala = Random.Range(0.8f, 1.5f);
            nuevoArbol.transform.localScale = new Vector3(escala, escala, escala);
        }
    }
}