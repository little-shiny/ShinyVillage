using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Almacena solo las tiles que son interactuables

    [SerializeField] private Tile hiddenInteractableTile; // tile que sustituirá a las interactuables
    [SerializeField] private Tile InteractedTile; // tile que ya ha sido interactuada

    public void Start()
    {
        // establece las tiles en nuestra variable del script
        foreach(var position in interactableMap.cellBounds.allPositionsWithin)
        {
            interactableMap.SetTile(position, hiddenInteractableTile);
        }
    }

    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position); // Obtenemos el tile donde nos encontramos

        if(tile != null) // Si existe tile
        {
            if(tile.name == "invisible_int") // Si es tile interactable
            {
                return true; 
            }
        }
        return false;
    }

    // Método que establece las tiles como "interactuadas"
    public void SetInteracted(Vector3Int position)
    {
        interactableMap.SetTile(position, InteractedTile);
    }

}
