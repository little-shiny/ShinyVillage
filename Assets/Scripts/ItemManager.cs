using System.Collections.Generic;
using UnityEngine;

// Script que perite asociar un prefab de un item junto con su tipo al instanciarlo para que no se "pierda" al meterlo en el inventario
// Se utiliza un Diccionario, ya que almacena pares de valores asociados
public class ItemManager : MonoBehaviour
{
    public Collectable[] collectableItems;
    private Dictionary<CollectableType, Collectable> collectableItemsDict = 
        new Dictionary<CollectableType, Collectable>();

    // Funcion que inicia el diccionario con los collectables que existen
    private void Awake()
    {
        foreach (Collectable item in collectableItems)
        {
            AddItem(item);
        }
    }

// Método que añade un item al diccionario
    private void AddItem(Collectable item)
    {
        // Se comprueba qye no exista ya en el diccionario
        if (!collectableItemsDict.ContainsKey(item.type))
        {
            collectableItemsDict.Add(item.type, item);
        }
    }

    //Función que devuelve un collectable del diccionario del tipo introducido por parámetro
    public Collectable GetItemByType(CollectableType type)
    {
        if (collectableItemsDict.ContainsKey(type))
        {
            return collectableItemsDict[type];
        }
        return null;
    }
}
