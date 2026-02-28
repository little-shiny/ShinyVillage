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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Movement movement = GetComponent<Movement>();
            Vector3 lastDir = new Vector3(
                movement.animator.GetFloat("horizontal"),
                movement.animator.GetFloat("vertical")
            ).normalized;

            if (lastDir == Vector3.zero)
                lastDir = Vector3.down;

            Vector3Int cardinalDir;
            if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
                cardinalDir = new Vector3Int((int)Mathf.Sign(lastDir.x), 0, 0);
            else
                cardinalDir = new Vector3Int(0, (int)Mathf.Sign(lastDir.y), 0);

            Vector3Int playerCell = GameManager.instance.tileManager.GetCellPosition(transform.position);
            Vector3Int targetCell = playerCell + cardinalDir;

            if (GameManager.instance.tileManager.IsInteractable(targetCell))
            {
                GameManager.instance.tileManager.SetInteracted(targetCell);
            }
        }
    }
    // Efecto visual al soltar items del inventario para que vuelvan a aparecer en el mapa
    public void DropItem(Item item)
    {
        // Posición donde se van a soltar (Spawn) es la posición del jugador
        Vector2 spawnLocation = transform.position;

        // Creamos un offset para que el objeto salga fuera del collider del jugador y evitar que se recoja automáticamente
        Vector2 spawnOffset = Random.insideUnitCircle * 2f;

        // Instanciamos el item que se suelta del inventario, como si le volvieramos a "dar vida"
        // La función Instantiate usa el collectable en cuestión, las coordenadas del jugador + el offset y la rotación del prefab
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // Añadimos el "efecto" al soltar con la física de Unity
        droppedItem.rb2d.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
    }
}