using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class TecladoAutoVR : MonoBehaviour
{
    [Header("Tus Campos de Texto")]
    public TMP_InputField inputMatricula; // Ahora lo usaremos para el Correo
    public TMP_InputField inputContrasena;

    private TMP_InputField campoActivo;
    private bool mayusculasActivas = false;

    [Header("Aspecto de las Teclas")]
    public TMP_FontAsset fuenteTexto;

    // Lista para guardar el componente visual de los botones y cambiar su texto
    private List<TextMeshProUGUI> textosDeTeclas = new List<TextMeshProUGUI>();

    // Capa 1: Minúsculas, Números y Símbolos de Correo (45 teclas)
    private string[] teclasMin = {
        "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
        "q", "w", "e", "r", "t", "y", "u", "i", "o", "p",
        "a", "s", "d", "f", "g", "h", "j", "k", "l", "ñ",
        "MAYUS", "z", "x", "c", "v", "b", "n", "m", ".", "@",
        "_", "-", "BORRAR", "ESPACIO", "ENTER"
    };

    // Capa 2: Mayúsculas y Símbolos Secundarios (45 teclas)
    private string[] teclasMayus = {
        "!", "\"", "#", "$", "%", "&", "/", "(", ")", "=",
        "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
        "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ñ",
        "minus", "Z", "X", "C", "V", "B", "N", "M", ",", "?",
        "_", "-", "BORRAR", "ESPACIO", "ENTER"
    };

    void Start()
    {
        campoActivo = inputMatricula;
        GenerarTecladoFisico();
    }

    // --- FUNCIONES EXCLUSIVAS PARA LOS LÁSERES ---
    public void EnfocarMatricula()
    {
        campoActivo = inputMatricula;
        this.gameObject.SetActive(true);
    }

    public void EnfocarContrasena()
    {
        campoActivo = inputContrasena;
        this.gameObject.SetActive(true);
    }
    // ----------------------------------------------

    void GenerarTecladoFisico()
    {
        for (int i = 0; i < teclasMin.Length; i++)
        {
            int indexActual = i;

            GameObject nuevoBoton = new GameObject("Tecla_" + i);
            nuevoBoton.transform.SetParent(this.transform, false);

            Image imagenFondo = nuevoBoton.AddComponent<Image>();
            Button componenteBoton = nuevoBoton.AddComponent<Button>();

            // --- MEJORA DE FRONTEND: COLORES Y HOVER ---
            ColorBlock colores = componenteBoton.colors;
            colores.normalColor = new Color32(40, 40, 40, 255);       // Gris oscuro elegante
            colores.highlightedColor = new Color32(80, 80, 80, 255);  // Gris claro (Cuando el láser lo toca)
            colores.pressedColor = new Color32(0, 150, 255, 255);     // Azul eléctrico (Al hacer clic)
            colores.selectedColor = new Color32(40, 40, 40, 255);     // Evita que se quede "pegado" visualmente
            colores.colorMultiplier = 1f;
            colores.fadeDuration = 0.1f; // Transición suave
            componenteBoton.colors = colores;
            // -------------------------------------------

            GameObject objetoTexto = new GameObject("Texto");
            objetoTexto.transform.SetParent(nuevoBoton.transform, false);
            TextMeshProUGUI textoTMP = objetoTexto.AddComponent<TextMeshProUGUI>();

            textoTMP.text = teclasMin[i];
            textoTMP.color = Color.white; // Texto en blanco para contrastar
            textoTMP.fontSizeMax = 20;
            textoTMP.enableAutoSizing = true;
            textoTMP.alignment = TextAlignmentOptions.Center;

            // Si tienes una fuente asignada en el Inspector, la aplica
            if (fuenteTexto != null) textoTMP.font = fuenteTexto;

            RectTransform rectTexto = objetoTexto.GetComponent<RectTransform>();
            rectTexto.anchorMin = Vector2.zero;
            rectTexto.anchorMax = Vector2.one;
            rectTexto.offsetMin = new Vector2(2, 2); // Un poco de margen interno para las letras
            rectTexto.offsetMax = new Vector2(-2, -2);

            textosDeTeclas.Add(textoTMP);
            componenteBoton.onClick.AddListener(() => TeclaPresionada(indexActual));
        }
    }

    void TeclaPresionada(int indiceTecla)
    {
        if (campoActivo == null) return;

        // Revisamos en qué capa estamos para saber qué letra mandar
        string valorDeLaTecla = mayusculasActivas ? teclasMayus[indiceTecla] : teclasMin[indiceTecla];

        if (valorDeLaTecla == "MAYUS" || valorDeLaTecla == "minus")
        {
            AlternarMayusculas();
        }
        else if (valorDeLaTecla == "BORRAR")
        {
            if (campoActivo.text.Length > 0)
                campoActivo.text = campoActivo.text.Substring(0, campoActivo.text.Length - 1);
        }
        else if (valorDeLaTecla == "ESPACIO")
        {
            campoActivo.text += " ";
        }
        else if (valorDeLaTecla == "ENTER")
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            // Escribimos la letra o símbolo correspondiente
            campoActivo.text += valorDeLaTecla;
        }
    }

    void AlternarMayusculas()
    {
        // Volteamos la variable lógica (si era falso, se hace verdadero y viceversa)
        mayusculasActivas = !mayusculasActivas;

        // Recorremos todos los textos de la interfaz y los cambiamos de capa
        for (int i = 0; i < textosDeTeclas.Count; i++)
        {
            textosDeTeclas[i].text = mayusculasActivas ? teclasMayus[i] : teclasMin[i];
        }
    }
}