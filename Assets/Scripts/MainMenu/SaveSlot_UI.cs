// Componente que controla UN elemento de la lista de partidas guardadas.
// Se coloca en el prefab de "fila de partida" que se instancia dinámicamente
// en el menú de carga. Cada instancia recibe sus datos desde MainMenuManager.
//
//    SaveSlotItem
//   ├── SlotNameText        (TextMeshProUGUI)
//   ├── PlayerNameText      (TextMeshProUGUI)
//   ├── LastSavedText       (TextMeshProUGUI)
//   ├── PlayTimeText        (TextMeshProUGUI)
//   └── ButtonsPanel
//       ├── LoadButton      (Button)
//       ├── OverwriteButton (Button)
//       └── DeleteButton    (Button)
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    // Asignaciones de los elementos del prefab
    // Se asignan desde el Inspector arrastrando cada objeto del prefab

    [Header("Info texts")]
    [SerializeField] private TextMeshProUGUI slotNameText;    // Ej: Partida 1
    [SerializeField] private TextMeshProUGUI playerNameText;  // Ej: jugador: Link
    [SerializeField] private TextMeshProUGUI lastSavedText;   // Ej: Guardado: 01/03/2026 14:32
    [SerializeField] private TextMeshProUGUI playTimeText;    // Ej: Tiempo: 2h 15m

    [Header("Buttons")]
    [SerializeField] private Button loadButton;               
    [SerializeField] private Button deleteButton;       
    [SerializeField] private Button overwriteButton;         

    // ── Datos internos ────────────────────────────────────────────────────────

    // Guardamos los datos del slot para usarlos en los callbacks de los botones
    private SaveSlotData _slotData;

    // Referencia al menú principal para delegar las acciones (cargar/borrar)
    // Usamos una referencia en lugar de llamar a SaveGameManager directamente
    // para mantener toda la lógica de UI centralizada en MainMenuManager
    private MainMenuManager _menuManager;

    // ── Inicialización ────────────────────────────────────────────────────────
    private void Awake()
    {
        // Auto-asignamos referencias si no están asignadas en el Inspector
        if (slotNameText    == null) slotNameText    = transform.Find("SlotNameText")   ?.GetComponent<TextMeshProUGUI>();
        if (playerNameText  == null) playerNameText  = transform.Find("PlayerNameText") ?.GetComponent<TextMeshProUGUI>();
        if (lastSavedText   == null) lastSavedText   = transform.Find("LastSavedText")  ?.GetComponent<TextMeshProUGUI>();
        if (playTimeText    == null) playTimeText    = transform.Find("PlayTimeText")   ?.GetComponent<TextMeshProUGUI>();
        if (loadButton      == null) loadButton      = transform.Find("LoadButton")     ?.GetComponent<Button>();
        if (deleteButton    == null) deleteButton    = transform.Find("DeleteButton")   ?.GetComponent<Button>();
        if (overwriteButton == null) overwriteButton = transform.Find("OverwriteButton")?.GetComponent<Button>();

        // Corrige escala 0 en hijos (bug del prefab)
        foreach (Transform t in GetComponentsInChildren<Transform>(includeInactive: true))
        {
            if (t.localScale == Vector3.zero)
            {
                t.localScale = Vector3.one;
                Debug.LogWarning($"[SaveSlotUI] Escala 0 corregida en '{t.name}'");
            }
        }
    }

    public void Setup(SaveSlotData data, MainMenuManager manager)
{
    if (data == null)
    {
        Debug.LogError("[SaveSlotUI] Setup recibió SaveSlotData NULL.");
        return;
    }

    _slotData = data;
    _menuManager = manager;

    // 1) Textos: deben renderizarse aunque falten botones
    bool faltanTextos = slotNameText == null || playerNameText == null || lastSavedText == null || playTimeText == null;
    if (faltanTextos)
    {
        Debug.LogError("[SaveSlotUI] Faltan referencias de texto:\n" +
                       $"  slotNameText={slotNameText}\n" +
                       $"  playerNameText={playerNameText}\n" +
                       $"  lastSavedText={lastSavedText}\n" +
                       $"  playTimeText={playTimeText}");
        return; // sin textos no hay nada útil que mostrar
    }

    slotNameText.text   = string.IsNullOrWhiteSpace(data.SlotName) ? "(Sin nombre)" : data.SlotName;
    playerNameText.text = $"Player: {(string.IsNullOrWhiteSpace(data.PlayerName) ? "(Sin jugador)" : data.PlayerName)}";
    lastSavedText.text  = $"Last saved: {data.LastSaved:dd/MM/yyyy HH:mm}";
    playTimeText.text   = FormatPlayTime(data.PlayTime);

    // 2) Botones: si faltan, NO romper visualización de textos
    bool faltanBotones = loadButton == null || deleteButton == null || overwriteButton == null;
    if (faltanBotones)
    {
        Debug.LogWarning("[SaveSlotUI] Faltan botones; se muestran textos pero no habrá acciones:\n" +
                         $"  loadButton={loadButton}\n" +
                         $"  deleteButton={deleteButton}\n" +
                         $"  overwriteButton={overwriteButton}");
        return;
    }

    // Limpieza defensiva para evitar listeners duplicados al refrescar lista
    loadButton.onClick.RemoveAllListeners();
    deleteButton.onClick.RemoveAllListeners();
    overwriteButton.onClick.RemoveAllListeners();

    loadButton.onClick.AddListener(() => _menuManager.OnLoadSlotClicked(_slotData.Id));
    deleteButton.onClick.AddListener(() => _menuManager.OnDeleteSlotClicked(_slotData.Id, gameObject));
    overwriteButton.onClick.AddListener(() => _menuManager.OnOverwriteSlotClicked(_slotData.Id));

    bool hayPartidaActiva = SaveGameManager.Instance != null && SaveGameManager.Instance.CurrentSlotId != -1;
    overwriteButton.gameObject.SetActive(hayPartidaActiva);
}

    // ── Métodos auxiliares ────────────────────────────────────────────────────

    /// Convierte segundos totales en formato "Xh Ym" legible para el jugador.
    /// Ejemplo 7320s → "2h 2m" 
    private string FormatPlayTime(float totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return $"Tiempo: {(int)time.TotalHours}h {time.Minutes}m";
    }
}