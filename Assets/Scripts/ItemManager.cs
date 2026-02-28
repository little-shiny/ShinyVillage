using System.Collections.Generic;
using UnityEngine;

// Script que perite asociar un prefab de un item junto con su tipo al instanciarlo para que no se "pierda" al meterlo en el inventario
// Se utiliza un Diccionario, ya que almacena pares de valores asociados
public class ItemManager : MonoBehaviour
{
    public Item[] items;
    private Dictionary<string, Item> nameToItemDict = 
        new Dictionary<string, Item>();

    // Funcion que inicia el diccionario con los Items que existen
    private void Awake()
    {
        foreach (Item item in items)
        {
            AddItem(item);
        }
    }

// Método que añade un item al diccionario
    private void AddItem(Item item)
    {
        // Se comprueba qye no exista ya en el diccionario
        if (!nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    //Función que devuelve un Item del diccionario del tipo introducido por parámetro
    public Item GetItemByName(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            return nameToItemDict[key];
        }
        return null;
    }
}
