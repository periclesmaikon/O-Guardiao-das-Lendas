using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{

    [Header("Configurações de Cena")]
    [SerializeField] private string nomeDaCenaSitio;
    [SerializeField] private string nomeDaCenaCidade;

    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelHQ;
    public void Jogar()
    {
        // 1. Verifica se a configuração de som existe e guarda o valor temporariamente
        bool tinhaConfigSom = PlayerPrefs.HasKey("SomAtivado");
        int valorSom = 1;
        if (tinhaConfigSom)
        {
            valorSom = PlayerPrefs.GetInt("SomAtivado");
        }

        // 2. Limpa TODOS os PlayerPrefs do jogo
        PlayerPrefs.DeleteAll();

        // 3. Restaura apenas a variável do som se ela já existia antes
        if (tinhaConfigSom)
        {
            PlayerPrefs.SetInt("SomAtivado", valorSom);
            PlayerPrefs.Save();
        }

        PlayerPrefs.SetInt("Jogou", 1);
        PlayerPrefs.Save();

        painelMenuInicial.SetActive(false);
        painelHQ.SetActive(true);
    }

    public void Continuar()
    {
        // Verifica se o player já jogou (já clicou em Novo Jogo alguma vez)
        if (PlayerPrefs.HasKey("Jogou"))
        {
            // Verifica se a chave existe E se o valor dela é 1 (pegou o ônibus)
            if (PlayerPrefs.HasKey("PegouOnibus") && PlayerPrefs.GetInt("PegouOnibus") == 1)
            {
                SceneManager.LoadScene(nomeDaCenaSitio);
            }
            else
            {
                // Se ele tem o save de "Jogou", mas não pegou o ônibus (valor é 0 ou a chave nem existe)
                SceneManager.LoadScene(nomeDaCenaCidade);
            }
        }
        else
        {
            // Caso a variável "Jogou" não exista (nunca jogou), roda a lógica do start
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
}
