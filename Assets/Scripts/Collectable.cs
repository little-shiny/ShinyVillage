using UnityEngine;
// Se necesita que el player se ponga encima del collectable
// Se añade el collectable al player
// se elimina el collectable de la escena

public class Collectable : MonoBehaviour
{
    public CollectableType type;

    // Función que detecta el trigger del collider del item. el problema es que no sabemos que collider es y si es el del player
    // Para ello se necesita otro script para saber que viene del player (Script Player.cs)
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>(); // Busca si existe el script del componente Player y si colisiona con el item

        // Si player existe

        if (player)
        {
            player.inventory.Add(type);
            //Ahora se elimina el item porque se ha "recogido"
            Destroy(this.gameObject); // Pasamos el item para eliminarlo
        }
    }
    
}

// Función que nos permite saber el tipo de item que se recoge
// Por defecto son integers, NONE es 0, ONION_SEED es 1
public enum CollectableType
{
    NONE, ONION_SEED
}
