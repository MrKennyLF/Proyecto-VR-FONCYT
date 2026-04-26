using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEPP", menuName = "VR-FONCYT/Item de Seguridad")]
public class DatosEPP : ScriptableObject
{
    [Header("Identificación")]
    public string nombreEquipo;
    public enum Categoria { Cabeza, Manos, Ojos, Cuerpo }
    public Categoria tipoDeEquipo;

    [Header("Reglas de Evaluación")]
    public bool esCorrecto; // True = Guante de Nitrilo. False = Guante de Tela.
    public bool esObligatorioParaOperar;
    public int puntosAsignados;

    [Header("Retroalimentación")]
    [TextArea] public string mensajeAlerta; // Ej: "¡Peligro! Material inadecuado para altas temperaturas."
}