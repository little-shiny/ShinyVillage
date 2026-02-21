using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Atributo de la clase que permite la persistencia de los datos para poder visualizarlos luego en la escena
public class Inventory // No es monoBehaviour porque no se le atribuye a ningun objeto en la escena
{
    [System.Serializable]
    public class Slot
    {
        public int count; // Cantidad de items en el slot
        public int maxAllowed;
        // Necesitamos saber que tipo de item es
        public CollectableType type;

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
        public void AddItem(CollectableType type)
        {
            this.type = type; //Le atribuimos al slot el tipo de objeto para poder aplicarlo
            count++; // Se suma uno a la cantidad de items en el slot
        }
    }

    

    public List<Slot> slots = new List<Slot>();

    // Constructoir

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
    public void Add(CollectableType typeToAdd)
    {
        // Ahora buscamos si ya hay items como el que queremos añadir en el inventarioi
        foreach(Slot slot in slots)
        {
            //En caso de que ya haya un item de ese tipo y quepa: 

            if(slot.type == typeToAdd && slot.CanAddItem()) // si ya 
            {
                slot.AddItem(typeToAdd); // se añade el item al slot
                return; // Termionamos
            }

        }

         // En caso de que no haya ningun item de ese tipo
        foreach(Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE) // Si el slot está vacío
            {
                slot.AddItem(typeToAdd);
                return; 
            }
        }

    }
}
