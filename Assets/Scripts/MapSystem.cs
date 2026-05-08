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

    public bool isMapOpen { get; private set; } = false;
    private bool hasMap = false;
    public bool wasMapOpened { get; private set; } = false;

    void Start()
    {
        if (mapPanel != null) mapPanel.SetActive(false);
        
        // Garante que o desfoque extra comece totalmente zerado
        if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
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

    public void CollectMap()
    {
        hasMap = true;
    }

    public void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        
        if (mapPanel != null) mapPanel.SetActive(isMapOpen);

        if (isMapOpen)
        {
            wasMapOpened = true;
            // Aplica o peso máximo no volume do desfoque
            if (uiBlurVolume != null) uiBlurVolume.weight = 1f;
        }
        else
        {
            // Zera o peso
            if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
        }
    }
}