using UnityEngine;
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
    
    // dtos internos
    private SaveSlotData _slotData;
    private IslandSelector_UI _selector;

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
            travelButton.onClick.RemoveAllListeners();
            travelButton.onClick.AddListener(OnTravelButtonClicked);
        }
    }

    /// <summary>
    /// Callback cuando se pulsa el botón de viajar.
    /// Notifica al selector para que inicie el viaje.
    /// </summary>
    private void OnTravelButtonClicked()
    {
        Debug.Log($"[IslandButton] Viajando a isla: {_slotData.SlotName}");
        _selector.TravelToIsland(_slotData);
    }
}