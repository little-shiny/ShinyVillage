// ============================================================
// IslandManager.cs
// Gestiona las islas (partidas guardadas) y su configuración visual.
// - Genera y aplica paletas de colores únicas para cada isla/partida
// - Actua como puente entre el sistema de guardado y la vista visual
// - Modifica los Tilemaps para reflejar la identidad visual de cada isla
//

// Este manager se reinicia cada vez que se carga SampleScene.
// NO usa DontDestroyOnLoad 
// ============================================================

using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Gestiona las islas (partidas guardadas) y aplica su configuración visual.
/// Cada isla tiene una paleta de colores única que se aplica al Tilemap.
/// Este manager actúa como puente entre el sistema de guardado y la vista de la isla.
/// </summary>
public class IslandManager : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════════════════
    // SINGLETON
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Instancia única del IslandManager accesible globalmente dentro de SampleScene.
    /// Se reinicializa en cada carga de la escena.
    /// </summary>
    public static IslandManager Instance { get; private set; }

    // ══════════════════════════════════════════════════════════════════════
    // REFERENCIAS DEL INSPECTOR
    // ══════════════════════════════════════════════════════════════════════
    
    [Header("Referencias")]
    [Tooltip("Tilemap de fondo que representa el terreno de la isla")]
    [SerializeField] private Tilemap backgroundTilemap;
    
    [Tooltip("Tilemap de elementos interactuables (de cara a futuro)")]
    [SerializeField] private Tilemap interactableTilemap;
    
    [Header("Estado Actual")]
    [Tooltip("Configuración de la isla actualmente cargada")]
    [SerializeField] private IslandConfig _currentIsland;

    // ══════════════════════════════════════════════════════════════════════
    // CICLO DE VIDA DE UNITY
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Inicializa el singleton del IslandManager.
    /// Se ejecuta al cargar SampleScene, antes de Start().
    /// </summary>
    private void Awake()
    {
        // ─────────────────────────────────────────────────────────────────
        // Implementación del patrón Singleton
        // ─────────────────────────────────────────────────────────────────
        
        // Si ya existe otra instancia de IslandManager en la escena,
        // destruir este GameObject para mantener solo una instancia
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[IslandManager] Ya existe una instancia de IslandManager. " +
                           "Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
        
        // Establecer esta instancia como la instancia activa
        Instance = this;

    }

    /// <summary>
    /// Valida las referencias asignadas en el Inspector.
    /// Se ejecuta después de Awake().
    /// </summary>
    private void Start()
    {
        ValidateReferences();

        // Al cargar SampleScene, si hay una partida activa cargamos su isla.
        // SaveGameManager persiste entre escenas, así que CurrentSlotId ya está asignado.
        if (SaveGameManager.Instance != null && SaveGameManager.Instance.CurrentSlotId != -1)
        {
            var allSlots = SaveGameManager.Instance.GetAllSaveSlots();
            SaveSlotData current = allSlots.Find(s => s.Id == SaveGameManager.Instance.CurrentSlotId);
            if (current != null)
                LoadIsland(current);
        }
    }
    
    /// <summary>
    /// Limpia la referencia del singleton cuando el GameObject es destruido.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[IslandManager] Instancia destruida y referencia singleton limpiada.");
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // VALIDACIÓN PRIVADA
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Valida que las referencias necesarias estén asignadas en el Inspector.
    /// Muestra advertencias si falta alguna referencia.
    /// </summary>
    private void ValidateReferences()
    {
        if (backgroundTilemap == null)
        {
            Debug.LogError("[IslandManager] Falta asignar 'backgroundTilemap' en el Inspector. " +
                          "Los efectos visuales de la isla no se aplicarán correctamente.");
        }
        
        if (interactableTilemap == null)
        {
            Debug.LogWarning("[IslandManager] 'interactableTilemap' no asignado. " +
                           "Los efectos solo se aplicarán al background tilemap.");
        }
        
        Debug.Log($"[IslandManager] Referencias validadas:\n" +
                  $"  - Background Tilemap: {(backgroundTilemap != null ? "✓" : "✗")}\n" +
                  $"  - Interactable Tilemap: {(interactableTilemap != null ? "✓" : "⚠")}");
    }

    // ══════════════════════════════════════════════════════════════════════
    // API PÚBLICA - Gestión de Islas
    // ══════════════════════════════════════════════════════════════════════
 
    /// <summary>
    /// Carga una isla basándose en los datos de un slot de guardado.
    /// Genera la configuración visual usando la semilla (seed) y la aplica inmediatamente.
    /// </summary>
    /// <param name="slotData">Datos del slot desde la base de datos</param>
    public void LoadIsland(SaveSlotData slotData)
    {
        // Validar que se recibieron datos válidos
        if (slotData == null)
        {
            Debug.LogError("[IslandManager] Se intentó cargar una isla con datos NULL. " +
                          "No se aplicarán visuales.");
            return;
        }
        
        Debug.Log($"[IslandManager] Cargando isla: '{slotData.SlotName}' (Seed: {slotData.IslandSeed})");
        
        // Generar la configuración de la isla usando la semilla guardada
        // Esto garantiza que siempre se generen los mismos colores para esta partida
        _currentIsland = IslandGenerator.GenerateIsland(slotData.IslandSeed, slotData.SlotName);
        
        // Aplicar la paleta de colores a los Tilemaps
        ApplyIslandVisuals();

        Debug.Log($"[IslandManager] Isla '{slotData.SlotName}' cargada y visuales aplicados correctamente.");
    }

    /// <summary>
    /// Obtiene la configuración de la isla actualmente cargada.
    /// Otros scripts pueden usar esto para acceder a los colores de la isla.
    /// </summary>
    /// <returns>Configuración actual de la isla, o null si no hay isla cargada</returns>
    public IslandConfig GetCurrentIsland()
    {
        return _currentIsland;
    }

    /// <summary>
    /// Resetea la isla a colores por defecto (blanco neutral).
    /// Útil al volver al menú principal o antes de cargar una nueva partida.
    /// </summary>
    public void ResetIslandVisuals()
    {
        // Resetear color del tilemap de fondo
        if (backgroundTilemap != null)
        {
            backgroundTilemap.color = Color.white;
        }
        
        // Resetear color del tilemap interactuable
        if (interactableTilemap != null)
        {
            interactableTilemap.color = Color.white;
        }
        
        // Limpiar la configuración actual
        _currentIsland = null;
        
        Debug.Log("[IslandManager] Visuales de isla reseteados a blanco neutral.");
    }

    // ══════════════════════════════════════════════════════════════════════
    // MÉTODOS PRIVADOS - Aplicación de Visuales
    // ══════════════════════════════════════════════════════════════════════
 
    /// <summary>
    /// Aplica la configuración visual de la isla actual a los Tilemaps.
    /// Modifica el color (tint) de los tilemaps según la paleta de la isla.
    /// </summary>
    private void ApplyIslandVisuals()
    {
        // Verificar que hay una configuración de isla válida
        if (_currentIsland == null)
        {
            Debug.LogWarning("[IslandManager] No hay configuración de isla para aplicar. " +
                           "Llama a LoadIsland() primero.");
            return;
        }
        
        // Aplicar el tinte global combinado con el color de terreno al tilemap de fondo
        if (backgroundTilemap != null)
        {
            // Se combina el groundColor con el globalTint para obtener el color final
            Color finalGroundColor = _currentIsland.groundColor * _currentIsland.globalTint;
            backgroundTilemap.color = finalGroundColor;
            
            Debug.Log($"[IslandManager] Color de terreno aplicado al background: " +
                     $"Ground={_currentIsland.groundColor}, " +
                     $"Global Tint={_currentIsland.globalTint}, " +
                     $"Final={finalGroundColor}");
        }
        
        // Aplicar el color de agua al tilemap interactuable (si existe)
        if (interactableTilemap != null)
        {
            // Se combina el waterColor con el globalTint para obtener el color final
            Color finalWaterColor = _currentIsland.waterColor * _currentIsland.globalTint;
            interactableTilemap.color = finalWaterColor;
            
            Debug.Log($"[IslandManager] Color de agua aplicado al interactable: " +
                     $"Water={_currentIsland.waterColor}, " +
                     $"Global Tint={_currentIsland.globalTint}, " +
                     $"Final={finalWaterColor}");
        }
        
    }
}