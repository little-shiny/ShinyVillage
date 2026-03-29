using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

//Clase que almacena los datos del jugador de una partida
[Serializable]
public class PlayerData
{
    public int Id;
    public int SlotId;
    public string Name;
    public Vector3 Position; // Posición en el mundo
}

// Guarda y carga el jugador
public class PlayerRepository
{
    private readonly DatabaseManager _db;

    public PlayerRepository(DatabaseManager db) => _db = db;

    /// Guarda al jugador. Si ya existe en este slot, lo actualiza; si no, lo crea.
    /// Patrón "upsert" (INSERT OR REPLACE en SQLite).
    public void SavePlayer(PlayerData data)
    {
        // Comprobamos si ya existe un registro para este slot
        object existing = _db.ExecuteScalar(
            "SELECT id FROM Players WHERE slot_id = @slot_id",
            cmd => DatabaseManager.AddParameter(cmd, "@slot_id", data.SlotId)
        );

        if (existing == null || existing == DBNull.Value)
        {
            // Primera vez → INSERT
            _db.ExecuteNonQuery(
                @"INSERT INTO Players (slot_id, name, pos_x, pos_y, pos_z)
                  VALUES (@slot_id, @name, @pos_x, @pos_y, @pos_z)",
                cmd => FillPlayerParams(cmd, data)
            );
        }
        else
        {
            // Ya existe → UPDATE
            _db.ExecuteNonQuery(
                @"UPDATE Players SET name=@name, pos_x=@pos_x, pos_y=@pos_y, pos_z=@pos_z
                  WHERE slot_id=@slot_id",
                cmd => FillPlayerParams(cmd, data)
            );
        }
    }


    /// Carga los datos del jugador de un slot concreto.
    /// Devuelve null si no existe ningún jugador para ese slot.
    public PlayerData LoadPlayer(int slotId)
    {
        // Usamos ExecuteQuery (el nuevo método seguro) en lugar de ExecuteReader
        // para evitar que el reader quede abierto y bloquee la conexión
        List<Dictionary<String, object>> rows = _db.ExecuteQuery(
             "SELECT id, slot_id, name, pos_x, pos_y, pos_z FROM Players WHERE slot_id = @slot_id",
            cmd => DatabaseManager.AddParameter(cmd, "@slot_id", slotId)
        );

        //Si no hay ninguna fila no existe el jugador
        if (rows.Count == 0)
        {
            Debug.LogWarning($"[DB] No se encontró jugador para slot {slotId}.");
            return null;
        }

        // Leemos la primera (y única) fila por nombre de columna, así no depende del orden y es más fácil de leer
        var row = rows[0];

        return new PlayerData
        {
            Id     = Convert.ToInt32(row["id"]),
            SlotId = Convert.ToInt32(row["slot_id"]),
            Name   = row["name"].ToString(),

            // Reconstruimos el Vector3 desde las 3 columnas separadas.
            // Usamos Convert.ToSingle porque SQLite devuelve los REAL como double.
            Position = new Vector3(
                Convert.ToSingle(row["pos_x"]),
                Convert.ToSingle(row["pos_y"]),
                Convert.ToSingle(row["pos_z"])
            )
        };
    


        // using (IDataReader reader = _db.ExecuteReader(
        //     "SELECT id, slot_id, name, pos_x, pos_y, pos_z FROM Players WHERE slot_id = @slot_id",
        //     cmd => DatabaseManager.AddParameter(cmd, "@slot_id", slotId)))
        // {
        //     if (reader.Read())
        //     {
        //         return new PlayerData
        //         {
        //             Id = reader.GetInt32(0),
        //             SlotId = reader.GetInt32(1),
        //             Name = reader.GetString(2),

        //             // Reconstruimos el Vector3 desde las 3 columnas separadas
        //             Position = new Vector3(
        //                 (float)reader.GetDouble(8),
        //                 (float)reader.GetDouble(9),
        //                 (float)reader.GetDouble(10)
        //             )
        //         };
        //     }
        // }

        // Debug.LogWarning($"[DB] No se encontró jugador para slot {slotId}");
        // return null;
    }

    // Método auxiliar para no repetir la asignación de parámetros
    private void FillPlayerParams(IDbCommand cmd, PlayerData data)
    {
        DatabaseManager.AddParameter(cmd, "@slot_id",    data.SlotId);
        DatabaseManager.AddParameter(cmd, "@name",       data.Name);
        DatabaseManager.AddParameter(cmd, "@pos_x",      data.Position.x);
        DatabaseManager.AddParameter(cmd, "@pos_y",      data.Position.y);
        DatabaseManager.AddParameter(cmd, "@pos_z",      data.Position.z);
    }
}