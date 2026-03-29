using UnityEngine;

/// Punto de entrada único para guardar/cargar partidas.
/// Los scripts del juego solo hablan con esta clase, no con los repositorios directamente.
/// Esto hace que sea fácil cambiar la implementación sin tocar el resto del código.

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    // Repositorios
    private SaveSlotRepository _slotRepo;
    private PlayerRepository   _playerRepo;
    // Aquí añadiremos FarmRepository, InventoryRepository...

    // Slot actualmente cargado
    public int CurrentSlotId { get; private set; } = -1;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Añadimos una proteccion porque si la instancia es nula los Awake se han iniciado en el orden incorrecto, manejamos esa posibilidad
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("[SaveGameManager] DatabaseManager.Instance es null. " +
                        "Asegúrate de que DatabaseManager esté en la escena y " +
                        "configurado ANTES en Edit > Project Settings > Script Execution Order.");
            return;
        }

        // Inicializamos los repositorios pasándoles la instancia del DatabaseManager
        _slotRepo   = new SaveSlotRepository(DatabaseManager.Instance);
        _playerRepo = new PlayerRepository(DatabaseManager.Instance);
    }

    // -------------------------------------------------------
    // API PÚBLICA que se usa para los otros scripts
    // -------------------------------------------------------

    /// Crea una nueva partida y la deja lista para empezar a jugar.
    /// La llamaremos desde el menú principal    
    public void NewGame(string slotName, string playerName)
    {
        //Crear el slot
        int slotId = _slotRepo.CreateSlot(slotName, playerName);
        CurrentSlotId = slotId;

        // crear datos iniciales del jugador
        var defaultPlayer = new PlayerData
        {
            SlotId    = slotId,
            Name      = playerName,
            Position  = Vector3.zero
        };
        _playerRepo.SavePlayer(defaultPlayer);

        // Aquí crearíamos la granja inicial, el inventrario

        Debug.Log($"[Save] Nueva partida creada: '{slotName}' para '{playerName}'");
    }

    /// Carga una partida existente. Llamar a esto al pulsar un slot en el menú

    public PlayerData LoadGame(int slotId)
    {
        CurrentSlotId = slotId;
        PlayerData player = _playerRepo.LoadPlayer(slotId);
        // Aquí cargaríamos inventario, mapa de la granja

        Debug.Log($"[Save] Partida {slotId} cargada.");
        return player;
    }

    /// Guarda el estado actual. Llamar periódicamente o al salir al menú
    public void SaveCurrentGame(PlayerData currentPlayer, float playTime)
    {
        if (CurrentSlotId == -1)
        {
            Debug.LogError("[Save] No hay ningún slot activo. ¿Olvidaste llamar a NewGame o LoadGame?");
            return;
        }

        // Guardamos todos los sistemas
        _playerRepo.SavePlayer(currentPlayer);
        _slotRepo.UpdateLastSaved(CurrentSlotId, playTime);
        // _farmRepo.SaveFarms(...)
        // _inventoryRepo.SaveInventory(...)

        Debug.Log("[Save] Partida guardada.");
    }

    public System.Collections.Generic.List<SaveSlotData> GetAllSaveSlots()
        => _slotRepo.GetAllSlots();

    public void DeleteSave(int slotId)
        => _slotRepo.DeleteSlot(slotId);
}