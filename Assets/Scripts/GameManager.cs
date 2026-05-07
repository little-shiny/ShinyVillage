// ============================================================
// GameManager.cs
// Gestor central de la escena de juego (SampleScene).
//
// Mantiene referencias a los managers específicos del juego:
// - ItemManager: catálogo de items disponibles
// - TileManager: gestión de tiles interactuables
//
// Este manager se reinicializa cada vez que se carga SampleScene,
// asegurando que todas las referencias estén actualizadas.
// NO persiste entre escenas (no usa DontDestroyOnLoad).
//
// - Se eliminó DontDestroyOnLoad para evitar persistencia innecesaria
// - Se mejoró la validación de referencias con mensajes de error claros
// - Se añadió OnDestroy para limpiar la referencia del singleton
// - Se mejoraron los logs para facilitar debugging
//
// ============================================================

using UnityEngine;

/// <summary>
/// Gestor central de la escena de juego (SampleScene).
/// Actúa como punto de acceso único a los managers de juego mediante
/// el patrón Singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════════════════
    // SINGLETON
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Instancia única del GameManager accesible globalmente dentro de SampleScene.
    /// Se reinicializa en cada carga de la escena.
    /// </summary>
    public static GameManager instance;
    
    // ══════════════════════════════════════════════════════════════════════
    // REFERENCIAS A MANAGERS DE JUEGO
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Manager que gestiona el catálogo de items del juego.
    /// Componente ubicado en el mismo GameObject que GameManager.
    /// </summary>
    public ItemManager itemManager;
    
    /// <summary>
    /// Manager que gestiona los tiles interactuables del mapa.
    /// Componente ubicado en el mismo GameObject que GameManager.
    /// </summary>
    public TileManager tileManager;

    // ══════════════════════════════════════════════════════════════════════
    // CICLO DE VIDA DE UNITY
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Inicializa el singleton y obtiene las referencias a los managers.
    /// Se ejecuta al cargar SampleScene, antes de Start().
    /// </summary>
    private void Awake()
    {
        // ─────────────────────────────────────────────────────────────────
        // Implementación del patrón Singleton
        // ─────────────────────────────────────────────────────────────────
        
        // Si ya existe otra instancia de GameManager en la escena,
        // destruir este GameObject para mantener solo una instancia
        if (instance != null && instance != this) 
        {
            Debug.LogWarning("[GameManager] Ya existe una instancia de GameManager en la escena. " +
                           "Destruyendo el duplicado para mantener el patrón Singleton.");
            Destroy(this.gameObject);
            return;
        }
        
        // Establecer esta instancia como la instancia activa
        instance = this;
        
        
        // ─────────────────────────────────────────────────────────────────
        // Obtención de referencias a los managers
        // ─────────────────────────────────────────────────────────────────
        
        // Intentar obtener ItemManager del mismo GameObject
        itemManager = GetComponent<ItemManager>();
        
        // Intentar obtener TileManager del mismo GameObject
        tileManager = GetComponent<TileManager>();
        
        // ─────────────────────────────────────────────────────────────────
        // Validación de referencias
        // ─────────────────────────────────────────────────────────────────
        
        // Validar que ItemManager se encontró correctamente
        if (itemManager == null)
        {
            Debug.LogError("[GameManager] No se encontró el componente ItemManager en este GameObject. " +
                          "Asegúrate de que ItemManager está añadido al mismo GameObject que GameManager.");
        }
        
        // Validar que TileManager se encontró correctamente
        if (tileManager == null)
        {
            Debug.LogError("[GameManager] No se encontró el componente TileManager en este GameObject. " +
                          "Asegúrate de que TileManager está añadido al mismo GameObject que GameManager.");
        }
        
        // ─────────────────────────────────────────────────────────────────
        // Log de inicialización 
        // ─────────────────────────────────────────────────────────────────
        
        Debug.Log($"[GameManager] Inicializado correctamente en SampleScene.\n" +
                  $"  - ItemManager: {(itemManager != null ? "✓ Encontrado" : "✗ NO encontrado")}\n" +
                  $"  - TileManager: {(tileManager != null ? "✓ Encontrado" : "✗ NO encontrado")}");
    }
    
    /// <summary>
    /// Limpia la referencia del singleton cuando el GameObject es destruido.
    /// Esto ocurre al salir de SampleScene o al recargar la escena.
    /// </summary>
    private void OnDestroy()
    {
        // Solo limpiar la referencia si esta instancia es la activa
        if (instance == this)
        {
            instance = null;
            Debug.Log("[GameManager] Instancia destruida y referencia singleton limpiada.");
        }
    }
    
    // ══════════════════════════════════════════════════════════════════════
    // API PÚBLICA (para uso futuro)
    // ══════════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Verifica si todas las referencias de managers están correctamente asignadas.
    /// Útil para debugging o validación en tiempo de ejecución.
    /// </summary>
    /// <returns>True si todas las referencias son válidas, false en caso contrario.</returns>
    public bool ValidarReferencias()
    {
        bool todasValidas = true;
        
        if (itemManager == null)
        {
            Debug.LogError("[GameManager] ItemManager es null.");
            todasValidas = false;
        }
        
        if (tileManager == null)
        {
            Debug.LogError("[GameManager] TileManager es null.");
            todasValidas = false;
        }
        
        if (todasValidas)
        {
            Debug.Log("[GameManager] Todas las referencias son válidas.");
        }
        
        return todasValidas;
    }

    public void CerrarJuego()
    {
        Debug.Log("[Menu] Cerrando juego...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}