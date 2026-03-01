// ============================================================
// MainMenuManager.cs
// Controlador de la pantalla principal del juego.
//
// FLUJO:
//   1. El jugador ve la pantalla principal con dos opciones:
//      "Nueva Partida" y "Cargar Partida"
//   2. Al pulsar "Nueva Partida" → aparece un panel con un InputField
//      para introducir el nombre del jugador y el nombre de la partida
//   3. Al pulsar "Cargar Partida" → aparece un panel con la lista de
//      partidas guardadas (instanciadas dinámicamente con SaveSlotUI)
//   4. Al pulsar "Cargar" en un slot → carga esa partida y cambia de escena
//   5. Al pulsar "Borrar" en un slot → elimina la partida de la BD y
//      refresca la lista
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    // ── Paneles principales ───────────────────────────────────────────────────

    [Header("Paneles de navegación")]
    // Panel que se ve al arrancar: título + botones principales
    [SerializeField] private GameObject mainPanel;

    // Panel para introducir datos de nueva partida
    [SerializeField] private GameObject newGamePanel;

    // Panel con el scroll de partidas guardadas
    [SerializeField] private GameObject loadGamePanel;

    // Diálogo de confirmación antes de borrar
    [SerializeField] private GameObject confirmationPanel;

    // ── Referencias UI: Nueva Partida ────────────────────────────────────────

    [Header("Panel Nueva Partida")]
    // Campo de texto donde el jugador escribe el nombre de la partida
    [SerializeField] private TMP_InputField slotNameInput;

    // Campo de texto donde el jugador escribe el nombre de su personaje
    [SerializeField] private TMP_InputField playerNameInput;

    // Texto de error que aparece si los campos están vacíos
    [SerializeField] private TextMeshProUGUI errorNuevoText;

    // ── Referencias UI: Cargar Partida ───────────────────────────────────────

    [Header("Panel Cargar Partida")]
    // Transform del Content del ScrollView donde se instancian las filas
    [SerializeField] private Transform saveSlotsContainer;

    // Prefab que representa una fila de partida (debe tener SaveSlotUI)
    [SerializeField] private GameObject saveSlotPrefab;

    // Texto que aparece cuando no hay ninguna partida guardada
    [SerializeField] private TextMeshProUGUI noSavesText;

    // ── Referencias UI: Confirmación de borrado ───────────────────────────────

    [Header("Panel Confirmación Borrar")]
    [SerializeField] private TextMeshProUGUI confirmDeleteText;

    // ── Nombre de la escena de juego ─────────────────────────────────────────

    [Header("Configuración de escenas")]
    // Nombre exacto de la escena del juego (debe estar en Build Settings)
    [SerializeField] private string gameSceneName = "SampleScene";

    // ── Estado interno ────────────────────────────────────────────────────────

    // ID del slot que se quiere borrar, guardado temporalmente mientras
    // se espera la confirmación del jugador
    private int _pendingDeleteSlotId = -1;

    // Referencia al GameObject del slot pendiente de borrar para
    // eliminarlo de la UI sin recargar toda la lista
    private GameObject _pendingDeleteSlotGO;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        // Estado inicial: solo el panel principal es visible
        MostrarPanel(mainPanel
);

        // Limpiamos el texto de error al iniciar
        if (errorNuevoText != null)
            errorNuevoText.text = "";
    }

    // =========================================================================
    // BOTONES DEL PANEL PRINCIPAL
    // =========================================================================


    /// Se llama al pulsar el botón "Nueva Partida".
    /// Muestra el formulario para introducir el nombre del jugador.
    /// Asignar este método al onClick del botón NuevaPartidaButton.
    public void OnNuevaPartidaClicked()
    {
        // Limpiamos los campos por si el jugador volvió atrás y ya había escrito algo
        slotNameInput.text   = "";
        playerNameInput.text = "";

        if (errorNuevoText != null)
            errorNuevoText.text = "";

        MostrarPanel(newGamePanel);
    }

    /// Se llama al pulsar el botón "Cargar Partida".
    /// Carga las partidas de la BD y las muestra en el ScrollView.
    /// Asignar este método al onClick del botón CargarPartidaButton.

    public void OnCargarPartidaClicked()
    {
        MostrarPanel(loadGamePanel);
        RefrescarListaPartidas();
    }

    // =========================================================================
    // PANEL NUEVA PARTIDA
    // =========================================================================


    /// Se llama al pulsar "Confirmar" en el panel de nueva partida.
    /// Valida los campos, crea la partida en BD y carga la escena del juego.

    public void OnConfirmarNuevaPartidaClicked()
    {
        // ── Validación: los campos no pueden estar vacíos ──────────────────────

        string slotName   = slotNameInput.text.Trim();   // .Trim() elimina espacios al inicio/final
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(playerName))
        {
            // Mostramos error en la UI en lugar de un Debug.LogError
            if (errorNuevoText != null)
                errorNuevoText.text = "Por favor, rellena todos los campos.";
            return;
        }

        // ── Creamos la partida a través de SaveGameManager ────────────────────

        // SaveGameManager.NewGame() → crea el slot en BD, guarda el jugador inicial
        // y deja CurrentSlotId listo para cuando el juego cargue
        SaveGameManager.Instance.NewGame(slotName, playerName);

        Debug.Log($"[Menu] Nueva partida creada: '{slotName}' para '{playerName}'. Cargando escena...");

        // ── Cargamos la escena del juego ──────────────────────────────────────
        SceneManager.LoadScene(gameSceneName);
    }


    /// Botón "Cancelar" en el panel de nueva partida.
    /// Vuelve al panel principal sin hacer nada.
    public void OnCancelarNuevaPartidaClicked()
    {
        MostrarPanel(mainPanel
);
    }

    // =========================================================================
    // PANEL CARGAR PARTIDA
    // =========================================================================


    /// Consulta la BD y genera dinámicamente una fila (SaveSlotUI) por cada
    /// partida encontrada. Si no hay ninguna, muestra el texto "noSavesText".

    private void RefrescarListaPartidas()
    {
        // ── Limpiamos los slots anteriores ────────────────────────────────────
        // Destruimos todos los hijos del Content para evitar duplicados
        foreach (Transform child in saveSlotsContainer)
            Destroy(child.gameObject);

        // ── Consultamos la base de datos ──────────────────────────────────────
        List<SaveSlotData> slots = SaveGameManager.Instance.GetAllSaveSlots();

        // ── Sin partidas: mostramos el aviso ──────────────────────────────────
        bool haySaves = slots.Count > 0;
        noSavesText.gameObject.SetActive(!haySaves);

        if (!haySaves) return;

        // ── Instanciamos una fila por cada partida ────────────────────────────
        foreach (SaveSlotData slot in slots)
        {
            // Creamos el prefab como hijo del Content del ScrollView
            GameObject slotGO = Instantiate(saveSlotPrefab, saveSlotsContainer);

            // Obtenemos el componente SaveSlotUI y le pasamos los datos
            SaveSlotUI slotUI = slotGO.GetComponent<SaveSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("[Menu] El prefab no tiene el componente SaveSlotUI.");
                continue;
            }

            // Setup rellena los textos y asigna los listeners de los botones
            slotUI.Setup(slot, this);
        }
    }


    /// Llamado desde SaveSlotUI cuando el jugador pulsa "Cargar" en un slot.
    /// Carga la partida en SaveGameManager y cambia a la escena del juego.
    /// slotId>ID del slot a cargar (viene de SaveSlotData.Id)
    public void OnLoadSlotClicked(int slotId)
    {
        Debug.Log($"[Menu] Cargando partida con slot ID: {slotId}");

        // LoadGame carga los datos del jugador (posición, etc.) y devuelve PlayerData
        // La escena del juego usará SaveGameManager.Instance.CurrentSlotId para saber
        // qué partida está activa
        SaveGameManager.Instance.LoadGame(slotId);

        SceneManager.LoadScene(gameSceneName);
    }

    /// Llamado desde SaveSlotUI al pulsar "Sobrescribir".
    /// Guarda el estado actual del juego en el slot indicado y recarga la lista.
    /// Solo funciona si hay una partida activa (CurrentSlotId != -1).
    public void OnOverwriteSlotClicked(int slotId)
    {
        if (SaveGameManager.Instance.CurrentSlotId == -1)
        {
            Debug.LogWarning("[Menu] No hay partida activa para sobrescribir.");
            return;
        }

        // Necesitamos los datos actuales del jugador para guardarlos
        // En un juego real estos vendrían del Player activo en escena.
        // Como estamos en el menú, usamos los datos que ya están en BD y solo actualizamos la fecha y el tiempo de juego.
        SaveGameManager.Instance.SaveCurrentGame(
            new PlayerData
            {
                SlotId   = slotId,
                Name     = "",      // Se rellenará con el jugador real
                Position = Vector3.zero
            },
            playTime: 0f            // Se rellenará con el tiempo real
        );

        Debug.Log($"[Menu] Slot {slotId} sobrescrito.");

        // Refrescamos la lista para que se vea la nueva fecha de guardado
        RefrescarListaPartidas();
    }

    /// Llamado desde SaveSlotUI cuando el jugador pulsa "Borrar" en un slot.
    /// Guarda el ID y el GameObject pendientes y muestra el diálogo de confirmación.
    /// NO borra inmediatamente: esperamos confirmación del jugador.

    /// slotId>ID del slot a borrar
    /// slotGameObject>El GameObject de la fila en la UI (para eliminarlo visualmente)
    public void OnDeleteSlotClicked(int slotId, GameObject slotGameObject)
    {
        // Guardamos temporalmente qué slot queremos borrar
        _pendingDeleteSlotId = slotId;
        _pendingDeleteSlotGO = slotGameObject;

        // Actualizamos el texto del diálogo de confirmación
        if (confirmDeleteText != null)
            confirmDeleteText.text = $"¿Seguro que quieres borrar esta partida?\nEsta acción no se puede deshacer.";

        // Mostramos el panel de confirmación encima del panel de carga
        confirmationPanel.SetActive(true);
    }

    /// /// Botón "Cancelar" en el panel de cargar partida.
    /// Vuelve al panel principal.
    public void OnCancelarCargarClicked()
    {
        MostrarPanel(mainPanel
);
    }

    // =========================================================================
    // PANEL CONFIRMACIÓN BORRAR
    // =========================================================================


    /// Confirma el borrado: elimina el slot de la BD y lo quita de la lista visible.
    /// Asignar al botón "Confirmar" del diálogo de confirmación.
    public void OnConfirmarBorrarClicked()
    {
        if (_pendingDeleteSlotId == -1) return; // Protección: no hay nada pendiente

        // ── Borramos de la base de datos ───────────────────────────────────────
        // DeleteSave usa ON DELETE CASCADE → borra también Player, Inventory, etc.
        SaveGameManager.Instance.DeleteSave(_pendingDeleteSlotId);

        // ── Eliminamos la fila visualmente sin recargar toda la lista ──────────
        if (_pendingDeleteSlotGO != null)
            Destroy(_pendingDeleteSlotGO);

        Debug.Log($"[Menu] Slot {_pendingDeleteSlotId} borrado.");

        // ── Limpiamos el estado pendiente ──────────────────────────────────────
        _pendingDeleteSlotId = -1;
        _pendingDeleteSlotGO = null;

        // Cerramos el diálogo de confirmación
        confirmationPanel.SetActive(false);

        // Si ya no quedan partidas, mostramos el aviso "Sin partidas guardadas"
        if (saveSlotsContainer.childCount == 0)
            noSavesText.gameObject.SetActive(true);
    }

    /// Cancela el borrado: cierra el diálogo sin tocar la base de datos.
    /// Asignar al botón "Cancelar" del diálogo de confirmación.
    public void OnCancelarBorrarClicked()
    {
        // Limpiamos el estado pendiente sin hacer nada en la BD
        _pendingDeleteSlotId = -1;
        _pendingDeleteSlotGO = null;

        confirmationPanel.SetActive(false);
    }

    // =========================================================================
    // UTILIDADES
    // =========================================================================

    /// Oculta todos los paneles y muestra solo el indicado.
    /// Centraliza la navegación entre paneles para evitar errores al olvidar desactivar algún panel manualmente.
    /// panelToShow  >El panel que debe quedar visible
    private void MostrarPanel(GameObject panelToShow)
    {
        // Desactivamos todos los paneles
        mainPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // Activamos solo el que queremos mostrar
        panelToShow.SetActive(true);
    }
}