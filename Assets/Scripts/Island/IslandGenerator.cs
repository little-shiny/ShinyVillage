using UnityEngine;

/// <summary>
/// Genera configuraciones de isla basadas en un ID de semilla.
/// Cada ID de slot producirá colores únicos y consistentes. Esto asegura que cada partida guardada tenga su propia "personalidad" visual como si fuesen islas distintas
/// </summary>
public static class IslandGenerator
{
    /// Genera una configuración de isla basada en un ID de slot.
    /// Usa el ID como semilla para generar colores consistentes.

    /// <param name="slotId">ID único del slot de guardado</param>
    /// <param name="islandName">Nombre de la isla/partida</param>
    /// <returns>Configuración de isla con colores únicos</returns>
    public static IslandConfig GenerateIsland(int slotId, string islandName)
    {
        // Se usa el slotId como semilla para que siempre genere los mismos colores para la misma partida. 

        Random.InitState(slotId * 1000); // Multiplicamos por 1000 para mayor variación de los colores
        
        // Se crea la configuración en memoria (no como asset en disco) (Idea de Claude al depurar)
        IslandConfig config = ScriptableObject.CreateInstance<IslandConfig>();
        config.slotId = slotId;
        config.islandName = islandName;
        
        // Generación de colores 
        // Se usan rangos específicos para obtener colores agradables para la vista y que no sean chillones
        
        // Color de terreno: verdes, marrones, amarillos 
        config.groundColor = GenerateGroundColor();
        
        // Color de agua: azules, turquesas, violetas (para que tenga sentido y el agua no sea roja)
        config.waterColor = GenerateWaterColor();
        
        // Color de acento: colores para detalles
        config.accentColor = GenerateAccentColor();
        
        // Tinte global: muy sutil, casi blanco con ligera variación 
        config.globalTint = GenerateGlobalTint();
        
        // Se restaura el estado aleatorio para no afectar otros sistemas
        Random.InitState(System.DateTime.Now.Millisecond);
        
        return config;
    }
    
    /// <summary>
    /// Genera un color de terreno (verdes, marrones, amarillos).
    /// </summary>
    private static Color GenerateGroundColor()
    {
        // Hue entre 30-120 grados = amarillos, verdes, marrones
        float hue = Random.Range(0.08f, 0.35f);
        // Saturación media para colores naturales
        float saturation = Random.Range(0.3f, 0.6f);
        // Brillo medio
        float value = Random.Range(0.5f, 0.8f);
        
        return Color.HSVToRGB(hue, saturation, value);
    }
    
    /// <summary>
    /// Genera un color de agua (azules, turquesas).
    /// </summary>
    private static Color GenerateWaterColor()
    {
        // Hue entre 180-240 grados = azules y turquesas
        float hue = Random.Range(0.5f, 0.65f);
        // Saturación alta para colores vivos
        float saturation = Random.Range(0.5f, 0.8f);
        // Brillo medio-alto
        float value = Random.Range(0.6f, 0.9f);
        
        return Color.HSVToRGB(hue, saturation, value);
    }
    
    /// <summary>
    /// Genera un color de acento vibrante (cualquier tono).
    /// </summary>
    private static Color GenerateAccentColor()
    {
        // Cualquier hue es válido para acentos
        float hue = Random.Range(0f, 1f);
        // Saturación muy alta para que destaque
        float saturation = Random.Range(0.7f, 1f);
        // Brillo alto
        float value = Random.Range(0.8f, 1f);
        
        return Color.HSVToRGB(hue, saturation, value);
    }
    
    /// <summary>
    /// Genera un tinte global muy sutil para toda la isla.
    /// </summary>
    private static Color GenerateGlobalTint()
    {
        // Tinte muy cercano al blanco para no alterar mucho los colores
        float hue = Random.Range(0f, 1f);
        // Saturaciónbaja para que sea sutil y no cambie los colores ya definidos muy drasticamente
        float saturation = Random.Range(0.05f, 0.15f);
        // Brillo muy alto
        float value = Random.Range(0.95f, 1f);

        return Color.HSVToRGB(hue, saturation, value);
    }

    /// <summary>
    /// Genera un color visualmente distinto para cada semilla usando la proporción áurea.
    /// Garantiza colores bien distribuidos en el espectro para cualquier secuencia de enteros.
    /// </summary>
    public static Color GetDistinctColor(int seed)
    {
        // El multiplicador de la proporción áurea distribuye los tonos de forma óptima
        float hue = ((seed * 0.618033988749895f) % 1f + 1f) % 1f;
        return Color.HSVToRGB(hue, 0.75f, 0.85f);
    }
}