using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    // Awake inicializa las variables antes de que se inicie la aplicación, de manera que solo se carga una vez
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
            // Obtenemos la dirección desde el Animator del componente Movement
            Movement movement = GetComponent<Movement>();
            Vector3 lastDir = new Vector3(
                movement.animator.GetFloat("horizontal"),
                movement.animator.GetFloat("vertical")
            ).normalized;

            // Si no hay dirección guardada, usamos down por defecto
            if (lastDir == Vector3.zero)
                lastDir = Vector3.down;

            // Obtenemos la celda exacta donde está el jugador y su centro
            // para evitar imprecisiones al estar en el borde de una tile
            Vector3Int playerCell = GameManager.instance.tileManager.GetCellPosition(transform.position);
            Vector3 playerCellCenter = GameManager.instance.tileManager.GetCellCenter(playerCell);

            // Calculamos la posición de la tile frente al jugador sumando la dirección al centro de su celda
            Vector3 facingPos = playerCellCenter + lastDir * 1f;
            Vector3Int targetCell = GameManager.instance.tileManager.GetCellPosition(facingPos);

            // Si la tile es interactuable, la marcamos como interactuada
            if (GameManager.instance.tileManager.IsInteractable(targetCell))
            {
                GameManager.instance.tileManager.SetInteracted(targetCell);
            }
        }
    }

    // Efecto visual al soltar items del inventario para que vuelvan a aparecer en el mapa
    public void DropItem(Collectable item)
    {
        // Posición donde se van a soltar (Spawn) es la posición del jugador
        Vector2 spawnLocation = transform.position;

        // Creamos un offset para que el objeto salga fuera del collider del jugador y evitar que se recoja automáticamente
        Vector2 spawnOffset = Random.insideUnitCircle * 2f;

        // Instanciamos el item que se suelta del inventario, como si le volvieramos a "dar vida"
        // La función Instantiate usa el collectable en cuestión, las coordenadas del jugador + el offset y la rotación del prefab
        Collectable droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // Añadimos el "efecto" al soltar con la física de Unity
        droppedItem.rb2d.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
    }
}