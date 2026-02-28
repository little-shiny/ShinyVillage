using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Atributo de la clase que permite la persistencia de los datos para poder visualizarlos luego en la escena
public class Inventory // No es monoBehaviour porque no se le atribuye a ningun objeto en la escena
{
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count; // Cantidad de items en el slot
        public int maxAllowed;
        // Necesitamos saber que tipo de item es
        public Sprite icon;


        //Constructor
        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 99;
        }
       
        

        // boolean para el control al añadir elementos al inventario
        // Ahora crearemos el inventario que es una lista de Slots
        // Devuelve true si el inventario tiene huecos 
        public bool CanAddItem()
        {
            if(count < maxAllowed)
            {
                return true;
            }

            return false;
        }

        //Función que añade el item en el slot
        // Se cambia ItemType por Item para poder acceder así al icono desde inventory_ui
        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName; //Le atribuimos al slot el tipo de objeto para poder aplicarlo
            this.icon = item.data.icon;
            count++; // Se suma uno a la cantidad de items en el slot
        }

        //Función que elimina un item de un slot del inventario
        // Se comprueba si el slot está vacío antes para evitar errores
        public void RemoveItem()
        {
            if(count > 0) // Si hay al menos 1 item en el slot
            {
                count--;

                if(count == 0) // Si el slot se queda vacío
                {
                    icon = null;
                    itemName = "";
                }
            }
        }
    }

    
    public List<Slot> slots = new List<Slot>();

    // Constructor

    public Inventory(int numSlots)
    {
        // Al crear el inventario se pasa por parámetro el número de slots y con el for se llena la lista con los slots
        for(int i =0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    // método que nos permite añadir items en el inventario
    public void Add(Item item)
    {
        // Ahora buscamos si ya hay items como el que queremos añadir en el inventarioi
        foreach(Slot slot in slots)
        {
            //En caso de que ya haya un item de ese tipo y quepa: 

            if(slot.itemName == item.data.itemName && slot.CanAddItem()) // si ya 
            {
                slot.AddItem(item); // se añade el item al slot
                return; // Termionamos
            }

        }

         // En caso de que no haya ningun item de ese tipo
        foreach(Slot slot in slots)
        {
            if (slot.itemName == "") // Si el slot está vacío
            {
                slot.AddItem(item);
                return; 
            }
        }

    }

    // Función que elimina un item del inventario. Necesitamos obtener el indice del slot en el que se encuentra el item antes de eliminarlo ya que es una lista

    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }
}
