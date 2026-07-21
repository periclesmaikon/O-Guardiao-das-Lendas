using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarParaMenu : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private string nomeDaCenaMenu = "Menu";

    [Header("Sistemas de UI (Arraste no Inspector)")]
    [SerializeField] private MapSystem mapSystem;
    [SerializeField] private BookSystem bookSystem;

    void Update()
    {
        // Verifica se a tecla ESC foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. Tenta fechar o Mapa primeiro
            if (mapSystem != null && mapSystem.isMapOpen)
            {
                mapSystem.ToggleMap();
                return; // Para a execução aqui, não vai para o menu
            }

            // 2. Se o Mapa não estava aberto, tenta fechar o Livro
            if (bookSystem != null && bookSystem.isBookOpen)
            {
                bookSystem.ToggleBook();
                return; // Para a execução aqui, não vai para o menu
            }

            // 3. Se nenhuma tela estava aberta, volta para o menu
            // Garante que o tempo do jogo não fique congelado ao voltar pro menu
            Time.timeScale = 1f; 

            MenuPrincipalManager.veioDeOutraTela = true;
            // Carrega a cena do menu
            SceneManager.LoadScene(nomeDaCenaMenu);
        }
    }
}