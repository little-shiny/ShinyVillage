using System.IO;
using UnityEngine;
//Script que controla y maneja el guardado y carga de datos de las partidas del juego
[System.Serializable]
public class PlayerData
{
    public string playerName;
}
public class JSONManager : MonoBehaviour
{
    //Ruta de guardado de los JSON con los datos. Guardará los datos en la carpeta Assets
    private string filePath;

    void Start()
    {
        // Sustitución por persistent data para que cuando se construya de nuevo el juego no se elimine
        filePath = Path.Combine(Application.persistentDataPath, "UserData.json");
    }

    //Update que guarda los datos cada vez que se pulsa la letra "S" para probar la función y carga los datos con la tecla "L"
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveData();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }
    }
    //Método que guarda los datos del objeto PlayerData en un archivo JSON con la librería propia de Unity. 
    //Almacena en un string los datos del archivo JSON
    public void SaveData()
    {
        PlayerData player = new PlayerData();
        player.playerName = "ExampleUsername"; //Usamos un usuario de prueba 

        string json = JsonUtility.ToJson(player, true);
        File.WriteAllText(filePath, json);
        //Debug
        Debug.Log("Data saved" + json);
    }

    //Función que carga los datos del Json. Necesitamos comprobar si el fichero ya existe porque si no lanza excepción
    public void LoadData()
    {
        if (File.Exists(filePath)) //Si existe el archivo
        {
            string json = File.ReadAllText(filePath);
            //Ahora convertimos el string del json en un objeto playerData de nuevo
            PlayerData loadedPlayer = JsonUtility.FromJson<PlayerData>(json);

            //debug
            Debug.Log("Loaded Data" + loadedPlayer.playerName);
        }
        else
        {
            //debug
            Debug.Log("No saved data found");
        }
    }

    //Método que elimina el archivo de datos de guardado existente y crea uno por defecto
    public void ResetData()
    {
        PlayerData resetData = new PlayerData();
        resetData.playerName = "Default";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            
            //debug
            Debug.Log("File Deleted");
        }
    }
}
