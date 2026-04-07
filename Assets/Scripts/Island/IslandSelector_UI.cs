using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menú que muestra todas las islas disponibles (partidas guardadas).
/// Permite al jugador viajar entre diferentes islas (cambiar entre partidas guardadas de forma visual) para simular el viaje
/// </summary>
public class IslandSelector_UI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject islandButtonPrefab;
    [SerializeField] private Transform islandListContainer;
    [SerializeField] private GameObject selectorPanel;
    
    private List<GameObject> _instantiatedButtons = new List<GameObject>();

    /// <summary>
/// Abre el selector y carga todas las islas disponibles.
/// </summary>
public void OpenSelector()
{
    // Se limpia la lista anterior por si acaso
    ClearIslandList();
    
    // VERIFICACIÓN: El prefab debe estar asignado
    if (islandButtonPrefab == null)
    {
        Debug.LogError("[IslandSelector] El prefab 'islandButtonPrefab' NO está asignado en el Inspector. " +
                      "No se pueden crear los botones.");
        return;
    }
    
    // VERIFICACIÓN: El contenedor debe existir
    if (islandListContainer == null)
    {
        Debug.LogError("[IslandSelector] El contenedor 'islandListContainer' NO está asignado.");
        return;
    }
    
    // Se obtienen todos los slots de guardado
    List<SaveSlotData> allSlots = SaveGameManager.Instance.GetAllSaveSlots();
    
    Debug.Log($"[IslandSelector] Cargando {allSlots.Count} islas disponibles");
    
    // Se crea un botón por cada isla
    foreach (SaveSlotData slot in allSlots)
    {
        GameObject buttonObj = Instantiate(islandButtonPrefab, islandListContainer);
        IslandButton_UI buttonUI = buttonObj.GetComponent<IslandButton_UI>();
        
        if (buttonUI != null)
        {
            buttonUI.Setup(slot, this);
            _instantiatedButtons.Add(buttonObj);
        }
        else
        {
            Debug.LogError($"[IslandSelector] El prefab no tiene el componente IslandButton_UI. " +
                          "Botón inválido para {slot.SlotName}");
        }
    }
    
    // Se muestra el panel
    if (selectorPanel != null)
        selectorPanel.SetActive(true);
}

    /// <summary>
    /// Cierra el selector de islas.
    /// </summary>
    public void CloseSelector()
    {
        if (selectorPanel != null)
            selectorPanel.SetActive(false);
        
        ClearIslandList();
    }

    /// <summary>
    /// Viaja a una isla específica (carga la partida guardada).
    /// </summary>
    /// <param name="slotData">Datos de la isla destino</param>
    public void TravelToIsland(SaveSlotData slotData)
    {
        Debug.Log($"[IslandSelector] Iniciando viaje a: {slotData.SlotName}");
        
        // Se carga la partida guardada
        SaveGameManager.Instance.LoadGame(slotData.Id);
        
        // Se carga la isla visualmente
        IslandManager.Instance.LoadIsland(slotData);
        
        // Se cierra el selector
        CloseSelector();
        
        Debug.Log($"[IslandSelector] Viaje completado a: {slotData.SlotName}");
    }

    /// <summary>
    /// Limpia todos los botones instanciados.
    /// </summary>
    private void ClearIslandList()
    {
        foreach (GameObject btn in _instantiatedButtons)
        {
            if (btn != null)
                Destroy(btn);
        }
        
        _instantiatedButtons.Clear();
    }
}