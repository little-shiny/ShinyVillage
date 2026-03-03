using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// Modelo de datos que representa una partida guardada.
[Serializable]
public class SaveSlotData
{
    public int Id;
    public string SlotName;
    public string PlayerName;
    public DateTime CreatedAt;
    public DateTime LastSaved;
    public float PlayTime;        // En segundos
}


/// Gestiona las operaciones de base de datos para los slots de partida.
/// Separa la lógica de BD del resto del juego (patrón Repository).
public class SaveSlotRepository
{
    private readonly DatabaseManager _db;

    public SaveSlotRepository(DatabaseManager db)
    {
        _db = db;
    }

    // -------------------------------------------------------
    // CREAR nueva partida
    // -------------------------------------------------------


    /// Inserta un nuevo slot de partida y devuelve su ID generado.
    public int CreateSlot(string slotName, string playerName)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Insertar el slot
        _db.ExecuteNonQuery(
            @"INSERT INTO SaveSlots (slot_name, player_name, created_at, last_saved, play_time)
              VALUES (@slot_name, @player_name, @created_at, @last_saved, 0)",
            cmd =>
            {
                // Usamos parámetros para evitar SQL Injection y problemas con caracteres especiales
                DatabaseManager.AddParameter(cmd, "@slot_name",   slotName);
                DatabaseManager.AddParameter(cmd, "@player_name", playerName);
                DatabaseManager.AddParameter(cmd, "@created_at",  now);
                DatabaseManager.AddParameter(cmd, "@last_saved",  now);
            }
        );

        // Obtenemos el ID de la fila recién insertada
        int newId = Convert.ToInt32(_db.ExecuteScalar("SELECT last_insert_rowid()"));
        Debug.Log($"[DB] Slot creado con ID: {newId}");
        return newId;
    }

    // -------------------------------------------------------
    // LEER todas las partidas
    // -------------------------------------------------------


    /// Devuelve todos los slots guardados ordenados por última modificación.
    /// lo usaremos para mostrar la lista en la pantalla de selección de partida.
    public List<SaveSlotData> GetAllSlots()
    {
        var slots = new List<SaveSlotData>();

        // Usamos ahora executeQuery
        var rows = _db.ExecuteQuery(
            "SELECT id, slot_name, player_name, created_at, last_saved, play_time " + "FROM SaveSlots ORDER BY last_saved DESC"
        );

        // Para cada resultado de la query anterior
        foreach(var row in rows)
        {
            slots.Add(new SaveSlotData // Creamos un slot de guardado
            {
                Id = Convert.ToInt32(row["id"]),
                SlotName = row["slot_name"].ToString(),
                CreatedAt = DateTime.Parse(row["created_at"].ToString()),
                LastSaved  = DateTime.Parse(row["last_saved"].ToString()),
                PlayTime   = Convert.ToSingle(row["play_time"])
            });
        }

        Debug.Log($"[DB] Se encontraron {slots.Count} partidas guardadas.");
        return slots;
    }

    // -------------------------------------------------------
    // ACTUALIZAR tiempo de juego y fecha de guardado
    // -------------------------------------------------------

    public void UpdateLastSaved(int slotId, float playTime)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _db.ExecuteNonQuery(
            "UPDATE SaveSlots SET last_saved = @last_saved, play_time = @play_time WHERE id = @id",
            cmd =>
            {
                DatabaseManager.AddParameter(cmd, "@last_saved", now);
                DatabaseManager.AddParameter(cmd, "@play_time",  playTime);
                DatabaseManager.AddParameter(cmd, "@id",         slotId);
            }
        );
    }

    // -------------------------------------------------------
    // ELIMINAR partida (CASCADE borrará todo lo relacionado)
    // -------------------------------------------------------


    /// Borra el slot y todos sus datos relacionados gracias al ON DELETE CASCADE
    /// definido en las Foreign Keys al crear las tablas.
    public void DeleteSlot(int slotId)
    {
        // Primero activar el soporte de foreign keys en SQLite (no viene activo por defecto)
        _db.ExecuteNonQuery("PRAGMA foreign_keys = ON");

        _db.ExecuteNonQuery(
            "DELETE FROM SaveSlots WHERE id = @id",
            cmd => DatabaseManager.AddParameter(cmd, "@id", slotId)
        );

        Debug.Log($"[DB] Slot {slotId} eliminado con todos sus datos.");
    }
}