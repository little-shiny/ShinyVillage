using System.Collections.Generic;
using UnityEngine;

// Clase que implementa el funcionamiento de la interfaz del inventario
public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel; // Variable que representa la parte visual del inventario
    public Player player;

    public List<Slot_UI> slots = new List<Slot_UI>();
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
            Setup(); // Se carga la información del inventario
        }
        else
        {
            inventoryPanel.SetActive(false); // Lo oculta
        }
    }

    // Método que obtiene la información del inventario para mostrarla al abrirlo
    void Setup()
    {
        // Comprueba si el script de inventario y el de inventory_ui tienen los mismos slots para añadirlos 
        // Hay que comprobar si cuando se van agregando los slots, hay alguno que sea vacío. por eso el bucle repite mientras estén llenos
        
        for(int i = 0; i < slots.Count; i++)
        {
            if(player.inventory.slots[i].type != CollectableType.NONE) // si el slot no esta vacío se configura
            {
                slots[i].SetItem(player.inventory.slots[i]); // se hace el slot visible en la interfaz
            }
            else
            {
                slots[i].SetEmpty(); // si no hay nada lo "pintamos" vacio
            }
        }
        
    }
}
