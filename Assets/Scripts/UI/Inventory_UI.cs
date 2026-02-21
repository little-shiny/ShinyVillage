using UnityEngine;

// Clase que implementa el funcionamiento de la interfaz del inventario
public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel; // Variable que representa la parte visual del inventario
    void Update()
    {
        // Atajo de teclado para abrirse
        if (Input.GetKeyDown(KeyCode.Tab)) // Al presionar el tabulador
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        // comprobamos el estado del inventario
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true); // Hace visible el inventario
        }
        else
        {
            inventoryPanel.SetActive(false); // Lo oculta
        }
    }
}
