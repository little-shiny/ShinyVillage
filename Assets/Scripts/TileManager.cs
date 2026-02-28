using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Almacena solo las tiles que son interactuables

    [SerializeField] private Tile hiddenInteractableTile; // Tile que sustituirá a las interactuables
    [SerializeField] private Tile InteractedTile; // Tile que ya ha sido interactuada

    public void Start()
    {
        // Establece las tiles en nuestra variable del script
        foreach(var position in interactableMap.cellBounds.allPositionsWithin)
        {
            interactableMap.SetTile(position, hiddenInteractableTile);
        }
    }

    // Comprueba si la tile en la posición dada es interactuable
    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position); // Obtenemos el tile donde nos encontramos

        if(tile != null && tile.name == "invisible_int") // Si existe tile y es interactuable
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

    // Método que obtiene la posición de la celda del tilemap (no en el mundo)
    public Vector3Int GetCellPosition(Vector3 worldPosition)
    {
        return interactableMap.WorldToCell(worldPosition);
    }

    // Método que devuelve el centro en coordenadas de mundo de una celda
    // Se usa para calcular la tile frente al jugador sin imprecisiones en los bordes
    public Vector3 GetCellCenter(Vector3Int cellPosition)
    {
        return interactableMap.GetCellCenterWorld(cellPosition);
    }
}