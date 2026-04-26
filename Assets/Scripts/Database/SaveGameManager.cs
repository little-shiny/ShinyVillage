using System.Collections.Generic;
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
    
    private InventoryRepository _inventoryRepo;

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
        _inventoryRepo = new InventoryRepository(DatabaseManager.Instance);
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

        // El inventario inicial está vacío, no es necesario guardarlo aún

        Debug.Log($"[Save] Nueva partida creada: '{slotName}' para '{playerName}'");
    }

    /// Carga una partida existente. Llamar a esto al pulsar un slot en el menú

    public PlayerData LoadGame(int slotId)
    {
        CurrentSlotId = slotId;
        PlayerData player = _playerRepo.LoadPlayer(slotId);
        // Aquí cargaríamos inventario, mapa de la granja

        // Carga el slot completo para poder ver la isla
        List<SaveSlotData> allSlots = GetAllSaveSlots();
        SaveSlotData currentSlot = allSlots.Find(s => s.Id == slotId);

        if(currentSlot != null && IslandManager.Instance != null)
        {
            //Se carga la configuración de la isla
            IslandManager.Instance.LoadIsland(currentSlot);
        }

        Debug.Log($"[Save] Partida {slotId} cargada con isla: {currentSlot?.SlotName}");
        return player;
    }

    /// <summary>
    /// Carga el inventario del jugador desde la base de datos y lo aplica al inventario actual.
    /// Se debe llamar después de LoadGame, cuando el Player ya esté instanciado.
    /// </summary>
    /// <param name="playerInventory">Referencia al inventario del jugador</param>
    public void LoadPlayerInventory(Inventory playerInventory)
    {
        if (CurrentSlotId == -1)
        {
            Debug.LogError("[Save] No hay ningún slot activo. ¿Olvidaste llamar a LoadGame?");
            return;
        }

        // Se cargan los items de la base de datos
        List<InventoryItemData> inventoryItems = _inventoryRepo.LoadInventory(CurrentSlotId);

        // Se aplican al inventario del jugador
        foreach (var itemData in inventoryItems)
        {
            // Se obtiene el prefab del item desde el ItemManager
            Item itemPrefab = GameManager.instance.itemManager.GetItemByName(itemData.ItemName);
            
            if (itemPrefab != null && itemData.SlotIndex >= 0 && itemData.SlotIndex < playerInventory.slots.Count)
            {
                // Se restaura el slot con los datos guardados
                var slot = playerInventory.slots[itemData.SlotIndex];
                slot.itemName = itemData.ItemName;
                slot.icon = itemPrefab.data.icon;
                slot.count = itemData.Quantity;
            }
            else
            {
                Debug.LogWarning($"[Save] No se pudo cargar item '{itemData.ItemName}' en posición {itemData.SlotIndex}");
            }
        }

        Debug.Log($"[Save] Inventario del jugador cargado: {inventoryItems.Count} items");
    }

    /// Guarda el estado actual. Llamar periódicamente o al salir al menú
    public void SaveCurrentGame(PlayerData currentPlayer, Inventory playerInventory, float playTime)
    {
        if (CurrentSlotId == -1)
        {
            Debug.LogError("[SaveGameManager] No hay ningún slot activo. ¿Olvidaste llamar a NewGame o LoadGame?");
            return;
        }

        // Se guardan todos los sistemas
        _playerRepo.SavePlayer(currentPlayer);
        _inventoryRepo.SaveInventory(CurrentSlotId, playerInventory); // ← Guarda el inventario
        _slotRepo.UpdateLastSaved(CurrentSlotId, playTime);

        Debug.Log("[SaveGameManager] Partida guardada correctamente.");
    }

    
    public System.Collections.Generic.List<SaveSlotData> GetAllSaveSlots()
        => _slotRepo.GetAllSlots();

    public void DeleteSave(int slotId)
        => _slotRepo.DeleteSlot(slotId);
}