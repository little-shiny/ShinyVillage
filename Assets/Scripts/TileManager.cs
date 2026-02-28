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
        if (position.x == int.MinValue) return false; // posición inválida por borde
    
        TileBase tile = interactableMap.GetTile(position);
        if(tile != null && tile.name == "invisible_int")
        {
            return true;
        }
        
        return false;
    }

    // Método que establece las tiles como "interactuadas"
    public void SetInteracted(Vector3Int position)
    {
        interactableMap.SetTile(position, InteractedTile);
    }

    // Método que obtiene la posición de la celda del tilemap (No en el mundo)
    public Vector3Int GetCellPosition(Vector3 worldPosition)
    {
        Vector3Int cellPos = interactableMap.WorldToCell(worldPosition);
        Vector3 cellCenter = interactableMap.GetCellCenterWorld(cellPos);

        // Establecemos un limite para el cual el interactionPoint sea preciso para que no pinte tiles adyacentes
        float threshold = 0.3f;

        if(Vector3.Distance(worldPosition,cellCenter) > threshold)
        {
            return new Vector3Int(int.MinValue, int.MinValue, 0);
        }

        return cellPos;
    }

}
