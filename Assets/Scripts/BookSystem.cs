using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class BookSystem : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject bookPanel;
    public Image displayImage;
    
    [Header("Conteúdo")]
    public List<Sprite> bookPages; 
    
    [Header("Evento do Mapa (Queda)")]
    public GameObject mapPrefab;     
    public Transform dropPoint;      
    private bool hasDroppedMap = false; 

    [Header("Integração de Sistemas")]
    public MapSystem mapSystem; // Referência ao script do mapa

     [Header("Efeito de Desfoque (Volume Dedicado)")]
    public Volume uiBlurVolume;
    
    // Agora a variável pode ser lida por outros scripts
    public bool isBookOpen { get; private set; } = false; 
    private int currentPageIndex = 0;

    void Start()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
        if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Se o mapa estiver aberto, NÃO deixa abrir o livro
            if (!isBookOpen && mapSystem != null && mapSystem.isMapOpen)
            {
                return; // Interrompe o código aqui
            }
            
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        isBookOpen = !isBookOpen;
        if (bookPanel != null) bookPanel.SetActive(isBookOpen);

        if (isBookOpen)
        {
            currentPageIndex = 0; 
            UpdatePage();
            if (uiBlurVolume != null) uiBlurVolume.weight = 1f;
            //Time.timeScale = 0f; 
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (!hasDroppedMap)
            {
                DropMapItem();
            }
        }
        else
        {
            if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
            //Time.timeScale = 1f; 
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void DropMapItem()
    {
        if (mapPrefab != null && dropPoint != null)
        {
            Instantiate(mapPrefab, dropPoint.position, dropPoint.rotation);
            hasDroppedMap = true; 
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < bookPages.Count - 1)
        {
            currentPageIndex++;
            UpdatePage();
        }
        else
        {
            ToggleBook(); 
        }
    }

    void UpdatePage()
    {
        if (bookPages.Count > 0 && displayImage != null)
        {
            displayImage.sprite = bookPages[currentPageIndex];
        }
    }
}