using UnityEngine;

/// Script genérico para mostrar/ocultar paneles UI.
/// Se puede usar para inventarios, menús de pausa, selectores, etc.

public class PanelController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Si está marcado, el panel empieza desactivado")]
    [SerializeField] private bool startHidden = true;

    private void Start()
    {
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }


    /// Abre el panel (lo activa).

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        Debug.Log($"[PanelController] Panel '{gameObject.name}' abierto");
    }
    /// Cierra el panel (lo desactiva).

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log($"[PanelController] Panel '{gameObject.name}' cerrado");
    }

    /// Alterna el estado del panel (abre si está cerrado, cierra si está abierto).

    public void TogglePanel()
    {
        bool newState = !gameObject.activeSelf;
        gameObject.SetActive(newState);
        
        string estado = newState ? "abierto" : "cerrado";
        Debug.Log($"[PanelController] Panel '{gameObject.name}' {estado}");
    }
}