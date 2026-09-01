using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class LegendQuizManager : MonoBehaviour
{
    [Header("Configurações da Interface (UI)")]
    public GameObject painelQuiz;
    public TextMeshProUGUI textoPergunta;
    public Button[] botoesResposta;
    private TextMeshProUGUI[] textosDosBotoes;
    public GameObject miraCrosshair;

    [Header("Telas de Falha e Opções")]
    public GameObject painelErro;
    public Button botaoTentarNovamente;
    public Button botaoProximaLenda;

    [Header("Opções de Destino (Próxima Lenda)")]
    public GameObject painelDestinos;
    public Button botaoLago;
    public Button botaoFloresta;

    [Header("Tela de Vitória (Insígnia)")]
    public GameObject painelInsignia;
    public Image imagemInsigniaUI;
    public TextMeshProUGUI textoNomeInsigniaUI;
    public TextMeshProUGUI textoDescricaoInsigniaUI;
    public Button botaoContinuarInsignia;

    [Header("Caminho: Lago")]
    public GameObject paredeInvisivelLago;
    public GameObject cercaFechadaLago;
    public GameObject portaoAbertoLago;

    [Header("Caminho: Floresta")]
    public GameObject paredeInvisivelFloresta;
    public GameObject cercaFechadaFloresta;
    public GameObject portaoAbertoFloresta;

    [Header("Créditos (Fim de Jogo)")]
    public GameObject painelCreditos;
    [Tooltip("Arraste as 3 imagens das insígnias que ficam na tela de créditos")]
    public Image[] imagensInsigniasCreditos;
    [Tooltip("Escreva exatamente os nomes das insignias)")]
    public string[] nomesInsigniasCreditos;
    [Tooltip("Nome exato da cena do Menu Principal para o botão voltar")]
    public string nomeCenaMenu = "Menu";

    [Header("Controle do Jogador")]
    public MonoBehaviour playerMovementScript;
    private LegendData lendaAtual;

    void Start()
    {
        Debug.Log("[QuizManager] Inicializando gerenciador central...");

        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(false);

        CarregarCaminhosSalvos();
        ConfigurarBotoesIniciais();
    }

    private void ConfigurarBotoesIniciais()
    {
        textosDosBotoes = new TextMeshProUGUI[botoesResposta.Length];
        for (int i = 0; i < botoesResposta.Length; i++)
        {
            textosDosBotoes[i] = botoesResposta[i].GetComponentInChildren<TextMeshProUGUI>();
            int indexBotao = i;
            botoesResposta[indexBotao].onClick.RemoveAllListeners();
            botoesResposta[indexBotao].onClick.AddListener(() => AvaliarResposta(indexBotao));
        }

        if (botaoTentarNovamente != null)
        {
            botaoTentarNovamente.onClick.RemoveAllListeners();
            botaoTentarNovamente.onClick.AddListener(TentarNovamente);
        }

        if (botaoProximaLenda != null)
        {
            botaoProximaLenda.onClick.RemoveAllListeners();
            botaoProximaLenda.onClick.AddListener(TentarCaminhoOuCreditos);
        }

        if (botaoLago != null)
        {
            botaoLago.onClick.RemoveAllListeners();
            botaoLago.onClick.AddListener(IrParaLago);
        }

        if (botaoFloresta != null)
        {
            botaoFloresta.onClick.RemoveAllListeners();
            botaoFloresta.onClick.AddListener(IrParaFloresta);
        }

        Debug.Log("[QuizManager] Todos os botões mapeados e listeners limpos.");
    }

    public void VerificarEstadoEInteragir(LegendData dadosRecebidos)
    {
        if ((painelQuiz != null && painelQuiz.activeSelf) ||
            (painelErro != null && painelErro.activeSelf) ||
            (painelDestinos != null && painelDestinos.activeSelf) ||
            (painelInsignia != null && painelInsignia.activeSelf) ||
            (painelCreditos != null && painelCreditos.activeSelf))
        {
            Debug.LogWarning("[QuizManager] Bloqueando interação 3D: UI está aberta.");
            return; 
        }

        lendaAtual = dadosRecebidos;

        if (lendaAtual == null) return;

        int statusQuiz = PlayerPrefs.GetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 0);
        
        if (statusQuiz == 0) AbrirQuiz();
        else if (statusQuiz == 1) MostrarInsigniaSalva(true);
        else if (statusQuiz == 2) MostrarInsigniaSalva(false);
    }

    private void AbrirQuiz()
    {
        PausarJogoEExibirMouse(true);
       
        if (painelErro != null) painelErro.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(false);

        textoPergunta.text = lendaAtual.pergunta;
        for (int i = 0; i < lendaAtual.respostas.Length; i++)
        {
            if (i < textosDosBotoes.Length) textosDosBotoes[i].text = lendaAtual.respostas[i];
        }
        painelQuiz.SetActive(true);
    }

    private void AvaliarResposta(int indiceEscolhido)
    {
        if (indiceEscolhido == lendaAtual.indiceRespostaCorreta)
        {
            PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 1);
            PlayerPrefs.Save();

            if (imagemInsigniaUI != null)
            {
                imagemInsigniaUI.sprite = lendaAtual.spriteInsignia;
                imagemInsigniaUI.color = Color.white;
            }
            if (textoNomeInsigniaUI != null) textoNomeInsigniaUI.text = lendaAtual.nomeInsignia;
            if (textoDescricaoInsigniaUI != null) textoDescricaoInsigniaUI.text = lendaAtual.descricaoInsignia;

            if (botaoContinuarInsignia != null)
            {
                botaoContinuarInsignia.onClick.RemoveAllListeners();
                TextMeshProUGUI textoBotao = botaoContinuarInsignia.GetComponentInChildren<TextMeshProUGUI>();

                if (JogoEstaFinalizado())
                {
                    botaoContinuarInsignia.onClick.AddListener(AbrirCreditos); 
                    if (textoBotao != null) textoBotao.text = "FINALIZAR";
                }
                else
                {
                    botaoContinuarInsignia.onClick.AddListener(AbrirOpcoesDestino); 
                    if (textoBotao != null) textoBotao.text = "SALVAR PRÓXIMA LENDA";
                }
            }

            painelQuiz.SetActive(false);
            if (painelInsignia != null) painelInsignia.SetActive(true);
        }
        else
        {
            if (botaoProximaLenda != null)
            {
                TextMeshProUGUI textoBotaoErro = botaoProximaLenda.GetComponentInChildren<TextMeshProUGUI>();
                if (textoBotaoErro != null)
                {
                    if (JogoEstaFinalizado())
                    {
                        textoBotaoErro.text = "ENCERRAR";
                    }
                    else
                    {
                        textoBotaoErro.text = "SALVAR PRÓXIMA LENDA";
                    }
                }
            }

            painelQuiz.SetActive(false);
            if (painelErro != null) painelErro.SetActive(true);
        }
    }

    private void MostrarInsigniaSalva(bool acertouNoPassado)
    {
        PausarJogoEExibirMouse(true);

        if (imagemInsigniaUI != null)
        {
            imagemInsigniaUI.sprite = lendaAtual.spriteInsignia;
            imagemInsigniaUI.color = acertouNoPassado ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        if (textoNomeInsigniaUI != null) textoNomeInsigniaUI.text = lendaAtual.nomeInsignia;
        if (textoDescricaoInsigniaUI != null)
        {
            textoDescricaoInsigniaUI.text = acertouNoPassado ? lendaAtual.descricaoInsignia : "Você não conseguiu conquistar esta insígnia.";
        }

        if (botaoContinuarInsignia != null)
        {
            botaoContinuarInsignia.onClick.RemoveAllListeners();
            botaoContinuarInsignia.onClick.AddListener(FecharQuiz); 

            TextMeshProUGUI textoBotao = botaoContinuarInsignia.GetComponentInChildren<TextMeshProUGUI>();
            if (textoBotao != null) textoBotao.text = "FECHAR";
        }

        if (painelInsignia != null) painelInsignia.SetActive(true);
    }

    public void TentarNovamente() 
    {
        PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 0);
        PlayerPrefs.Save();

        if (lendaAtual.puzzleManager != null) lendaAtual.puzzleManager.PrepararTentarNovamente();
        
        FecharQuiz(); 
    }

    private void TentarCaminhoOuCreditos()
    {
        PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 2);
        PlayerPrefs.Save();

        if (JogoEstaFinalizado())
        {
            AbrirCreditos();
        }
        else
        {
            AbrirOpcoesDestino();
        }
    }

    public void AbrirOpcoesDestino() 
    {
        AtualizarBotoesDestino();

        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
       
        if (painelDestinos != null) painelDestinos.SetActive(true);
    }

    public void AbrirCreditos()
    {
        Debug.Log("[QuizManager] Fim de jogo detectado! Abrindo Créditos...");
        
        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);

        if (painelCreditos != null)
        {
            painelCreditos.SetActive(true);
            
            for (int i = 0; i < imagensInsigniasCreditos.Length; i++)
            {
                if (imagensInsigniasCreditos[i] != null && i < nomesInsigniasCreditos.Length)
                {
                    int status = PlayerPrefs.GetInt("StatusQuiz_" + nomesInsigniasCreditos[i], 0);
                    imagensInsigniasCreditos[i].color = (status == 1) ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);
                }
            }
        }
    }

    public void VoltarParaMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nomeCenaMenu);
    }

   private bool JogoEstaFinalizado()
    {
        return PlayerPrefs.GetInt("CaminhoAberto_Lago", 0) == 1 &&
               PlayerPrefs.GetInt("CaminhoAberto_Floresta", 0) == 1;
    }

    private void AtualizarBotoesDestino()
    {
        if (botaoLago != null) botaoLago.interactable = !(PlayerPrefs.GetInt("CaminhoAberto_Lago", 0) == 1);
        if (botaoFloresta != null) botaoFloresta.interactable = !(PlayerPrefs.GetInt("CaminhoAberto_Floresta", 0) == 1);
    }

    private void PausarJogoEExibirMouse(bool pausar)
    {
        Cursor.visible = pausar;
        Cursor.lockState = pausar ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pausar ? 0f : 1f;
        if (playerMovementScript != null) playerMovementScript.enabled = !pausar;
        if (miraCrosshair != null) miraCrosshair.SetActive(!pausar);
    }
    private void IrParaLago()
    {
        PlayerPrefs.SetInt("CaminhoAberto_Lago", 1);
        PlayerPrefs.Save();
        if (paredeInvisivelLago != null) paredeInvisivelLago.SetActive(false);
        if (cercaFechadaLago != null) cercaFechadaLago.SetActive(false);
        if (portaoAbertoLago != null) portaoAbertoLago.SetActive(true);
        FecharQuiz();
    }

    private void IrParaFloresta()
    {
        PlayerPrefs.SetInt("CaminhoAberto_Floresta", 1);
        PlayerPrefs.Save();
        if (paredeInvisivelFloresta != null) paredeInvisivelFloresta.SetActive(false);
        if (cercaFechadaFloresta != null) cercaFechadaFloresta.SetActive(false);
        if (portaoAbertoFloresta != null) portaoAbertoFloresta.SetActive(true);
        FecharQuiz();
    }

    private void CarregarCaminhosSalvos()
    {
        if (PlayerPrefs.GetInt("CaminhoAberto_Lago", 0) == 1)
        {
            if (paredeInvisivelLago != null) paredeInvisivelLago.SetActive(false);
            if (cercaFechadaLago != null) cercaFechadaLago.SetActive(false);
            if (portaoAbertoLago != null) portaoAbertoLago.SetActive(true);
        }
        if (PlayerPrefs.GetInt("CaminhoAberto_Floresta", 0) == 1)
        {
            if (paredeInvisivelFloresta != null) paredeInvisivelFloresta.SetActive(false);
            if (cercaFechadaFloresta != null) cercaFechadaFloresta.SetActive(false);
            if (portaoAbertoFloresta != null) portaoAbertoFloresta.SetActive(true);
        }
    }

    public void FecharQuiz()
    {
        PausarJogoEExibirMouse(false);
        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(false);
    }
}