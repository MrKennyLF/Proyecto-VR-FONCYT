using UnityEngine;

public class GestorEPP : MonoBehaviour
{
    [Header("Estado del EPP ??")]
    public bool tieneCasco = false;
    public bool tieneLentes = false;
    public bool tieneChaleco = false;
    public bool tieneAudifonos = false;

    // Esta es la función clave que la máquina le preguntará a este script
    public bool TieneTodoElEPP()
    {
        return tieneCasco && tieneLentes && tieneChaleco && tieneAudifonos;
    }

    // --- FUNCIONES PARA CONECTAR A TUS OBJETOS VR ---
    // Cuando el jugador se ponga el objeto (ej. mediante un Socket Interactor), llamas a estas:

    public void EquiparCasco() { tieneCasco = true; Debug.Log("?? Casco equipado."); }
    public void QuitarCasco() { tieneCasco = false; }

    public void EquiparLentes() { tieneLentes = true; Debug.Log("?? Lentes equipados."); }
    public void QuitarLentes() { tieneLentes = false; }

    public void EquiparChaleco() { tieneChaleco = true; Debug.Log("?? Chaleco equipado."); }
    public void QuitarChaleco() { tieneChaleco = false; }

    public void EquiparAudifonos() { tieneAudifonos = true; Debug.Log("?? Audífonos equipados."); }
    public void QuitarAudifonos() { tieneAudifonos = false; }
}