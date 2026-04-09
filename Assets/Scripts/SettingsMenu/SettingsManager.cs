using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using TMPro;

/// <summary>
/// Gestiona los ajustes del juego incluyendo volumen de música, brillo y guardado de partida.
/// Este manager utiliza PlayerPrefs para persistir los ajustes entre sesiones.
/// Se implementa como Singleton para facilitar el acceso desde cualquier parte del juego.
/// Persiste entre escenas con DontDestroyOnLoad.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // ═══════════════════════════════════════════════════════════════
    // SINGLETON
    // ═══════════════════════════════════════════════════════════════
    
    public static SettingsManager Instance { get; private set; }

    // ═══════════════════════════════════════════════════════════════
    // REFERENCIAS DE UI
    // ═══════════════════════════════════════════════════════════════
    
    [Header("Panel de Ajustes")]
    [Tooltip("Panel principal que contiene todos los controles de ajustes")]
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Controles de Audio")]
    [Tooltip("Slider para controlar el volumen de la música")]
    [SerializeField] private Slider musicVolumeSlider;
    
    [Tooltip("Texto que muestra el porcentaje actual del volumen")]
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    
    [Header("Controles de Brillo")]
    [Tooltip("Slider para controlar el brillo de la pantalla")]
    [SerializeField] private Slider brightnessSlider;
    
    [Tooltip("Texto que muestra el porcentaje actual del brillo")]
    [SerializeField] private TextMeshProUGUI brightnessText;
    
    [Header("Referencias de Audio")]
    [Tooltip("AudioSource que reproduce la música del juego - se busca automáticamente si no está asignado")]
    [SerializeField] private AudioSource musicAudioSource;
    
    [Header("Referencias de Post-Processing")]
    [Tooltip("Volume de Post-Processing para controlar el brillo - se busca automáticamente si no está asignado")]
    [SerializeField] private Volume postProcessVolume;

    // ═══════════════════════════════════════════════════════════════
    // CLAVES DE PLAYERPREFS
    // ═══════════════════════════════════════════════════════════════
    
    // Estas constantes definen las claves bajo las cuales se guardan los ajustes
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string BRIGHTNESS_KEY = "Brightness";

    // ═══════════════════════════════════════════════════════════════
    // VALORES POR DEFECTO
    // ═══════════════════════════════════════════════════════════════
    
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;  // 70% de volumen por defecto
    private const float DEFAULT_BRIGHTNESS = 0f;      // 0 de post-exposure (brillo neutro)

    // ═══════════════════════════════════════════════════════════════
    // LIFECYCLE DE UNITY
    // ═══════════════════════════════════════════════════════════════

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[SettingsManager] Inicializado y persistente entre escenas.");
    }

    private void OnEnable()
    {
        // Suscribirse al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Desuscribirse del evento de carga de escena
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {


        // Buscar referencias en la escena actual
        FindReferencesInScene();

        // Se cargan los ajustes guardados
        LoadSettings();
        
        // Se configuran los listeners de los sliders
        SetupListeners();
        
        // Se validan las referencias
        ValidateReferences();
    }

    /// <summary>
    /// Se llama automáticamente cuando se carga una nueva escena.
    /// Busca las referencias necesarias en la nueva escena y aplica los ajustes guardados.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SettingsManager] Escena cargada: {scene.name}");
        
        // Buscar nuevas referencias en la escena recién cargada
        FindReferencesInScene();
    }

    // ═══════════════════════════════════════════════════════════════
    // CONFIGURACIÓN INICIAL
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Busca referencias de AudioSource y Volume en la escena actual.
    /// Se ejecuta automáticamente al cargar cada escena.
    /// </summary>
    private void FindReferencesInScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        // ─────────────────────────────────────────────────────────────
        // Buscar AudioSource de música si no está asignado manualmente
        // ─────────────────────────────────────────────────────────────
        if (musicAudioSource == null)
        {
            // Opción 1: Buscar por nombre "MusicManager"
            GameObject musicGO = GameObject.Find("MusicManager");
            if (musicGO != null)
            {
                musicAudioSource = musicGO.GetComponent<AudioSource>();
                if (musicAudioSource != null)
                {
                    Debug.Log($"[SettingsManager] AudioSource encontrado en {sceneName}: MusicManager");
                }
            }
            
            // Opción 2: Si no se encuentra por nombre, buscar cualquier AudioSource con Loop activado
            if (musicAudioSource == null)
            {
                AudioSource[] sources = FindObjectsOfType<AudioSource>();
                foreach (var source in sources)
                {
                    if (source.loop && source.clip != null)
                    {
                        musicAudioSource = source;
                        Debug.Log($"[SettingsManager] AudioSource encontrado en {sceneName}: {source.gameObject.name}");
                        break;
                    }
                }
            }
        }
        
        // ─────────────────────────────────────────────────────────────
        // Buscar Volume de Post-Processing si no está asignado
        // ─────────────────────────────────────────────────────────────
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
            if (postProcessVolume != null)
            {
                Debug.Log($"[SettingsManager] Volume encontrado en {sceneName}: {postProcessVolume.gameObject.name}");
            }
        }
        
        // ─────────────────────────────────────────────────────────────
        // Aplicar volumen guardado al AudioSource encontrado
        // ─────────────────────────────────────────────────────────────
        if (musicAudioSource != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
            musicAudioSource.volume = savedVolume;
            Debug.Log($"[SettingsManager] Volumen aplicado al AudioSource: {savedVolume}");
        }
        
        // ─────────────────────────────────────────────────────────────
        // Aplicar brillo guardado al Volume encontrado
        // ─────────────────────────────────────────────────────────────
        if (postProcessVolume != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, DEFAULT_BRIGHTNESS);
            ApplyBrightness(savedBrightness);
            Debug.Log($"[SettingsManager] Brillo aplicado al Volume: {savedBrightness}");
        }
    }

    /// <summary>
    /// Configura los listeners de los sliders para que respondan a cambios del usuario.
    /// </summary>
    private void SetupListeners()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }
    }

    /// <summary>
    /// Valida que todas las referencias necesarias estén asignadas.
    /// Adaptado para no dar warnings innecesarios según la escena.
    /// </summary>
    private void ValidateReferences()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        // Panel de settings solo es necesario en MainMenu
        if (settingsPanel == null && sceneName == "MainMenu")
        {
            Debug.LogWarning("[SettingsManager] Panel de ajustes no asignado en MainMenu. " +
                           "Asigna el SettingsPanel en el Inspector.");
        }

        // Sliders solo son necesarios en MainMenu
        if (musicVolumeSlider == null && sceneName == "MainMenu")
        {
            Debug.LogWarning("[SettingsManager] Slider de música no asignado en MainMenu.");
        }

        if (brightnessSlider == null && sceneName == "MainMenu")
        {
            Debug.LogWarning("[SettingsManager] Slider de brillo no asignado en MainMenu.");
        }

        // AudioSource debe existir en todas las escenas con música
        if (musicAudioSource == null)
        {
            Debug.LogWarning($"[SettingsManager] No se encontró AudioSource de música en {sceneName}. " +
                           "Crea un GameObject 'MusicManager' con un componente AudioSource.");
        }
        else
        {
            Debug.Log($"[SettingsManager] AudioSource OK en {sceneName}: {musicAudioSource.gameObject.name}");
        }

        // Volume solo da warning en MainMenu (opcional en otras escenas)
        if (postProcessVolume == null && sceneName == "MainMenu")
        {
            Debug.LogWarning("[SettingsManager] No se encontró Volume de Post-Processing en MainMenu.");
        }
        else if (postProcessVolume != null)
        {
            Debug.Log($"[SettingsManager] Volume OK en {sceneName}: {postProcessVolume.gameObject.name}");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // GESTIÓN DEL PANEL
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Abre el panel de ajustes desde un botón.
    /// </summary>
    public void OpenSettings()
    {
            if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            EnsurePanelPosition(); // 
            Debug.Log("[SettingsManager] Panel de ajustes abierto");
        }
        else
        {
            Debug.LogWarning("[SettingsManager] No se puede abrir el panel: settingsPanel es null");
        }
    }

    /// <summary>
    /// Cierra el panel de ajustes.
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("[SettingsManager] Panel de ajustes cerrado");
        }
    }

    /// <summary>
    /// Alterna la visibilidad del panel de ajustes.
    /// </summary>
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            
            string estado = isActive ? "cerrado" : "abierto";
            Debug.Log($"[SettingsManager] Panel de ajustes {estado}");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // CONTROL DE MÚSICA
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Se llama cuando el usuario cambia el valor del slider de música.
    /// Actualiza el volumen del AudioSource y el texto mostrado.
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        // Se aplica el volumen al AudioSource
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = value;
        }

        // Se actualiza el texto del porcentaje
        UpdateMusicVolumeText(value);
    }

    /// <summary>
    /// Actualiza el texto que muestra el porcentaje de volumen cuando cambia.
    /// </summary>
    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            int percentage = Mathf.RoundToInt(value * 100);
            musicVolumeText.text = $"{percentage}%";
        }
    }

    /// <summary>
    /// Establece el volumen de la música directamente.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        // Se asegura que el valor esté en el rango válido
        volume = Mathf.Clamp01(volume);
        
        // Se actualiza el slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = volume;
        }
        
        // Se aplica el volumen (el listener del slider también lo hará, pero lo hacemos aquí por seguridad)
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
        
        UpdateMusicVolumeText(volume);
    }

    // ═══════════════════════════════════════════════════════════════
    // CONTROL DE BRILLO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Se llama cuando el usuario cambia el valor del slider de brillo.
    /// </summary>
    private void OnBrightnessChanged(float value)
    {
        // Se aplica el brillo 
        ApplyBrightness(value);
        
        // Se actualiza el texto del porcentaje
        UpdateBrightnessText(value);
    }

    /// <summary>
    /// Aplica el brillo al Volume de Post-Processing usando ColorAdjustments.
    /// </summary>
    private void ApplyBrightness(float value)
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            // Se intenta obtener el efecto ColorAdjustments del perfil
            if (postProcessVolume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var colorAdjustments))
            {
                // Se modifica el post-exposure (brillo)
                colorAdjustments.postExposure.value = value;
            }
            else
            {
                Debug.LogWarning("[SettingsManager] No se encontró ColorAdjustments en el perfil de Post-Processing");
            }
        }
    }

    /// <summary>
    /// Actualiza el texto que muestra el valor del brillo.
    /// </summary>
    private void UpdateBrightnessText(float value)
    {
        if (brightnessText != null)
        {
            // Se muestra como porcentaje relativo al rango del slider
            // Si el slider va de -1 a 1, mostramos de 0% a 100%
            float normalizedValue = (value + 1f) / 2f; // Normaliza de [-1,1] a [0,1]
            int percentage = Mathf.RoundToInt(normalizedValue * 100);
            brightnessText.text = $"{percentage}%";
        }
    }

    /// <summary>
    /// Establece el brillo directamente.
    /// </summary>
    public void SetBrightness(float brightness)
    {
        // Se actualiza el slider
        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightness;
        }
        
        // Se aplica el brillo
        ApplyBrightness(brightness);
        UpdateBrightnessText(brightness);
    }

    // ═══════════════════════════════════════════════════════════════
    // GUARDADO Y CARGA DE AJUSTES
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Guarda todos los ajustes actuales usando PlayerPrefs.
    /// Se puede llamar desde un botón "Guardar Ajustes" en la UI.
    /// </summary>
    public void SaveSettings()
    {
        // Se guardan los valores de los sliders
        if (musicVolumeSlider != null)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);
        }

        if (brightnessSlider != null)
        {
            PlayerPrefs.SetFloat(BRIGHTNESS_KEY, brightnessSlider.value);
        }

        // Se asegura que los datos se escriban al disco
        PlayerPrefs.Save();
        
        Debug.Log("[SettingsManager] Ajustes guardados exitosamente");
    }

    /// <summary>
    /// Carga los ajustes guardados desde PlayerPrefs.
    /// Se llama automáticamente al iniciar.
    /// </summary>
    private void LoadSettings()
    {
        // Se cargan los ajustes o se usan valores por defecto si no existen
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
        float brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, DEFAULT_BRIGHTNESS);

        // Se aplican los ajustes cargados
        SetMusicVolume(musicVolume);
        SetBrightness(brightness);

        Debug.Log($"[SettingsManager] Ajustes cargados - Música: {musicVolume}, Brillo: {brightness}");
    }

    // ═══════════════════════════════════════════════════════════════
    // GUARDADO DE PARTIDA
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Guarda la partida actual usando el SaveGameManager.
    /// </summary>
    public void SaveGame()
    {
        // Se verifica que el SaveGameManager exista
        if (SaveGameManager.Instance == null)
        {
            Debug.LogError("[SettingsManager] SaveGameManager no está disponible");
            Debug.LogWarning("[SettingsManager] Asegúrate de iniciar el juego desde MainMenu, no directamente desde SampleScene");
            return;
        }

        // Se verifica que haya una partida activa
        if (SaveGameManager.Instance.CurrentSlotId == -1)
        {
            Debug.LogWarning("[SettingsManager] No hay ninguna partida activa para guardar");
            return;
        }

        // Se busca el componente Player en la escena
        Player player = FindObjectOfType<Player>();
        
        if (player == null)
        {
            Debug.LogError("[SettingsManager] No se encontró el componente Player en la escena");
            return;
        }

        // Se crea el PlayerData con los datos actuales
        PlayerData currentPlayer = new PlayerData
        {
            SlotId = SaveGameManager.Instance.CurrentSlotId,
            Name = "Jugador", // Nombre por defecto
            Position = player.transform.position // Posición actual del jugador
        };

        // Se calcula el tiempo de juego actual
        float playTime = Time.time;

        // Se guarda la partida
        SaveGameManager.Instance.SaveCurrentGame(currentPlayer, playTime);

        Debug.Log($"[SettingsManager] Partida guardada - Posición: {player.transform.position}");
    }

    // ═══════════════════════════════════════════════════════════════
    // NAVEGACIÓN AL MENÚ PRINCIPAL
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Carga la escena del menú principal.
    /// </summary>
    public void GoToMainMenu()
    {
        // Antes de cambiar de escena, se guardan los ajustes
        SaveSettings();
        
        // Se limpia la isla actual
        if (IslandManager.Instance != null)
        {
            IslandManager.Instance.ResetIslandVisuals();
        }

        // Se carga la escena del menú principal
        SceneManager.LoadScene("MainMenu");
        
        Debug.Log("[SettingsManager] Cargando menú principal...");
    }

    /// <summary>
/// Asegura que el panel de settings esté correctamente posicionado al abrirse.
/// </summary>
    private void EnsurePanelPosition()
    {
        if (settingsPanel != null)
        {
            RectTransform rectTransform = settingsPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Forzar recálculo del layout
                Canvas.ForceUpdateCanvases();
                
                // Asegurar que está dentro de la pantalla
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}