using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BookSystem : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject bookPanel;
    public Image displayImage;
    
    [Header("Conteúdo")]
    public List<Sprite> bookPages; // Arraste a capa (index 0) e as páginas para cá
    
    private int currentPageIndex = 0;
    private bool isBookOpen = false;

    void Start()
    {
        bookPanel.SetActive(false);
    }

    void Update()
    {
        // Vamos ver se o Update está rodando mesmo
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("A tecla L foi detectada!"); // Vai aparecer no Console
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        isBookOpen = !isBookOpen;
        bookPanel.SetActive(isBookOpen);

        if (isBookOpen)
        {
            currentPageIndex = 0; // Sempre começa pela capa
            UpdatePage();
            //Time.timeScale = 0f; // Pausa o jogo ao ler
            
            // Libera e mostra o cursor para clicar no botão
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f; // Despausa o jogo
            
            // Esconde e trava o cursor de volta no jogo
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
            ToggleBook(); // Fecha o livro ao chegar na última página
        }
    }

    void UpdatePage()
    {
        if (bookPages.Count > 0)
        {
            displayImage.sprite = bookPages[currentPageIndex];
        }
    }
}