using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Gestiona las islas (partidas guardadas) y aplica su configuración visual.
/// Cada isla tiene una paleta de colores única que se aplica al Tilemap.
/// Este manager actúa como puente entre el sistema de guardado y la vista de la isla 
/// </summary>
public class IslandManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Tilemap de fondo que representa el terreno de la isla")]
    [SerializeField] private Tilemap backgroundTilemap;
    
    [Tooltip("Tilemap de elementos interactuables (de cara a futuro)")]
    [SerializeField] private Tilemap interactableTilemap;
    
    [Header("Estado Actual")]
    [Tooltip("Configuración de la isla actualmente cargada")]
    private IslandConfig _currentIsland;
    
    /// <summary>
    /// Singleton para acceso global desde otros scripts.
    /// </summary>
    public static IslandManager Instance { get; private set; }

    private void Awake()
    {
        // Patrón Singleton estándar
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
    }

    /// <summary>
    /// Aplica la configuración visual de la isla actual a los Tilemaps.
    /// Cambia los colores de los tiles para reflejar la palet de la isla.
    /// </summary>
    private void ApplyIslandVisuals()
    {
        if (_currentIsland == null)
        {
            Debug.LogWarning("[IslandManager] No hay isla cargada para aplicar visuales");
            return;
        }
        
        // Se aplica el tinte global al tilemap de fondo
        // Esto cambia el color de todos los tiles de golpe
        if (backgroundTilemap != null)
        {
            backgroundTilemap.color = _currentIsland.globalTint;
            Debug.Log($"[IslandManager] Tinte aplicado al fondo: {_currentIsland.globalTint}");
        }
        
        // También se puede aplicar al tilemap interactuable si existe (futuro)
        if (interactableTilemap != null)
        {
            // Se usa un tinte más sutil para elementos interactuables
            Color subtleTint = Color.Lerp(Color.white, _currentIsland.globalTint, 0.5f);
            interactableTilemap.color = subtleTint;
        }
        
        Debug.Log($"[IslandManager] Visuales de isla aplicados: {_currentIsland.islandName}");
    }

    /// <summary>
    /// Obtiene la configuración de la isla actualmente cargada.
    /// Otros scripts pueden usar esto para acceder a los colores de la isla.(futuro)
    /// </summary>
    public IslandConfig GetCurrentIsland()
    {
        return _currentIsland;
    }

    /// <summary>
    /// Resetea la isla a colores por defecto (blanco neutral).
    /// Útil al volver al menú principal para que no haya overlap de colores en los diferentes slots sin cerrar el programa
    /// </summary>
    public void ResetIslandVisuals()
    {
        if (backgroundTilemap != null)
            backgroundTilemap.color = Color.white;
        
        if (interactableTilemap != null)
            interactableTilemap.color = Color.white;
        
        _currentIsland = null;
        
        Debug.Log("[IslandManager] Visuales de isla reseteados");
    }
}