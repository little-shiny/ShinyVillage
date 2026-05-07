using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Componente UI que representa un botón para viajar a una isla específica.
/// Se instancia dinámicamente en el menú de selección de islas.
/// Muestra información básica de la isla y permite viajar a ella.
/// VERSIÓN CON DIAGNÓSTICO COMPLETO para detectar problemas de click.
/// </summary>
public class IslandButton_UI : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI islandNameText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image colorPreview;  // Muestra el tinte de la isla
    [SerializeField] private Button travelButton;
    
    // Datos internos
    private SaveSlotData _slotData;
    private IslandSelector_UI _selector;
    
    // Flag para diagnóstico
    private bool _setupCompleted = false;

    /// <summary>
    /// Se ejecuta cuando se crea el objeto.
    /// Intenta auto-asignar las referencias si no están configuradas en el Inspector.
    /// </summary>
    private void Awake()
    {
        Debug.Log($"[IslandButton_UI] ===== AWAKE en {gameObject.name} =====");
        
        // Auto-asignación defensiva: si el botón no está asignado en el Inspector,
        // se intenta buscar automáticamente en los hijos del GameObject
        if (travelButton == null)
        {
            Debug.LogWarning($"[IslandButton_UI] travelButton es NULL, buscando automáticamente...");
            
            // Se busca un componente Button que tenga "Travel" o "Viajar" en su nombre
            Button[] buttons = GetComponentsInChildren<Button>(true); // includeInactive = true
            Debug.Log($"[IslandButton_UI] Encontrados {buttons.Length} botones en los hijos");
            
            foreach (Button btn in buttons)
            {
                Debug.Log($"[IslandButton_UI] - Botón encontrado: {btn.gameObject.name}");
                
                if (btn.gameObject.name.Contains("Travel") || 
                    btn.gameObject.name.Contains("Viajar") ||
                    btn.gameObject.name.Contains("travel") ||
                    btn.gameObject.name.Contains("viajar"))
                {
                    travelButton = btn;
                    Debug.Log($"[IslandButton_UI] ✓ Botón de viaje AUTO-ASIGNADO: {btn.gameObject.name}");
                    break;
                }
            }
            
            // Si aún no se encuentra, se intenta asignar el primer botón
            if (travelButton == null && buttons.Length > 0)
            {
                travelButton = buttons[0];
                Debug.LogWarning($"[IslandButton_UI] Se asignó el primer botón encontrado: {buttons[0].gameObject.name}");
            }
            
            // Si aún es null, error crítico
            if (travelButton == null)
            {
                Debug.LogError($"[IslandButton_UI] ❌ NO se encontró ningún botón de viaje en {gameObject.name}. " +
                              "Por favor, asignar 'travelButton' en el Inspector.");
            }
        }
        else
        {
            Debug.Log($"[IslandButton_UI] ✓ travelButton ya estaba asignado: {travelButton.gameObject.name}");
        }
        
        // Verificar que el botón tiene el componente necesario
        if (travelButton != null)
        {
            // Verificar que el botón esté activo
            if (!travelButton.gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"[IslandButton_UI] El botón {travelButton.gameObject.name} está INACTIVO en la jerarquía");
            }
            
            // Verificar que el botón no esté deshabilitado
            if (!travelButton.interactable)
            {
                Debug.LogWarning($"[IslandButton_UI] El botón {travelButton.gameObject.name} tiene interactable = FALSE");
            }
            
            // Verificar que tenga Image (necesario para el raycast)
            Image btnImage = travelButton.GetComponent<Image>();
            if (btnImage == null)
            {
                Debug.LogWarning($"[IslandButton_UI] El botón {travelButton.gameObject.name} NO tiene componente Image");
            }
            else if (!btnImage.raycastTarget)
            {
                Debug.LogWarning($"[IslandButton_UI] El botón {travelButton.gameObject.name} tiene raycastTarget = FALSE");
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
        Debug.Log($"[IslandButton_UI] ===== SETUP INICIADO =====");
        Debug.Log($"[IslandButton_UI] Isla: {slotData?.SlotName ?? "NULL"}");
        
        _slotData = slotData;
        _selector = selector;
        
        // Se rellena la información visual
        if (islandNameText != null)
        {
            islandNameText.text = slotData.SlotName;
            Debug.Log($"[IslandButton_UI] Texto isla asignado: {slotData.SlotName}");
        }
        else
        {
            Debug.LogWarning("[IslandButton_UI] islandNameText es NULL");
        }
        
        if (playerNameText != null)
        {
            playerNameText.text = $"Jugador: {slotData.PlayerName}";
            Debug.Log($"[IslandButton_UI] Texto jugador asignado: {slotData.PlayerName}");
        }
        else
        {
            Debug.LogWarning("[IslandButton_UI] playerNameText es NULL");
        }
        
        // Se genera una vista previa del color de la isla
        if (colorPreview != null)
        {
            IslandConfig preview = IslandGenerator.GenerateIsland(
                slotData.IslandSeed, 
                slotData.SlotName
            );
            colorPreview.color = preview.accentColor;
            Debug.Log($"[IslandButton_UI] Color preview asignado: {preview.accentColor}");
        }
        
        // Se configura el botón de viaje
        if (travelButton != null)
        {
            Debug.Log($"[IslandButton_UI] Configurando listener en botón: {travelButton.gameObject.name}");
            
            // Se eliminan todos los listeners previos para evitar duplicados
            travelButton.onClick.RemoveAllListeners();
            Debug.Log($"[IslandButton_UI] Listeners previos eliminados");
            
            // Se añade el listener que ejecutará la acción de viajar
            travelButton.onClick.AddListener(OnTravelButtonClicked);
            Debug.Log($"[IslandButton_UI] ✓ Listener AÑADIDO para isla: {slotData.SlotName}");
            
            // Verificar que realmente se añadió
            int listenerCount = travelButton.onClick.GetPersistentEventCount();
            Debug.Log($"[IslandButton_UI] Listeners persistentes en el botón: {listenerCount}");
            
            _setupCompleted = true;
        }
        else
        {
            // Error crítico: no se puede configurar el botón
            Debug.LogError($"[IslandButton_UI] ❌ El botón de viaje es NULL en Setup() para {slotData.SlotName}. " +
                          "El botón NO funcionará.");
        }
        
        Debug.Log($"[IslandButton_UI] ===== SETUP COMPLETADO (success={_setupCompleted}) =====");
    }

    /// <summary>
    /// Callback cuando se pulsa el botón de viajar.
    /// Notifica al selector para que inicie el viaje.
    /// </summary>
    private void OnTravelButtonClicked()
    {
        Debug.Log($"[IslandButton] ===== CLICK DETECTADO ===== ");
        Debug.Log($"[IslandButton] Viajando a isla: {_slotData?.SlotName ?? "NULL"}");
        
        // Se valida que el selector exista antes de llamar
        if (_selector != null)
        {
            Debug.Log($"[IslandButton] Llamando a TravelToIsland()...");
            _selector.TravelToIsland(_slotData);
        }
        else
        {
            Debug.LogError("[IslandButton] ❌ El selector es NULL. No se puede viajar.");
        }
    }
    
    /// <summary>
    /// Implementación de IPointerClickHandler para detectar clicks directamente.
    /// Este método se ejecuta cuando el usuario hace click en CUALQUIER parte del GameObject.
    /// Es útil para diagnóstico si el botón no funciona.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[IslandButton] ===== POINTER CLICK DETECTADO (respaldo) =====");
        Debug.Log($"[IslandButton] Click en: {gameObject.name}");
        Debug.Log($"[IslandButton] Setup completado: {_setupCompleted}");
        
        // Si el setup se completó, se puede hacer click manual
        if (_setupCompleted && _slotData != null && _selector != null)
        {
            Debug.Log($"[IslandButton] Ejecutando viaje como RESPALDO...");
            OnTravelButtonClicked();
        }
        else
        {
            Debug.LogError($"[IslandButton] No se puede viajar: setup={_setupCompleted}, slotData={_slotData != null}, selector={_selector != null}");
        }
    }
    
    /// <summary>
    /// Método público para test manual desde el Inspector.
    /// Se puede llamar desde un botón configurado en el Inspector para probar.
    /// </summary>
    [ContextMenu("Test Click Manual")]
    public void TestClickManual()
    {
        Debug.Log("[IslandButton] ===== TEST MANUAL =====");
        OnTravelButtonClicked();
    }
}