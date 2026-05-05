// ============================================================
// UIManager.cs
// Gestiona todos los paneles de UI de la escena de juego.
// Se encarga de inicializar referencias y manejar la apertura/cierre de paneles como inventario y settings.
//
// FUNCIÓN:
// - Singleton para acceso global desde cualquier parte del juego
// - Inicializa referencias de UI al cargar la escena
// - Provee métodos públicos para abrir/cerrar paneles
// - Maneja la persistencia de estado de UI entre transiciones de escena
// ============================================================

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestor centralizado de la interfaz de usuario en la escena de juego.
/// Maneja la inicialización y el estado de todos los paneles de UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════════════════
    // SINGLETON
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Instancia única del UIManager accesible globalmente.
    /// </summary>
    public static UIManager Instance { get; private set; }

    // ══════════════════════════════════════════════════════════════════════
    // REFERENCIAS DE UI
    // ══════════════════════════════════════════════════════════════════════
    
    [Header("Paneles principales")]
    [Tooltip("Panel de inventario del jugador")]
    [SerializeField] private GameObject inventoryPanel;
    
    [Tooltip("Panel de configuración/settings")]
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Botones de acceso rápido")]
    [Tooltip("Botón que abre/cierra el inventario")]
    [SerializeField] private Button inventoryButton;
    
    [Tooltip("Botón que abre/cierra el panel de settings")]
    [SerializeField] private Button settingsButton;
    
    [Header("Referencias de componentes")]
    [Tooltip("Componente Inventory_UI si existe en la escena")]
    [SerializeField] private Inventory_UI inventoryUI;

    // ══════════════════════════════════════════════════════════════════════
    // ESTADO INTERNO
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Indica si el panel de inventario está actualmente abierto.
    /// </summary>
    private bool _inventoryOpen = false;
    
    /// <summary>
    /// Indica si el panel de settings está actualmente abierto.
    /// </summary>
    private bool _settingsOpen = false;

    // ══════════════════════════════════════════════════════════════════════
    // CICLO DE VIDA DE UNITY
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Inicializa el singleton y valida las referencias del Inspector.
    /// Se ejecuta antes de Start().
    /// </summary>
    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[UIManager] Ya existe una instancia. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // No se usa DontDestroyOnLoad porque este manager es específico de SampleScene
        // y debe reinicializarse cada vez que se carga la escena
    }

    /// <summary>
    /// Inicializa el estado de los paneles y configura los botones.
    /// Se ejecuta al inicio de la escena.
    /// </summary>
    private void Start()
    {
        // Validar que las referencias necesarias estén asignadas
        ValidarReferencias();
        
        // Inicializar el estado de los paneles (cerrados por defecto)
        InicializarPaneles();
        
        // Configurar los listeners de los botones
        ConfigurarBotones();
        
        Debug.Log("[UIManager] Inicializado correctamente en SampleScene.");
    }

    /// <summary>
    /// Limpia la instancia del singleton al destruir el objeto.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // INICIALIZACIÓN PRIVADA
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Valida que las referencias del Inspector estén asignadas.
    /// Muestra advertencias en la consola si falta alguna referencia crítica.
    /// </summary>
    private void ValidarReferencias()
    {
        bool faltanReferencias = false;
        
        if (inventoryPanel == null)
        {
            Debug.LogError("[UIManager] Falta asignar 'inventoryPanel' en el Inspector.");
            faltanReferencias = true;
        }
        
        if (settingsPanel == null)
        {
            Debug.LogError("[UIManager] Falta asignar 'settingsPanel' en el Inspector.");
            faltanReferencias = true;
        }
        
        if (settingsButton == null)
        {
            Debug.LogWarning("[UIManager] 'settingsButton' no asignado. El botón de settings no funcionará.");
        }
        
        if (inventoryButton == null)
        {
            Debug.LogWarning("[UIManager] 'inventoryButton' no asignado. El botón de inventario puede no funcionar.");
        }
        
        // Si no está asignado Inventory_UI, intentar encontrarlo automáticamente
        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<Inventory_UI>();
            
            if (inventoryUI != null)
            {
                Debug.Log("[UIManager] Inventory_UI encontrado automáticamente.");
            }
            else
            {
                Debug.LogWarning("[UIManager] No se encontró Inventory_UI en la escena. " +
                               "Algunas funciones de inventario pueden no estar disponibles.");
            }
        }
        
        if (faltanReferencias)
        {
            Debug.LogError("[UIManager] Hay referencias críticas sin asignar. " +
                          "Por favor, revisa el Inspector del GameObject UIManager.");
        }
    }

    /// <summary>
    /// Inicializa el estado de todos los paneles (cerrados por defecto).
    /// </summary>
    private void InicializarPaneles()
    {
        // Asegurar que los paneles comienzan cerrados
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            _inventoryOpen = false;
        }
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            _settingsOpen = false;
        }
        
        Debug.Log("[UIManager] Paneles inicializados: todos cerrados.");
    }

    /// <summary>
    /// Configura los listeners (eventos OnClick) de los botones de UI.
    /// </summary>
    private void ConfigurarBotones()
    {
        // Configurar el botón de inventario
        if (inventoryButton != null)
        {
            // Remover listeners anteriores para evitar duplicados
            inventoryButton.onClick.RemoveAllListeners();
            
            // Añadir el listener que llama a ToggleInventory
            inventoryButton.onClick.AddListener(ToggleInventory);
            
            Debug.Log("[UIManager] Botón de inventario configurado.");
        }
        
        // Configurar el botón de settings
        if (settingsButton != null)
        {
            // Remover listeners anteriores para evitar duplicados
            settingsButton.onClick.RemoveAllListeners();
            
            // Añadir el listener que llama a ToggleSettings
            settingsButton.onClick.AddListener(ToggleSettings);
            
            Debug.Log("[UIManager] Botón de settings configurado.");
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // API PÚBLICA - Gestión de Inventario
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Alterna el estado del panel de inventario (abre si está cerrado, cierra si está abierto).
    /// Este método es llamado por el botón de inventario o por código externo.
    /// </summary>
    public void ToggleInventory()
    {
        // Si existe el componente Inventory_UI, delegar la lógica a él
        if (inventoryUI != null)
        {
            inventoryUI.ToggleInventory();
            _inventoryOpen = !_inventoryOpen;
            
            Debug.Log($"[UIManager] Inventario {(_inventoryOpen ? "abierto" : "cerrado")} " +
                     "mediante Inventory_UI.");
        }
        // Si no existe Inventory_UI, manejar el panel directamente
        else if (inventoryPanel != null)
        {
            _inventoryOpen = !_inventoryOpen;
            inventoryPanel.SetActive(_inventoryOpen);
            
            Debug.Log($"[UIManager] Inventario {(_inventoryOpen ? "abierto" : "cerrado")} " +
                     "directamente.");
        }
        else
        {
            Debug.LogWarning("[UIManager] No se puede abrir el inventario: " +
                           "ni Inventory_UI ni inventoryPanel están disponibles.");
        }
    }

    /// <summary>
    /// Abre el panel de inventario forzosamente.
    /// </summary>
    public void AbrirInventario()
    {
        if (!_inventoryOpen)
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Cierra el panel de inventario forzosamente.
    /// </summary>
    public void CerrarInventario()
    {
        if (_inventoryOpen)
        {
            ToggleInventory();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // API PÚBLICA - Gestión de Settings
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Alterna el estado del panel de settings (abre si está cerrado, cierra si está abierto).
    /// Este método es llamado por el botón de settings.
    /// </summary>
    public void ToggleSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[UIManager] No se puede abrir settings: settingsPanel no asignado.");
            return;
        }

        _settingsOpen = !settingsPanel.activeSelf;
        settingsPanel.SetActive(_settingsOpen);

        Debug.Log($"[UIManager] Settings {(_settingsOpen ? "abierto" : "cerrado")}.");
    }

    /// <summary>
    /// Abre el panel de settings forzosamente.
    /// </summary>
    public void AbrirSettings()
    {
        if (!_settingsOpen)
        {
            ToggleSettings();
        }
    }

    /// <summary>
    /// Cierra el panel de settings forzosamente.
    /// </summary>
    public void CerrarSettings()
    {
        if (_settingsOpen)
        {
            ToggleSettings();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // API PÚBLICA - Gestión General
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Cierra todos los paneles abiertos.
    /// Útil para resetear el estado de la UI o al cambiar de escena.
    /// </summary>
    public void CerrarTodosPaneles()
    {
        CerrarInventario();
        CerrarSettings();
        
        Debug.Log("[UIManager] Todos los paneles cerrados.");
    }

    /// <summary>
    /// Devuelve true si algún panel está actualmente abierto.
    /// Útil para controlar la interacción del jugador (ej: desactivar movimiento si hay UI abierta).
    /// </summary>
    /// <returns>True si hay algún panel abierto, false en caso contrario.</returns>
    public bool HayPanelAbierto()
    {
        return _inventoryOpen || _settingsOpen;
    }
}