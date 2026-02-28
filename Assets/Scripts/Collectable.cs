using UnityEngine;
// Separación de antiguo Collectable en Item y Collectable.


[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
   
    // Función que detecta el trigger del collider del item. el problema es que no sabemos que collider es y si es el del player
    // Para ello se necesita otro script para saber que viene del player (Script Player.cs)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>(); // Busca si existe el script del componente Player y si colisiona con el item

        // Si player existe
        if (player)
        {
            Item item = GetComponent<Item>();
            if(item != null)
            {
                player.inventory.Add(item);
                //Ahora se elimina el item porque se ha "recogido"
                // Forzamos el pintar el inventario de nuevo 
                Inventory_UI ui = FindFirstObjectByType<Inventory_UI>();
                if(ui != null)
                {
                    ui.Refresh();
                }
                Destroy(this.gameObject); // Pasamos el item para eliminarlo
            }
        }
    }    
}