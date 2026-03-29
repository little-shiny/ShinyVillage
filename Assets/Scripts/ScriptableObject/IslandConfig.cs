using System.IO;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// Configuración visual para una isla (partida guardada).
/// Cada isla tendrá colores únicos generados según la ID del slot de guardado para poder identificarlas.
/// Este ScriptableObject se genera solo, no se puede crear de forma manual
/// 
/// Se añade un menú
/// </summary>

[CreateAssetMenu(fileName ="IslandConfig", menuName ="Game/Island Config")]

public class IslandConfig : ScriptableObject
{
    [Header("Identificacion")]
    [Tooltip("ID del slot de guardado asociado a esta isla")]
    public int slotId;

    [Tooltip("Nombre de la isla (igual que el nombre del slot)")]
    public string islandName;
    
    [Header("Paleta de Colores")]
    [Tooltip("Color base del terreno")]
    public Color groundColor = Color.green;
    
    [Tooltip("Color secundario (agua)")]
    public Color waterColor = Color.blue;
    
    [Tooltip("Color de detalles")]
    public Color accentColor = Color.yellow;
    
    [Tooltip("Tinte general aplicado a todos los tiles")]
    public Color globalTint = Color.white;
}
