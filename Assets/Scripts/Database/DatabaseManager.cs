using System.Data;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Mono.dat
// Gestiona la conexion y las operaciones con la db en sqllite
//Singleton para que se pueda acceder desde todas las partes del juego 
public class DatabaseManager : MonoBehaviour
{
    //Singleton : una sola instancia
    public static DatabaseManager Instance
    {
        get;
        private set;
    }

    //Conexión con la db
    private IDbConnection _connection; 

    //Nombre de la db
    private const string DB_NAME = "savegame.db";

    //Ruta completa donde se guarda el archivo
    // Application.persistentdatapath es la carpeta de datos del juegpo
    private string DbPath => Path.Combine(Application.persistentDataPath, DB_NAME);

    private void Awake()
    {
        //Si existe una instancia de instance se destruye el objeto duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre escenas

        InitializeDatabase(); // Creamos/abrimos la base de datos al iniciar
    }
    
     private void OnDestroy()
    {
        // Siempre cerrar la conexión al destruir el objeto para evitar corrupción
        CloseConnection();
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    // -------------------------------------------------------
    // INICIALIZACIÓN
    // -------------------------------------------------------

    /// Abre la conexión y crea las tablas si no existen todavía.
    private void InitializeDatabase()
    {
        // Formato de connection string que entiende Mono.Data.Sqlite
        string connectionString = $"URI=file:{DbPath}";

        Debug.Log($"[DB] Base de datos en: {DbPath}");

        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        // Creamos todas las tablas 
        CreateTables();
    }

    /// Crea las tablas de la base de datos si no existen.
    /// "IF NOT EXISTS" asegura que no se sobreescriben datos ya guardados.
    private void CreateTables()
    {
        // Un solo comando puede ejecutar múltiples sentencias SQL separadas por ;
        string createTablesSQL = @"

            -- Tabla principal de slots de partida
            -- Cada fila es una partida guardada que aparecerá en la pantalla de selección
            CREATE TABLE IF NOT EXISTS SaveSlots (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                slot_name   TEXT NOT NULL,           -- Nombre del slot (ej: 'Partida 1')
                player_name TEXT NOT NULL,           -- Nombre del jugador
                created_at  TEXT NOT NULL,           -- Fecha de creación
                last_saved  TEXT NOT NULL,           -- Última vez guardado
                play_time   REAL DEFAULT 0           -- Tiempo total jugado en segundos
            );

            -- Datos del jugador vinculados a un slot
            CREATE TABLE IF NOT EXISTS Players (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                slot_id     INTEGER NOT NULL,        -- FK → SaveSlots.id
                name        TEXT NOT NULL,
                level       INTEGER DEFAULT 1,
                gold        INTEGER DEFAULT 0,
                exp         INTEGER DEFAULT 0,
                health      REAL DEFAULT 100,
                max_health  REAL DEFAULT 100,
                pos_x       REAL DEFAULT 0,          -- Posición en el mundo
                pos_y       REAL DEFAULT 0,
                pos_z       REAL DEFAULT 0,
                FOREIGN KEY (slot_id) REFERENCES SaveSlots(id) ON DELETE CASCADE
            );

            -- Granjas del jugador (puede tener varias)
            CREATE TABLE IF NOT EXISTS Farms (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                slot_id     INTEGER NOT NULL,
                farm_name   TEXT NOT NULL,           -- Nombre de la granja
                size_x      INTEGER DEFAULT 10,      -- Tamaño de la granja en tiles
                size_y      INTEGER DEFAULT 10,
                unlocked    INTEGER DEFAULT 1,       -- 1 = desbloqueada, 0 = no
                FOREIGN KEY (slot_id) REFERENCES SaveSlots(id) ON DELETE CASCADE
            );

            -- Estado de cada tile/celda de la granja
            CREATE TABLE IF NOT EXISTS FarmTiles (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                farm_id     INTEGER NOT NULL,
                tile_x      INTEGER NOT NULL,        -- Posición X del tile en la granja
                tile_y      INTEGER NOT NULL,        -- Posición Y del tile en la granja
                soil_state  TEXT DEFAULT 'dry',      -- Estado: dry, watered, fertilized
                crop_id     TEXT DEFAULT '',         -- ID del cultivo plantado (vacío = nada)
                growth_stage INTEGER DEFAULT 0,      -- Fase de crecimiento (0-5 por ejemplo)
                days_planted INTEGER DEFAULT 0,      -- Días desde que se plantó
                FOREIGN KEY (farm_id) REFERENCES Farms(id) ON DELETE CASCADE
            );

            -- Inventario del jugador (un item por fila)
            CREATE TABLE IF NOT EXISTS Inventory (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                slot_id     INTEGER NOT NULL,
                item_id     TEXT NOT NULL,           -- ID del item (ej: 'seed_carrot', 'tool_hoe')
                item_name   TEXT NOT NULL,           -- Nombre legible
                quantity    INTEGER DEFAULT 1,
                slot_index  INTEGER DEFAULT -1,      -- Posición en el inventario (-1 = sin asignar)
                item_data   TEXT DEFAULT '{}',       -- JSON para datos extra (durabilidad, etc.)
                FOREIGN KEY (slot_id) REFERENCES SaveSlots(id) ON DELETE CASCADE
            );
        ";

        ExecuteNonQuery(createTablesSQL);
        Debug.Log("[DB] Tablas creadas/verificadas correctamente.");
    }

    // -------------------------------------------------------
    // Métodos aux
    // -------------------------------------------------------

    //Ejecuta una sentencia sql que no devuelve datos (insert, update, delete...)
    //todo explicarla
    public void ExecuteNonQuery(string sql, Action<IDbCommand> parameterize = null)
    {
        using (IDbCommand cmd = _connection.CreateCommand())
        {
            cmd.CommandText = sql;
            parameterize?.Invoke(cmd); // Permite añadir parámetros desde fuera
            cmd.ExecuteNonQuery();
        }
    }

    // Ejecuta sentencia sql que devuelve datos como select
    // Devuelve IdataReader para leer fila a fila los datos
    //todo explicar
    public IDataReader ExecuteReader(string sql, Action<IDbCommand> parameterize = null)
    {
        IDbCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        parameterize?.Invoke(cmd);
        return cmd.ExecuteReader();
    }

    /// Crea y añade un parámetro a un comando SQL de forma segura.
    /// Usar SIEMPRE parámetros en lugar de concatenar strings 
    public static IDbDataParameter AddParameter(IDbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name; // Ej: "@player_name"
        param.Value = value ?? DBNull.Value; // Si es null, guardamos NULL en la BD
        cmd.Parameters.Add(param);
        return param;
    }

    private void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
            _connection.Dispose();
            Debug.Log("[DB] Conexión cerrada.");
        }
    }
}
