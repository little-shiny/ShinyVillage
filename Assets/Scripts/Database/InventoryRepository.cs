using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// Clase que almacena los datos de un item del inventario para la base de datos
/// </summary>
[Serializable]
public class InventoryItemData
{
    public int Id;
    public int SlotId;
    public string ItemId;      // Identificador único del item
    public string ItemName;    // Nombre legible del item
    public int Quantity;       // Cantidad de items en este slot
    public int SlotIndex;      // Posición en el inventario
    public string ItemData;    // Datos extra en JSON (para futuras expansiones)
}

/// <summary>
/// Repositorio que gestiona las operaciones de base de datos para el inventario del jugador
/// </summary>
public class InventoryRepository
{
    private readonly DatabaseManager _db;

    public InventoryRepository(DatabaseManager db) => _db = db;

    /// <summary>
    /// Guarda el inventario completo del jugador en la base de datos.
    /// Borra el inventario anterior y guarda el nuevo estado.
    /// </summary>
    /// <param name="slotId">ID del slot de guardado</param>
    /// <param name="inventory">Inventario actual del jugador</param>
    public void SaveInventory(int slotId, Inventory inventory)
    {
        // Primero se borra el inventario anterior de este slot
        _db.ExecuteNonQuery(
            "DELETE FROM Inventory WHERE slot_id = @slot_id",
            cmd => DatabaseManager.AddParameter(cmd, "@slot_id", slotId)
        );

        // Luego se guarda el estado actual del inventario
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            var slot = inventory.slots[i];
            
            // Solo se guardan los slots que tienen items
            if (!string.IsNullOrEmpty(slot.itemName) && slot.count > 0)
            {
                _db.ExecuteNonQuery(
                    @"INSERT INTO Inventory (slot_id, item_id, item_name, quantity, slot_index, item_data)
                      VALUES (@slot_id, @item_id, @item_name, @quantity, @slot_index, @item_data)",
                    cmd =>
                    {
                        DatabaseManager.AddParameter(cmd, "@slot_id", slotId);
                        DatabaseManager.AddParameter(cmd, "@item_id", slot.itemName); // Se usa itemName como ID
                        DatabaseManager.AddParameter(cmd, "@item_name", slot.itemName);
                        DatabaseManager.AddParameter(cmd, "@quantity", slot.count);
                        DatabaseManager.AddParameter(cmd, "@slot_index", i);
                        DatabaseManager.AddParameter(cmd, "@item_data", "{}"); // JSON vacío por ahora
                    }
                );
            }
        }

        Debug.Log($"[DB] Inventario guardado: {inventory.slots.Count} slots procesados para slot {slotId}");
    }

    /// <summary>
    /// Carga el inventario del jugador desde la base de datos.
    /// Devuelve una lista de items que debe ser aplicada al inventario del jugador.
    /// </summary>
    /// <param name="slotId">ID del slot de guardado</param>
    /// <returns>Lista de items del inventario ordenados por su posición</returns>
    public List<InventoryItemData> LoadInventory(int slotId)
    {
        var items = new List<InventoryItemData>();

        // Se consultan todos los items del inventario ordenados por su índice
        var rows = _db.ExecuteQuery(
            @"SELECT id, slot_id, item_id, item_name, quantity, slot_index, item_data 
              FROM Inventory 
              WHERE slot_id = @slot_id 
              ORDER BY slot_index ASC",
            cmd => DatabaseManager.AddParameter(cmd, "@slot_id", slotId)
        );

        // Se convierten las filas en objetos InventoryItemData
        foreach (var row in rows)
        {
            items.Add(new InventoryItemData
            {
                Id = Convert.ToInt32(row["id"]),
                SlotId = Convert.ToInt32(row["slot_id"]),
                ItemId = row["item_id"].ToString(),
                ItemName = row["item_name"].ToString(),
                Quantity = Convert.ToInt32(row["quantity"]),
                SlotIndex = Convert.ToInt32(row["slot_index"]),
                ItemData = row["item_data"].ToString()
            });
        }

        Debug.Log($"[DB] Inventario cargado: {items.Count} items para slot {slotId}");
        return items;
    }

    /// <summary>
    /// Limpia el inventario de un slot específico de la base de datos.
    /// </summary>
    /// <param name="slotId">ID del slot de guardado</param>
    public void ClearInventory(int slotId)
    {
        _db.ExecuteNonQuery(
            "DELETE FROM Inventory WHERE slot_id = @slot_id",
            cmd => DatabaseManager.AddParameter(cmd, "@slot_id", slotId)
        );

        Debug.Log($"[DB] Inventario limpiado para slot {slotId}");
    }
}