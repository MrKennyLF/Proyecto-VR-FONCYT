using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

// Estructura para leer la respuesta de "POST Sesiones - Start"
[System.Serializable]
public class RespuestaStartSesionJSON
{
    public int sesion_id;
    public int tiempo_disponible_segundos; // Ajusta el nombre exacto según responda tu API
}

// Estructura para enviar el cierre de sesión
[System.Serializable]
public class DatosFinSesion
{
    public int sesion_id;
}

public class TemporizadorSesionVR : MonoBehaviour
{
    [Header("Interfaz del HUD ?")]
    public TextMeshProUGUI textoTiempo;

    [Header("Configuración de la API ??")]
    public string urlApiStart = "http://localhost:8080/wp-json/simulador/v1/sesiones/start";
    public string urlApiEnd = "http://localhost:8080/wp-json/simulador/v1/sesiones/end";
    public string apiKey = "change-this-local-api-key";

    private float tiempoRestanteSegundos = 0f;
    private int idSesionServidor = 0;
    private bool sesionActiva = false;
    private string tokenAuth = "";

    void Start()
    {
        // 1. Recuperamos la llave de seguridad que guardó el Login
        tokenAuth = PlayerPrefs.GetString("token_auth", "");

        if (string.IsNullOrEmpty(tokenAuth))
        {
            Debug.LogError("? No se encontró un Token válido. ¡Expulsando por seguridad!");
            SceneManager.LoadScene("LogScene");
            return;
        }

        // 2. En lugar de arrancar a ciegas, le pedimos permiso y tiempo al servidor
        StartCoroutine(RegistrarInicioSesionEnBD());
    }

    void Update()
    {
        if (sesionActiva && tiempoRestanteSegundos > 0)
        {
            tiempoRestanteSegundos -= Time.deltaTime;
            ActualizarRelojVisual();

            if (tiempoRestanteSegundos <= 0)
            {
                TerminarSesion();
            }
        }
    }

    IEnumerator RegistrarInicioSesionEnBD()
    {
        Debug.Log("?? Solicitando apertura de sesión al servidor...");

        // Si tu endpoint "start" requiere mandar un JSON vacío o algún parámetro, lo creas aquí.
        // Si no pide body, mandamos un string vacío "{}".
        string jsonBody = "{}";

        using (UnityWebRequest request = new UnityWebRequest(urlApiStart, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-Key", apiKey);
            request.SetRequestHeader("Authorization", "Bearer " + tokenAuth);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("? Error al iniciar sesión en el servidor: " + request.error);
                Debug.LogError("Respuesta: " + request.downloadHandler.text);
                // Si falla la verificación, lo regresamos al login por seguridad
                SceneManager.LoadScene("LogScene");
            }
            else
            {
                // 3. ¡Éxito! Parseamos la respuesta real del servidor
                string respuesta = request.downloadHandler.text;
                RespuestaStartSesionJSON datosSesion = JsonUtility.FromJson<RespuestaStartSesionJSON>(respuesta);

                idSesionServidor = datosSesion.sesion_id;

                // Si la API te devuelve el tiempo contratado por el usuario, lo asignamos aquí
                // Si no te lo devuelve y se calcula de otra forma, puedes dejar tu hora fija temporalmente:
                tiempoRestanteSegundos = datosSesion.tiempo_disponible_segundos > 0 ? datosSesion.tiempo_disponible_segundos : 3600f;

                // 4. Encendemos el reloj
                sesionActiva = true;
                Debug.Log($"? Sesión #{idSesionServidor} iniciada. Tiempo asignado: {tiempoRestanteSegundos}s");
            }
        }
    }

    void ActualizarRelojVisual()
    {
        if (textoTiempo == null) return;
        TimeSpan tiempo = TimeSpan.FromSeconds(tiempoRestanteSegundos);
        textoTiempo.text = string.Format("{0:D2}:{1:D2}:{2:D2}", tiempo.Hours, tiempo.Minutes, tiempo.Seconds);

        if (tiempoRestanteSegundos <= 300f) textoTiempo.color = Color.red;
        else textoTiempo.color = Color.white;
    }
    // --- FUNCIÓN PÚBLICA PARA EL BOTÓN DE SALIDA ---
    public void SalirAntesDeTiempo()
    {
        if (!sesionActiva) return; // Evita errores si le pican dos veces rápido

        Debug.Log("?? Salida manual activada por el usuario. Cerrando sesión...");

        // 1. Detenemos el reloj inmediatamente
        sesionActiva = false;

        // 2. Ejecutamos exactamente la misma lógica de cierre que hace el temporizador
        StartCoroutine(CerrarSesionEnBD());
    }
    void TerminarSesion()
    {
        sesionActiva = false;
        tiempoRestanteSegundos = 0;
        textoTiempo.text = "00:00:00";

        Debug.LogWarning("? TIEMPO AGOTADO. Clausurando sesión en backend...");
        StartCoroutine(CerrarSesionEnBD());
    }

    IEnumerator CancelarSesionManual()
    {
        // Por si necesitas una función para cuando el usuario se quita el visor 
        // o le pica a "Salir al menú" manualmente desde un botón de pausa
        yield return StartCoroutine(CerrarSesionEnBD());
    }

    IEnumerator CerrarSesionEnBD()
    {
        DatosFinSesion datos = new DatosFinSesion { sesion_id = idSesionServidor }; // Mandamos el ID real obtenido en el Start
        string jsonBody = JsonUtility.ToJson(datos);

        using (UnityWebRequest request = new UnityWebRequest(urlApiEnd, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-Key", apiKey);
            request.SetRequestHeader("Authorization", "Bearer " + tokenAuth);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("? Error al cerrar sesión en BD: " + request.error);
            }
            else
            {
                Debug.Log("? ¡Sesión cerrada correctamente en el servidor!");
            }

            // Pase lo que pase con la red, limpiamos credenciales y expulsamos
            PlayerPrefs.DeleteKey("token_auth");
            SceneManager.LoadScene("LogScene");
        }
    }
}