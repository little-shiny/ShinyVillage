using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    // Awake lo que hace es incializa las variables antes de que se inicie la aplicacion, de manera que solo se carga una vez
    private void Awake()
    {
        // Creamos la instancia del inventario con la cantidad máxima de slots que queremos que tenga
        inventory = new Inventory(21);
    } 
}
