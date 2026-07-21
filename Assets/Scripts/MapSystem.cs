using UnityEngine;
using UnityEngine.Rendering;

public class MapSystem : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject mapPanel; 

    [Header("Integração de Sistemas")]
    public BookSystem bookSystem; 

    [Header("Efeito de Desfoque (Volume Dedicado)")]
    public Volume uiBlurVolume;

    [Header("Controle do Jogador")]
    public MonoBehaviour playerMovementScript;

    public bool isMapOpen { get; private set; } = false;
    private bool hasMap = false;
    public bool wasMapOpened { get; private set; } = false;

    void Start()
    {
        if (mapPanel != null) mapPanel.SetActive(false);
        
        if (uiBlurVolume != null) uiBlurVolume.weight = 0f;

        hasMap = PlayerPrefs.GetInt("MapCollected", 0) == 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!hasMap) return; 

            if (!isMapOpen && bookSystem != null && bookSystem.isBookOpen)
            {
                return; 
            }

            ToggleMap();
        }
    }

    // Função nova que o BookSystem usa para checar o progresso do jogador
    public bool HasMapCollected()
    {
        return hasMap;
    }

    public void CollectMap()
    {
        hasMap = true;

        PlayerPrefs.SetInt("MapCollected", 1);
        PlayerPrefs.Save();
    }

    public void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        
        if (mapPanel != null) mapPanel.SetActive(isMapOpen);

        if (isMapOpen)
        {
            wasMapOpened = true;
            if (uiBlurVolume != null) uiBlurVolume.weight = 1f;
                        Time.timeScale = 0f; 
            if (playerMovementScript != null) playerMovementScript.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
            if (playerMovementScript != null) playerMovementScript.enabled = true;
        }
    }
}