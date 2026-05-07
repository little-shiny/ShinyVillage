using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Menú que muestra todas las islas disponibles (partidas guardadas) excepto la actual.
/// Permite al jugador viajar entre islas cargando colores, inventario y posición.
/// </summary>
public class IslandSelector_UI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject islandButtonPrefab;
    [SerializeField] private Transform islandListContainer;
    [SerializeField] private GameObject selectorPanel;

    [Header("Mensaje vacío")]
    [Tooltip("Texto que se muestra cuando no hay otras islas disponibles")]
    [SerializeField] private TextMeshProUGUI noOtherIslandsText;

    private List<GameObject> _instantiatedButtons = new List<GameObject>();

    /// <summary>
    /// Abre el selector y carga todas las islas disponibles excepto la isla actual.
    /// </summary>
    public void OpenSelector()
    {
        ClearIslandList();

        if (islandButtonPrefab == null)
        {
            Debug.LogError("[IslandSelector] 'islandButtonPrefab' no asignado en el Inspector.");
            return;
        }

        if (islandListContainer == null)
        {
            Debug.LogError("[IslandSelector] 'islandListContainer' no asignado en el Inspector.");
            return;
        }

        if (SaveGameManager.Instance == null)
        {
            Debug.LogError("[IslandSelector] SaveGameManager no disponible.");
            return;
        }

        int currentSlotId = SaveGameManager.Instance.CurrentSlotId;
        List<SaveSlotData> allSlots = SaveGameManager.Instance.GetAllSaveSlots();

        // Se excluye la isla actual de la lista
        List<SaveSlotData> otherSlots = allSlots.FindAll(s => s.Id != currentSlotId);

        Debug.Log($"[IslandSelector] {otherSlots.Count} islas disponibles (excluyendo la actual, id={currentSlotId})");

        bool hayOtrasIslas = otherSlots.Count > 0;

        if (noOtherIslandsText != null)
            noOtherIslandsText.gameObject.SetActive(!hayOtrasIslas);

        if (hayOtrasIslas)
        {
            foreach (SaveSlotData slot in otherSlots)
            {
                GameObject buttonObj = Instantiate(islandButtonPrefab, islandListContainer);
                if (buttonObj.TryGetComponent(out IslandButton_UI buttonUI))
                {
                    buttonUI.Setup(slot, this);
                    _instantiatedButtons.Add(buttonObj);
                }
                else
                {
                    Debug.LogError($"[IslandSelector] El prefab no tiene IslandButton_UI. Botón inválido para '{slot.SlotName}'.");
                    Destroy(buttonObj);
                }
            }
        }

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
    /// Viaja a una isla: cambia el slot activo y carga colores, inventario y posición.
    /// </summary>
    public void TravelToIsland(SaveSlotData slotData)
    {
        Debug.Log($"[IslandSelector] Viajando a: '{slotData.SlotName}' (seed={slotData.IslandSeed})");

        Player player = FindAnyObjectByType<Player>();

        // Guardar la partida actual antes de cambiar de slot
        if (player != null)
        {
            SaveGameManager.Instance.SaveCurrentGame(player.GetPLayerData(), player.inventory, 0f);
            Debug.Log("[IslandSelector] Partida actual guardada antes de viajar.");
        }

        // Cambia el slot activo y carga los visuales de la isla destino
        SaveGameManager.Instance.LoadGame(slotData.Id);

        // Aplica posición e inventario del jugador destino al Player de la escena
        if (player != null)
        {
            SaveGameManager.Instance.LoadPlayerPosition(player);
            SaveGameManager.Instance.LoadPlayerInventory(player.inventory);
            Debug.Log($"[IslandSelector] Posición e inventario del jugador '{slotData.PlayerName}' aplicados.");

            // Refrescar la UI del inventario para que muestre los items del nuevo slot
            Inventory_UI inventoryUI = FindAnyObjectByType<Inventory_UI>();
            if (inventoryUI != null)
                inventoryUI.Refresh();
        }
        else
        {
            Debug.LogError("[IslandSelector] No se encontró el componente Player en la escena.");
        }

        CloseSelector();
        Debug.Log($"[IslandSelector] Viaje completado a '{slotData.SlotName}'.");
    }

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
