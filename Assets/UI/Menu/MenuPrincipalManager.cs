using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [SerializeField] private string nomeDaCenaSitio;
    [SerializeField] private string nomeDaCenaCidade;

    [Header("Painéis")]
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelHQ;

    [Header("Componentes de UI")]
    [SerializeField] private Slider sliderSensibilidade;

    [SerializeField] private Button botaoNovoJogo; 
    public static bool veioDeOutraTela = false;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Atualiza a posição do slider ao iniciar o jogo (valor padrão 2.55f)
        if (sliderSensibilidade != null)
        {
            sliderSensibilidade.value = PlayerPrefs.GetFloat("Sensibilidade", 2.55f);
        }

        if (botaoNovoJogo != null)
        {
            if (veioDeOutraTela)
            {
                botaoNovoJogo.interactable = false;
            }
            else
            {
                botaoNovoJogo.interactable = true;
            }
        }
    }

    public void Jogar()
    {
        // 1. Verifica se as configurações existem e guarda os valores temporariamente
        bool tinhaConfigSom = PlayerPrefs.HasKey("SomAtivado");
        int valorSom = tinhaConfigSom ? PlayerPrefs.GetInt("SomAtivado") : 1;

        bool tinhaConfigSensibilidade = PlayerPrefs.HasKey("Sensibilidade");
        float valorSensibilidade = tinhaConfigSensibilidade ? PlayerPrefs.GetFloat("Sensibilidade") : 2.55f;

        // 2. Limpa TODOS os PlayerPrefs do jogo (reseta o progresso)
        PlayerPrefs.DeleteAll();

        // 3. Restaura apenas as variáveis de configuração (Som e Sensibilidade)
        if (tinhaConfigSom) PlayerPrefs.SetInt("SomAtivado", valorSom);
        if (tinhaConfigSensibilidade) PlayerPrefs.SetFloat("Sensibilidade", valorSensibilidade);

        PlayerPrefs.SetInt("Jogou", 1);
        PlayerPrefs.Save();

        painelMenuInicial.SetActive(false);
        painelHQ.SetActive(true);
    }

    public void Continuar()
    {
        if (PlayerPrefs.HasKey("Jogou"))
        {
            if (PlayerPrefs.HasKey("PegouOnibus") && PlayerPrefs.GetInt("PegouOnibus") == 1)
            {
                SceneManager.LoadScene(nomeDaCenaSitio);
            }
            else
            {
                SceneManager.LoadScene(nomeDaCenaCidade);
            }
        }
        else
        {
            Jogar();
        }
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void MudarSensibilidade(float valor)
    {
        PlayerPrefs.SetFloat("Sensibilidade", valor);
        PlayerPrefs.Save();
    }

    public void SairDoJogo()
    {
        // Salva qualquer alteração pendente no PlayerPrefs por garantia
        PlayerPrefs.Save();
        
        Debug.Log("Encerrando o jogo...");

        // Fecha o aplicativo compilado
        Application.Quit();

        // Para a execução dentro do Editor da Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}