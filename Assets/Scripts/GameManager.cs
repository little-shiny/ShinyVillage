using UnityEngine;
// GameManager.cs - VERSIÓN 07/04

/// <summary>
/// Gestor central de la escena de juego (SampleScene).
/// Mantiene referencias a los managers específicos de juego: ItemManager, TileManager e IslandManager.
/// 
/// Este manager se reinicia cada vez que se carga SampleScene para que  que todas las referencias estén actalizadas y no haya duplicados
/// NO persiste entre escenas (no usa DontDestroyOnLoad).
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton para acceso global dentro de SampleScene
    public static GameManager instance;
    
    // Referencias a los managers de juego
    public ItemManager itemManager;
    public TileManager tileManager;

    private void Awake()
    {
        // Patrón Singleton que esta solo en la escena actual
        // Si ya existe una instancia, se destruye este objeto
        if (instance != null && instance != this) 
        {
            Debug.LogWarning("[GameManager] Destruyendo instancia duplicada.");
            Destroy(this.gameObject);
            return;
        }
        
        instance = this;
        
        // GameManager debe reinicializarse en cada carga de SampleScene
        
        // Obtener referencias a los componentes en el mismo GameObject
        itemManager = GetComponent<ItemManager>();
        tileManager = GetComponent<TileManager>();
        
        // Validar que las referencias se obtuvieron correctamente
        if (itemManager == null)
        {
            Debug.LogError("[GameManager] No se encontró ItemManager en este GameObject.");
        }
        
        if (tileManager == null)
        {
            Debug.LogError("[GameManager] No se encontró TileManager en este GameObject.");
        }
        
        Debug.Log($"[GameManager] Inicializado en SampleScene. " +
                  $"ItemManager: {(itemManager != null ? "✓" : "✗")}, " +
                  $"TileManager: {(tileManager != null ? "✓" : "✗")}");
    }
    
    private void OnDestroy()
    {
        // Limpiar la referencia del singleton al destruir el GameObject
        if (instance == this)
        {
            instance = null;
            Debug.Log("[GameManager] Instancia destruida y limpiada.");
        }
    }
}