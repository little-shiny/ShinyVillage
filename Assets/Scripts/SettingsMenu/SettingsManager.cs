using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using TMPro;

/// <summary>
/// Gestiona los ajustes del juego incluyendo volumen de música, brillo y guardado de partida.
/// Este manager utiliza PlayerPrefs para persistir los ajustes entre sesiones.
/// Se implementa como Singleton para facilitar el acceso desde cualquier parte del juego y el resto de los GameManagers
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
    [Tooltip("AudioSource que reproduce la música del juego")]
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Referencias de Post-Processing")]
    [Tooltip("Volume de Post-Processing para controlar el brillo")]
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

    // Referencia al jugador que se carga al inicio de la partida, no al abrir el panel
    private Player _cachedPlayer;
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
        // No se usa DontDestroyOnLoad porque este manager vive en la escena de juego
        
        // Se cargan las referencias del jugador AL INICIAR LA PARTIDA (en Awake, no en Start)
        // Awake se ejecuta aunque el GameObject esté desactivado
        LoadPlayerReferences();
    }

    private void Start()
    {
        // El panel inicia cerrado
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Se cargan los ajustes guardados
        LoadSettings();

        // Se configuran los listeners de los sliders
        SetupListeners();

        // Se validan las referencias
        ValidateReferences();
    }
    /// <summary>
    /// Carga las referencias del jugador AL INICIO de la partida, no al abrir el panel de settings.
    /// Este método se ejecuta una sola vez en Start().
    /// </summary>
    private void LoadPlayerReferences()
    {
        // Se busca el componente Player en la escena UNA SOLA VEZ al cargar la partida
        _cachedPlayer = FindObjectOfType<Player>();

        if (_cachedPlayer == null)
        {
            Debug.LogWarning("[SettingsManager] No se encontró el componente Player en la escena al cargar");
        }
        else
        {
            Debug.Log("[SettingsManager] Referencia al Player cargada correctamente al inicio de la partida");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // CONFIGURACIÓN INICIAL
    // ═══════════════════════════════════════════════════════════════


    /// Configura los listeners de los sliders para que respondan a cambios del usuario.
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

    /// Valida que todas las referencias necesarias estén asignadas en elinspector.
    private void ValidateReferences()
    {
        if (settingsPanel == null)
            Debug.LogError("[SettingsManager] Falta asignar el panel de ajustes en el Inspector");

        if (musicVolumeSlider == null)
            Debug.LogWarning("[SettingsManager] Falta asignar el slider de música");

        if (brightnessSlider == null)
            Debug.LogWarning("[SettingsManager] Falta asignar el slider de brillo");

        if (musicAudioSource == null)
            Debug.LogWarning("[SettingsManager] Falta asignar el AudioSource de música");

        if (postProcessVolume == null)
            Debug.LogWarning("[SettingsManager] Falta asignar el Volume de Post-Processing");
    }

    // ═══════════════════════════════════════════════════════════════
    // GESTIÓN DEL PANEL
    // ═══════════════════════════════════════════════════════════════

    /// Abre el panel de ajustes desde un boton
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("[SettingsManager] Panel de ajustes abierto");
        }
    }

    /// Cierra el panel de ajustes.
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("[SettingsManager] Panel de ajustes cerrado");
        }
    }


    /// Alterna la visibilidad del panel de ajustes.
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // CONTROL DE MÚSICA
    // ═══════════════════════════════════════════════════════════════

    /// Se llama cuando el usuario cambia el valor del slider de música.
    /// Actualiza el volumen del AudioSource y el texto mostrado.

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

    /// Actualiza el texto que muestra el porcentaje de volumen cuando cambia

    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            int percentage = Mathf.RoundToInt(value * 100);
            musicVolumeText.text = $"{percentage}%";
        }
    }

    /// Establece el volumen de la música directamente.

    public void SetMusicVolume(float volume)
    {
        // Se asegura que el valor esté en el rango válido
        volume = Mathf.Clamp01(volume);

        // Se actualiza el slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = volume;
        }

        // Se aplica el volumen (el listener del slider también lo hara pero lo hacemos aquí por si se llama 
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }

        UpdateMusicVolumeText(volume);
    }

    // ═══════════════════════════════════════════════════════════════
    // CONTROL DE BRILLO
    // ═══════════════════════════════════════════════════════════════


    /// Se llama cuando el usuario cambia el valor del slider de brillo.

    private void OnBrightnessChanged(float value)
    {
        // Se aplica el brillo 
        ApplyBrightness(value);

        // Se actualiza el texto del porcentaje
        UpdateBrightnessText(value);
    }


    /// Aplica el brillo al Volume de Post-Processing usando ColorAdjustments.

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


    /// Actualiza el texto que muestra el valor del brillo.

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


    /// Establece el brillo directamente.
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

    /// Guarda todos los ajustes actuales usando PlayerPrefs.
    /// Se puede llamar desde un botón "Guardar Ajustes" en la UI.
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

    /// Carga los ajustes guardados desde PlayerPrefs.
    /// Se llama automáticamente al iniciar.

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

    /// Guarda la partida actual usando el SaveGameManager.

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

        // Se obtiene la referencia al jugador (se busca solo la primera vez que se necesita)
        Player player = GetPlayerReference();
        
        if (player == null)
        {
            Debug.LogWarning("[SettingsManager] No se pudo obtener la referencia al Player. Probablemente estás en MainMenu donde no hay Player.");
            return;
        }

        // Se crea el PlayerData con los datos actuales del jugador
        PlayerData currentPlayer = new PlayerData
        {
            SlotId = SaveGameManager.Instance.CurrentSlotId,
            Name = "Jugador",
            Position = player.transform.position
        };

        // Se calcula el tiempo de juego actual
        float playTime = Time.time;

        // Se guarda la partida completa (posición + inventario)
        SaveGameManager.Instance.SaveCurrentGame(
            currentPlayer,
            player.inventory,
            playTime
        );

        Debug.Log($"[SettingsManager] Partida guardada exitosamente");
        Debug.Log($"  - Posición: {player.transform.position}");
        Debug.Log($"  - Tiempo jugado: {playTime:F2}s");
    }


    /// <summary>
    /// Obtiene la referencia al jugador (solo cuando se necesita).
    /// Si no está cacheada, la busca en la escena.
    /// </summary>
    private Player GetPlayerReference()
    {
        // Si ya está cacheada, se devuelve directamente
        if (_cachedPlayer != null)
        {
            return _cachedPlayer;
        }

        // Si no está cacheada, se busca en la escena
        _cachedPlayer = FindObjectOfType<Player>();

        // NO se muestra error aquí porque puede ser normal no encontrar Player en MainMenu
        if (_cachedPlayer != null)
        {
            Debug.Log("[SettingsManager] Referencia al Player obtenida correctamente");
        }

        return _cachedPlayer;
    }

    

    // ═══════════════════════════════════════════════════════════════
    // NAVEGACIÓN AL MENÚ PRINCIPAL
    // ═══════════════════════════════════════════════════════════════


    /// Carga la escena del menú principal.
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
}