// Componente que controla UN elemento de la lista de partidas guardadas.
// Se coloca en el prefab de "fila de partida" que se instancia dinámicamente
// en el menú de carga. Cada instancia recibe sus datos desde MainMenuManager.
//
//   SaveSlotItem (GameObject con este script)
//   ├── SlotNameText        (TextMeshProUGUI)
//   ├── PlayerNameText      (TextMeshProUGUI)
//   ├── LastSavedText       (TextMeshProUGUI)
//   ├── PlayTimeText        (TextMeshProUGUI)
//   ├── LoadButton          (Button)
//   └── DeleteButton        (Button)
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

    // ── Datos internos ────────────────────────────────────────────────────────

    // Guardamos los datos del slot para usarlos en los callbacks de los botones
    private SaveSlotData _slotData;

    // Referencia al menú principal para delegar las acciones (cargar/borrar)
    // Usamos una referencia en lugar de llamar a SaveGameManager directamente
    // para mantener toda la lógica de UI centralizada en MainMenuManager
    private MainMenuManager _menuManager;

    // ── Inicialización ────────────────────────────────────────────────────────

    /// Rellena este slot de UI con los datos de una partida guardada.
    /// Lo llama MainMenuManager al instanciar cada prefab.
    /// data >Datos del slot leídos desde la base de datos
    ///manager>Referencia al menú para delegar botones
    public void Setup(SaveSlotData data, MainMenuManager manager)
    {
        _slotData    = data;
        _menuManager = manager;

        // ── Rellenamos los textos ──────────────────────────────────────────────

        slotNameText.text   = data.SlotName;
        playerNameText.text = $"Player: {data.PlayerName}";

        // Formateamos la fecha en formato legible
        lastSavedText.text  = $"Last saved: {data.LastSaved:dd/MM/yyyy HH:mm}";

        // Convertimos segundos a horas y minutos
        playTimeText.text   = FormatPlayTime(data.PlayTime);

        // ── Asignamos los listeners a los botones ─────────────────────────────

        // Usamos lambda para capturar el slot específico de esta fila
        // Si usáramos OnClick directamente sin lambda, todas las filas  llamarían con el mismo valor por cierre de variable
        loadButton.onClick.AddListener(() => _menuManager.OnLoadSlotClicked(_slotData.Id));
        deleteButton.onClick.AddListener(() => _menuManager.OnDeleteSlotClicked(_slotData.Id, gameObject));
    }

    // ── Métodos auxiliares ────────────────────────────────────────────────────

    /// Convierte segundos totales en formato "Xh Ym" legible para el jugador.
    /// Ejemplo 7320s → "2h 2m" 
    private string FormatPlayTime(float totalSeconds)
    {
        // TimeSpan facilita la conversión de segundos a horas/minutos
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return $"Tiempo: {(int)time.TotalHours}h {time.Minutes}m";
    }
}