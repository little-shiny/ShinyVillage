using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Gestiona las islas (partidas guardadas) y aplica su configuración visual.
/// Cada isla tiene una paleta de colores única que se aplica al Tilemap.
/// Este manager actúa como puente entre el sistema de guardado y la vista de la isla 
/// 
/// Este manager se añade al mismo GameObject que los otros managers (GameManager)
/// para mantener la arquitectura del proyecto consistente.
/// </summary>
public class IslandManager : MonoBehaviour
{
    // Instanciación de SINGLETON para poder acceder desde otros scripts 
    public static IslandManager Instance { get; private set; }

    // Referencias para el inspector
    [Header("Referencias")]
    [Tooltip("Tilemap de fondo que representa el terreno de la isla")]
    [SerializeField] private Tilemap backgroundTilemap;
    
    [Tooltip("Tilemap de elementos interactuables (de cara a futuro)")]
    [SerializeField] private Tilemap interactableTilemap;
    
    [Header("Estado Actual")]
    [Tooltip("Configuración de la isla actualmente cargada")]


    // Estado interno 
    private IslandConfig _currentIsland;


    private void Awake()
    {
        // Patrón Singleton estándar
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        // NOTA: No se usa DontDestroyOnLoad porque está en el GameManager que ya maneja la persistencia entre escenas
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Validación de referencias del inspector
        ValidateReferences();
    }

    /// <summary>
    /// Valida que las referencias necesarias estén asignadas en el Inspector.
    /// Muestra advertencias si falta alguna (debug)
    /// </summary>
    private void ValidateReferences()
    {
        if (backgroundTilemap == null)
        {
            Debug.LogError("[IslandManager] falta asignar: Background tilemap en el inspector. ");
        }
        
        if (interactableTilemap == null)
        {
            Debug.LogWarning("[IslandManager] Interactable tilemap no asignado. " +
                           "se aplicarán efectos solo al background.");
        }
    }

     // ════════════════════════════════════════════════════════════════
    // API PÚBLICA - Gestión de Islas
    // ════════════════════════════════════════════════════════════════
 
    /// <summary>
    /// Carga una isla basándose en los datos de un slot de guardado.
    /// Genera la configuración visual y la aplica inmediatamente.
    /// </summary>
    /// <param name="slotData">Datos del slot desde la base de datos</param>
    public void LoadIsland(SaveSlotData slotData)
    {
        if (slotData == null)
        {
            Debug.LogError("[IslandManager] Se intentó cargar una isla con datos NULOS"); //verificacion para no causar error si la seed no existe
            return;
        }
        
        Debug.Log($"[IslandManager] Cargando isla: {slotData.SlotName} (Seed: {slotData.IslandSeed})");
        
        // Se genera la configuración de la isla usando la semilla guardada
        // Esto garantiza que siempre se generen los mismos colores para esta partida
        _currentIsland = IslandGenerator.GenerateIsland(slotData.IslandSeed, slotData.SlotName);
        
        // Se aplica la paleta de colores al Tilemap
        ApplyIslandVisuals();

        Debug.Log($"Isla '{slotData.SlotName}' cargada exitosamente");
    }

    // <summary>
    /// Obtiene la configuración de la isla actualmente cargada.
    /// Otros scripts pueden usar esto para acceder a los colores de la isla.
    /// </summary>
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
        if (backgroundTilemap != null)
            backgroundTilemap.color = Color.white;
        
        if (interactableTilemap != null)
            interactableTilemap.color = Color.white;
        
        _currentIsland = null;
        
        Debug.Log("Visuales de isla reseteados a blanco neutral");
    }


        // ════════════════════════════════════════════════════════════════
    // MÉTODOS PRIVADOS - Aplicación de Visuales
    // ════════════════════════════════════════════════════════════════
 
    /// <summary>
    /// Aplica la configuración visual de la isla actual a los Tilemaps.
    /// Cambia el color de todos los tiles de golpe usando la propiedad Tilemap.color
    /// 
    /// - No modifica los tiles individualmente (sería muy costoso)
    /// - Usa el tinte global del Tilemap 
    /// - Es instantáneo y no afecta al rendimiento
    /// </summary>
    private void ApplyIslandVisuals()
    {
        if (_currentIsland == null)
        {
            Debug.LogWarning("[IslandManager] No hay isla cargada para aplicar visuales");
            return;
        }
        
        // Se aplica el tinte global al tilemap de fondo
        // Esto multiplica el color de los tiles por este tinte
        if (backgroundTilemap != null)
        {
            backgroundTilemap.color = _currentIsland.globalTint;
            Debug.Log($"Tinte aplicado al Background: RGB({_currentIsland.globalTint.r:F2}, " +
                    $"{_currentIsland.globalTint.g:F2}, {_currentIsland.globalTint.b:F2})");
        }
        
        // También se puede aplicar al tilemap interactuable si existe
        // Se usa un tinte más sutil para que los elementos interactuables no cambien tanto
        if (interactableTilemap != null)
        {
            // Lerp = Linear Interpolation (interpolación lineal)
            // Mezcla 50% blanco con 50% del tinte de la isla
            Color subtleTint = Color.Lerp(Color.white, _currentIsland.globalTint, 0.5f);
            interactableTilemap.color = subtleTint;
            Debug.Log($"Tinte sutil aplicado al Interactable");
        }
    }
 
    // ════════════════════════════════════════════════════════════════
    // MÉTODOS DE EDITOR debug)
    // ════════════════════════════════════════════════════════════════
 
#if UNITY_EDITOR

    /// Método de ayuda para encontrar automáticamente los Tilemaps en la escena.
    /// Solo disponible en el Editor de Unity.

    [ContextMenu("Auto Find Tilemaps")]
    private void AutoFindTilemaps()
    {
        // Busca todos los Tilemaps en la escena
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        
        foreach (Tilemap tm in allTilemaps)
        {
            // Busca por nombre del GameObject
            if (tm.gameObject.name.ToLower().Contains("background"))
            {
                backgroundTilemap = tm;
                Debug.Log($"[IslandManager] Background Tilemap encontrado: {tm.gameObject.name}");
            }
            else if (tm.gameObject.name.ToLower().Contains("interactable"))
            {
                interactableTilemap = tm;
                Debug.Log($"[IslandManager] Interactable Tilemap encontrado: {tm.gameObject.name}");
            }
        }
        
        // Validar resultados
        if (backgroundTilemap == null)
        {
            Debug.LogWarning("[IslandManager] No se encontró ningún Tilemap con 'Background' en el nombre.");
        }
    }
#endif
}