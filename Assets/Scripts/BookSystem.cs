using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;

public class BookSystem : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject bookPanel;
    
    [Header("Conteúdo Interativo (Páginas)")]
    [Tooltip("GameObjects das páginas")]
    public List<GameObject> bookPagesObjects; 
    
    [Header("Sistema de Fragmentos")]
    public TextMeshProUGUI uiTextList; 

    [Header("Evento do Mapa (Queda)")]
    public GameObject mapPrefab;     
    public Transform dropPoint;      
    private bool hasDroppedMap = false; 

    [Header("Integração de Sistemas")]
    public MapSystem mapSystem; 
    public Volume uiBlurVolume;

    [Header("Controle do Jogador")]
    public MonoBehaviour playerMovementScript;
    
    public bool isBookOpen { get; private set; } = false; 
    private int currentPageIndex = 0;

    void Start()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
        if (uiBlurVolume != null) uiBlurVolume.weight = 0f;
        hasDroppedMap = PlayerPrefs.GetInt("MapDropped", 0) == 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isBookOpen && mapSystem != null && mapSystem.isMapOpen) return;
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
            Time.timeScale = 0f; 
            if (playerMovementScript != null) playerMovementScript.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (!hasDroppedMap) DropMapItem();
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

    private void DropMapItem()
{
    if (mapPrefab != null && dropPoint != null)
    {
        Instantiate(mapPrefab, dropPoint.position, dropPoint.rotation);

        hasDroppedMap = true;

        PlayerPrefs.SetInt("MapDropped", 1);
        PlayerPrefs.Save();
    }
}

    public void NextPage()
    {
        if (currentPageIndex < bookPagesObjects.Count - 1)
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
        foreach (GameObject page in bookPagesObjects)
        {
            if (page != null) page.SetActive(false);
        }

        if (bookPagesObjects.Count > 0 && currentPageIndex < bookPagesObjects.Count)
        {
            if (bookPagesObjects[currentPageIndex] != null)
            {
                bookPagesObjects[currentPageIndex].SetActive(true);
            }
        }
    }
}