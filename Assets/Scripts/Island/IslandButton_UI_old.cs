/*using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente UI que representa un botón para viajar a una isla específica.
/// Se instancia dinámicamente en el menú de selección de islas.
/// Muestra información básica de la isla y permite viajar a ella.
/// </summary>
public class IslandButton_UI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI islandNameText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image colorPreview;  // Muestra el tinte de la isla
    [SerializeField] private Button travelButton;

    // Datos internos
    private SaveSlotData _slotData;
    private IslandSelector_UI _selector;

    /// <summary>
    /// Se ejecuta cuando se crea el objeto.
    /// Intenta auto-asignar las referencias si no están configuradas en el Inspector.
    /// </summary>
    private void Awake()
    {
        //  si el botón no está asignado en el Inspector,
        // se intenta buscar automáticamente en los hijos del GameObject
        if (travelButton == null)
        {
            // Se busca un componente Button que tenga "TravelButton" en su nombre
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name.Contains("Travel") ||
                    btn.gameObject.name.Contains("Viajar"))
                {
                    travelButton = btn;
                    Debug.Log($"[IslandButton_UI] Botón de viaje auto-asignado: {btn.gameObject.name}");
                    break;
                }
            }

            // Si aún no se encuentra, se advierte 
            if (travelButton == null)
            {
                Debug.LogError($"[IslandButton_UI] NO se encontró el botón de viaje en {gameObject.name}. " +
                              "Por favor, asignar 'travelButton' en el Inspector.");
            }
        }
    }

    /// <summary>
    /// Configura este botón con los datos de una isla.
    /// </summary>
    /// <param name="slotData">Datos del slot de guardado</param>
    /// <param name="selector">Referencia al selector padre para callbacks</param>
    public void Setup(SaveSlotData slotData, IslandSelector_UI selector)
    {
        _slotData = slotData;
        _selector = selector;

        // Se rellena la información visual
        if (islandNameText != null)
            islandNameText.text = slotData.SlotName;

        if (playerNameText != null)
            playerNameText.text = $"Jugador: {slotData.PlayerName}";

        // Se genera una vista previa del color de la isla
        if (colorPreview != null)
        {
            IslandConfig preview = IslandGenerator.GenerateIsland(
                slotData.IslandSeed,
                slotData.SlotName
            );
            colorPreview.color = preview.globalTint;
        }

        // Se configura el botón de viaje
        if (travelButton != null)
        {
            // Se eliminan todos los listeners previos para evitar duplicados
            travelButton.onClick.RemoveAllListeners();

            // Se añade el listener que ejecutará la acción de viajar
            travelButton.onClick.AddListener(OnTravelButtonClicked);

            Debug.Log($"[IslandButton_UI] Listener configurado para isla: {slotData.SlotName}");
        }
        else
        {
            // Error crítico: no se puede configurar el botón
            Debug.LogError($"[IslandButton_UI] El botón de viaje es NULL en Setup() para {slotData.SlotName}. " +
                          "El botón NO funcionará.");
        }
    }

    /// <summary>
    /// Callback cuando se pulsa el botón de viajar.
    /// Notifica al selector para que inicie el viaje.
    /// </summary>
    private void OnTravelButtonClicked()
    {
        Debug.Log($"[IslandButton] Viajando a isla: {_slotData.SlotName}");

        // Se valida que el selector exista antes de llamar
        if (_selector != null)
        {
            _selector.TravelToIsland(_slotData);
        }
        else
        {
            Debug.LogError("[IslandButton] El selector es NULL. No se puede viajar.");
        }
    }
}
*/