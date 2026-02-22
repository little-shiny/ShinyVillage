using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot_UI : MonoBehaviour
{
    public Image itemIcon; // objeto que mostrará el icono del item en el inventario
    public TextMeshProUGUI quantityText; // Texto que mostrará la cantidad de items aplilados en el inventario

// Método que establece la parte visible del slot en el inventario
// obtiene los datos de inventory y los utiliza para mostrar la información
    public void SetItem(Inventory.Slot slot)
    {
        if(slot != null)// Si hay algo en el slot
        {
            itemIcon.sprite = slot.icon; // leemos el icono del slot
            itemIcon.color = new Color(1, 1, 1, 1); 
            quantityText.text = slot.count.ToString(); // obtenemos el numero de items del mismo tipo del slot y lo mostramos en el tmprotext
        }
    }

    // Por defecto unity muestra un cuadrado blanco cuando no hay un sprite asociado en la imagen
    // Lo ponemos transparente con el valor alfa del color (ultimo parametro en 0)

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        // Si no hay items, no queremos mostrar el texto de la cantidad, por lo que se pone la string vacia
        quantityText.text = "";
    }
}
