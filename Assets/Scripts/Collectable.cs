using UnityEngine;
// Se necesita que el player se ponga encima del collectable
// Se añade el collectable al player
// se elimina el collectable de la escena

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
            player.numOnionSeed++;

            //Ahora se elimina el item porque se ha "recogido"
            Destroy(this.gameObject); // Pasamos el item para eliminarlo
        }
    }
    
}
