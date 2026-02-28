using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    public Transform interactionPoint; // Objeto que definimos en unity para que sea el punto el cual interacciona

    // Awake lo que hace es incializa las variables antes de que se inicie la aplicacion, de manera que solo se carga una vez
    private void Awake()
    {
        // Creamos la instancia del inventario con la cantidad máxima de slots que queremos que tenga
        inventory = new Inventory(21);
    }

    void Update()
    {
        // Usaremos la tecla espacio para interactuar con las zonas del tilemap interactuable
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Convertimos la posicion en un vector para poder usarla como parámetro
            Vector3Int position = GameManager.instance.tileManager.GetCellPosition(interactionPoint.position);

            if (GameManager.instance.tileManager.IsInteractable(position))
            {
                Debug.Log("Tile is interactable");
                GameManager.instance.tileManager.SetInteracted(position);
            }

        }
    }

    // Efecto visual al soltar items del inventario para que vuelvan a aparecer en el mapa
    public void DropItem(Collectable item)
    {
        // Posición donde se van a soltar(Spawn) es la posición del jugador
        Vector2 spawnLocation = transform.position;

        // Creamos un offset para que el objeto salga fuera del collider del jugador y evitar que se recoja automáticamente
        Vector2 spawnOffset = Random.insideUnitCircle * 2f;

        // Instanciamos el item que se suelta del inventario, como le volvieramos a "dar vida"
        // La función instantiate usa el collectable en cuestión, las coordenadas del jugador + el offset y la rotación del prefab
        Collectable droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // Añadimos el "efecto" al soltar con la física de unity
        droppedItem.rb2d.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
    }
}
