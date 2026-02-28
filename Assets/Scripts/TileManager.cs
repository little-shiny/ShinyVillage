using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Almacena solo las tiles que son interactuables

    [SerializeField] private Tile hiddenInteractableTile; // tile que sustituirá a las interactuables

    public void Start()
    {
        // establece las tiles en nuestra variable del script
        foreach(var position in interactableMap.cellBounds.allPositionsWithin)
        {
            interactableMap.SetTile(position, hiddenInteractableTile);
        }
    }

}
