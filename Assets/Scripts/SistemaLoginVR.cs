using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

// Molde por si tu API devuelve el error estructurado en un JSON (Ej: {"message": "Sin licencias"})
[System.Serializable]
public class DatosLogin
{
    public string correo;
    public string password;
}

[System.Serializable]
public class DatosUsuarioJSON
{
    public string id;
    public string nombre;
    public string apellido;
    public string correo;
    public string rol;
    public string procedencia;
    public int empresa_id;
}

[System.Serializable]
public class RespuestaLoginJSON
{
    public string token;
    public string token_type;
    public int expires_in;
    public DatosUsuarioJSON usuario;
}
[System.Serializable]

public class RespuestaErrorJSON
{
    public string message;
}

public class SistemaLoginVR : MonoBehaviour
{
    [Header("Campos de Texto")]
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContrasena;

    [Header("Texto de Alertas UI ??")]
    public TextMeshProUGUI textoErrorUI; // <-- ARRASTRA AQUÍ EL NUEVO TEXTO ROJO

    [Header("Botón Real de Iniciar")]
    public Button botonIniciar;

    [Header("Teclado Casero")]
    public GameObject panelTeclado;

    [Header("Configuración de la API ??")]
    public string urlApi = "http://localhost:8080/wp-json/simulador/v1/auth/login";
    public string apiKey = "change-this-local-api-key";

    void Start()
    {
        // Nos aseguramos de que el texto empiece vacío al arrancar
        LimpiarError();

        if (botonIniciar != null)
        {
            botonIniciar.onClick.AddListener(IntentarAcceso);
        }
    }

    public void IntentarAcceso()
    {
        LimpiarError(); // Limpiamos cualquier error de un intento previo

        string correoIngresado = inputUsuario.text;
        string contrasenaIngresada = inputContrasena.text;

        if (string.IsNullOrEmpty(correoIngresado) || string.IsNullOrEmpty(contrasenaIngresada))
        {
            MostrarErrorEnPantalla("?? Por favor llena ambos campos.");
            return;
        }

        if (panelTeclado != null) panelTeclado.SetActive(false);

        StartCoroutine(EnviarDatosLoginJSON(correoIngresado, contrasenaIngresada));
    }

    IEnumerator EnviarDatosLoginJSON(string correo, string pass)
    {
        DatosLogin datos = new DatosLogin { correo = correo, password = pass };
        string jsonBody = JsonUtility.ToJson(datos);

        using (UnityWebRequest request = new UnityWebRequest(urlApi, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-Key", apiKey);

            yield return request.SendWebRequest();

            // --- MANEJO DE ERRORES EN PANTALLA ---
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                MostrarErrorEnPantalla("? Error de red. Verifica la conexión de las gafas.");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                // Opción A: Mapear por códigos de estado HTTP estándar de tu backend
                if (request.responseCode == 401)
                {
                    MostrarErrorEnPantalla("? Credenciales inválidas. Verifica tu correo o contraseña.");
                }
                else if (request.responseCode == 403)
                {
                    MostrarErrorEnPantalla("? Acceso denegado: Sin licencias disponibles o tiempo agotado.");
                }
                else
                {
                    // Opción B: Intentar leer el mensaje que mandó el servidor textualmente
                    try
                    {
                        string respuestaError = request.downloadHandler.text;
                        RespuestaErrorJSON errorJson = JsonUtility.FromJson<RespuestaErrorJSON>(respuestaError);

                        if (!string.IsNullOrEmpty(errorJson.message))
                            MostrarErrorEnPantalla($"? {errorJson.message}");
                        else
                            MostrarErrorEnPantalla($"? Error {request.responseCode} en el servidor.");
                    }
                    catch
                    {
                        MostrarErrorEnPantalla($"? Error inesperado ({request.responseCode}).");
                    }
                }
            }
            else
            {
                // ÉXITO (Mismo código anterior)
                string textoRespuesta = request.downloadHandler.text;
                RespuestaLoginJSON datosExtraidos = JsonUtility.FromJson<RespuestaLoginJSON>(textoRespuesta);

                PlayerPrefs.SetString("token_auth", datosExtraidos.token);
                PlayerPrefs.SetString("usuario_id", datosExtraidos.usuario.id);
                PlayerPrefs.SetString("usuario_nombre", datosExtraidos.usuario.nombre + " " + datosExtraidos.usuario.apellido);
                PlayerPrefs.Save();

                SceneManager.LoadScene("MainScene");
            }
        }
    }

    // Funciones auxiliares para controlar la UI de forma limpia
    void MostrarErrorEnPantalla(string mensaje)
    {
        if (textoErrorUI != null)
        {
            textoErrorUI.text = mensaje;
            textoErrorUI.gameObject.SetActive(true);
        }
        Debug.LogError(mensaje); // Lo dejamos también en consola por si acaso
    }

    void LimpiarError()
    {
        if (textoErrorUI != null)
        {
            textoErrorUI.text = "";
            textoErrorUI.gameObject.SetActive(false);
        }
    }
}