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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    // ── Paneles principales ───────────────────────────────────────────────────

    [Header("Paneles de navegación")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private GameObject confirmationPanel;

    // ── Referencias UI: Nueva Partida ────────────────────────────────────────

    [Header("Panel Nueva Partida")]
    [SerializeField] private TMP_InputField slotNameInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI errorNuevoText;

    // ── Referencias UI: Cargar Partida ───────────────────────────────────────

    [Header("Panel Cargar Partida")]
    // Transform del objeto "Content" del ScrollView
    [SerializeField] private Transform saveSlotsContainer;
    // Prefab con el componente SaveSlotUI
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private TextMeshProUGUI noSavesText;

    // ── Referencias UI: Confirmación de borrado ───────────────────────────────

    [Header("Panel Confirmación Borrar")]
    [SerializeField] private GameObject confirmDeletePanel;

    // ── Nombre de la escena de juego ─────────────────────────────────────────

    [Header("Configuración de escenas")]
    [SerializeField] private string gameSceneName = "SampleScene";

    // ── Estado interno ────────────────────────────────────────────────────────

    private int        _pendingDeleteSlotId = -1;
    private GameObject _pendingDeleteSlotGO;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        MostrarPanel(mainPanel);

        if (errorNuevoText != null)
            errorNuevoText.text = "";
    }

    // =========================================================================
    // BOTONES DEL PANEL PRINCIPAL
    // =========================================================================

    public void OnNuevaPartidaClicked()
    {
        slotNameInput.text   = "";
        playerNameInput.text = "";

        if (errorNuevoText != null)
            errorNuevoText.text = "";

        MostrarPanel(newGamePanel);
    }

    public void OnCargarPartidaClicked()
    {
        MostrarPanel(loadGamePanel);

        // Esperamos 1 frame para que Unity active el panel y procese
        // su RectTransform antes de instanciar los items del ScrollView.
        // Sin este delay el ContentSizeFitter calcula tamaños incorrectos.
        StartCoroutine(RefrescarListaPartidasDelayed());
    }

    // =========================================================================
    // PANEL NUEVA PARTIDA
    // =========================================================================

    public void OnConfirmarNuevaPartidaClicked()
    {
        string slotName   = slotNameInput.text.Trim();
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(playerName))
        {
            if (errorNuevoText != null)
                errorNuevoText.text = "Por favor, rellena todos los campos.";
            return;
        }

        SaveGameManager.Instance.NewGame(slotName, playerName);
        Debug.Log($"[Menu] Nueva partida creada: '{slotName}' para '{playerName}'. Cargando escena...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnCancelarNuevaPartidaClicked()
    {
        MostrarPanel(mainPanel);
    }

    // =========================================================================
    // PANEL CARGAR PARTIDA
    // =========================================================================

    /// Corrutina que espera 1 frame antes de llamar a RefrescarListaPartidas.
    /// Necesaria para que el panel esté completamente activo cuando se
    /// instancian los prefabs y se recalcula el layout.
    private IEnumerator RefrescarListaPartidasDelayed()
    {
        yield return null; // espera 1 frame
        RefrescarListaPartidas();
    }

    /// Consulta la BD y genera dinámicamente una fila (SaveSlotUI) por cada
    /// partida encontrada. Si no hay ninguna, muestra el texto noSavesText.
    private void RefrescarListaPartidas()
    {
        // ── Guardas ───────────────────────────────────────────────────────────
        if (saveSlotsContainer == null)
        {
            Debug.LogError("[Menu] saveSlotsContainer es NULL → asigna 'Content' en el Inspector.");
            return;
        }
        if (saveSlotPrefab == null)
        {
            Debug.LogError("[Menu] saveSlotPrefab es NULL → asigna 'SaveSlotItem' en el Inspector.");
            return;
        }
        
        if (SaveGameManager.Instance == null)
    {
        Debug.LogError("[Menu] SaveGameManager.Instance es NULL. " +
                       "Asegúrate de que el GameObject con SaveGameManager esté presente en la escena del MainMenu.");
        
        // Muestra el mensaje de "no hay partidas" como fallback
        if (noSavesText != null)
        {
            noSavesText.gameObject.SetActive(true);
            noSavesText.text = "Error: No se puede conectar con el sistema de guardado.";
        }
        return;
    }

        // ── Configuramos el VerticalLayoutGroup por código ────────────────────
        // Garantiza que childControlHeight = true para que el ContentSizeFitter
        // pueda leer la altura preferida de cada hijo y calcular el total correcto.
        VerticalLayoutGroup vlg = saveSlotsContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childControlWidth      = true;
            vlg.childControlHeight     = false;  
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing                = 8f;
            vlg.padding                = new RectOffset(10, 10, 10, 10);
        }

        // ── Limpiamos hijos anteriores ────────────────────────────────────────
        // Destruimos todos los hijos del Content para evitar duplicados
        foreach (Transform child in saveSlotsContainer)
            Destroy(child.gameObject);

        // ── Consultamos la base de datos ──────────────────────────────────────
        List<SaveSlotData> slots = SaveGameManager.Instance.GetAllSaveSlots();

        // ── Sin partidas ──────────────────────────────────────────────────────
        bool haySaves = slots.Count > 0;
        noSavesText?.gameObject.SetActive(!haySaves);
        if (!haySaves) return;

        // ── Instanciamos una fila por cada partida ────────────────────────────
        foreach (SaveSlotData slot in slots)
        {
            // Creamos el prefab como hijo del Content
            GameObject slotGO = Instantiate(saveSlotPrefab, saveSlotsContainer);

            // ── FORZAMOS la altura por código ─────────────────────────────────
            // El VerticalLayoutGroup necesita que cada hijo tenga un LayoutElement
            // con preferredHeight para calcular el tamaño total del Content.
            // Si el prefab no lo tiene en disco (o se perdió al editar en Play Mode),
            // lo añadimos y configuramos aquí directamente para garantizarlo siempre.
            LayoutElement le = slotGO.GetComponent<LayoutElement>();
            if (le == null)
            {
                le = slotGO.AddComponent<LayoutElement>();
            }
            le.minHeight       = 120f;
            le.preferredHeight = 120f; // altura fija de cada fila de partida
            le.preferredWidth  = -1f;  // -1 = ignorado, el ancho lo gestiona el padre

            // ── Comprobamos que el prefab tiene SaveSlotUI ────────────────────
            SaveSlot_UI slotUI = slotGO.GetComponent<SaveSlot_UI>();
            if (slotUI == null)
            {
                Debug.LogError($"[Menu] El prefab '{saveSlotPrefab.name}' no tiene SaveSlotUI. " +
                               "Asigna el prefab correcto en el Inspector.");
                continue;
            }

            // Pasamos los datos al componente de UI
            slotUI.Setup(slot, this);
        }

        // ── Forzamos recalculo del layout ─────────────────────────────────────
        // Con todos los hijos instanciados y con preferredHeight correcto,
        // forzamos que el ContentSizeFitter recalcule la altura del Content
        // en este mismo frame en lugar de esperar al siguiente.
        RectTransform contentRT = (RectTransform)saveSlotsContainer;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);
    }

    // =========================================================================
    // ACCIONES DE LOS SLOTS (llamadas desde SaveSlotUI)
    // =========================================================================

    /// Carga la partida seleccionada y cambia a la escena del juego.
    public void OnLoadSlotClicked(int slotId)
    {
        Debug.Log($"[Menu] Cargando partida con slot ID: {slotId}");
        SaveGameManager.Instance.LoadGame(slotId);
        SceneManager.LoadScene(gameSceneName);
    }


    /// Muestra el diálogo de confirmación antes de borrar.
    /// NO borra inmediatamente: esperamos confirmación del jugador.
    public void OnDeleteSlotClicked(int slotId, GameObject slotGameObject)
    {
        _pendingDeleteSlotId = slotId;
        _pendingDeleteSlotGO = slotGameObject;

        if (confirmDeletePanel != null)
        {
            //Hay que buscar el gameObject que se llama "Message" en el panel de advertencia de cancelación
            Transform textTransform = confirmDeletePanel.transform.Find("Message");

            if(textTransform != null)
            {
                //Se obtiene el componente TMP del objeto
                TextMeshProUGUI confirmText = textTransform.GetComponent<TextMeshProUGUI>();

                if(confirmText != null)
                {
                    confirmText.text = "¿Seguro que quieres borrar esta partida?\nEsta acción no se puede deshacer.";
                }
            }

            confirmationPanel.SetActive(true);
        }
    }

    /// Vuelve al panel principal desde el panel de carga.
    public void OnCancelarCargarClicked()
    {
        MostrarPanel(mainPanel);
    }

    // =========================================================================
    // PANEL CONFIRMACIÓN BORRAR
    // =========================================================================

    /// Confirma el borrado: elimina de la BD y refresca la lista.
    public void OnConfirmarBorrarClicked()
    {
        if (_pendingDeleteSlotId == -1) return;

        // Borramos de BD (ON DELETE CASCADE borra Player, Inventory, etc.)
        SaveGameManager.Instance.DeleteSave(_pendingDeleteSlotId);

        if (_pendingDeleteSlotGO != null)
            Destroy(_pendingDeleteSlotGO);

        Debug.Log($"[Menu] Slot {_pendingDeleteSlotId} borrado.");

        _pendingDeleteSlotId = -1;
        _pendingDeleteSlotGO = null;

        confirmationPanel.SetActive(false);

        // Refrescamos la lista completa para que el layout recalcule
        // la altura del Content con un hijo menos
        RefrescarListaPartidas();
    }

    /// Cancela el borrado sin tocar la base de datos.
    public void OnCancelarBorrarClicked()
    {
        _pendingDeleteSlotId = -1;
        _pendingDeleteSlotGO = null;
        confirmationPanel.SetActive(false);
    }

    // =========================================================================
    // UTILIDADES
    // =========================================================================

    /// Oculta todos los paneles y muestra solo el indicado.
    /// Centraliza la navegación entre paneles.
    private void MostrarPanel(GameObject panelToShow)
    {
        mainPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
        confirmationPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public void CerrarJuego()
    {
        Debug.Log("[Menu] Cerrando juego...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}