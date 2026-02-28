using UnityEngine;
// Patrón Singleton, instancia unica
// Static para garantizar que si se crea una instancia sea la unica
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ItemManager itemManager;

    private void Awake()
    {
        // Si existe una instancia diferente a la que se crea se elimina para tener una sola
        if (instance != null && instance != this) 
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        itemManager = GetComponent<ItemManager>();
    }
}
