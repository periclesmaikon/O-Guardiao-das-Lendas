using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LegendQuizManager : MonoBehaviour
{
    [Header("Configurações da Interface (UI)")]
    public GameObject painelQuiz;
    public TextMeshProUGUI textoPergunta;
    public Button[] botoesResposta;
    private TextMeshProUGUI[] textosDosBotoes;

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

        CarregarCaminhosSalvos();
        ConfigurarBotoesIniciais();
    }

    private void ConfigurarBotoesIniciais()
    {
        // Configura botões de resposta do quiz de forma limpa
        textosDosBotoes = new TextMeshProUGUI[botoesResposta.Length];
        for (int i = 0; i < botoesResposta.Length; i++)
        {
            textosDosBotoes[i] = botoesResposta[i].GetComponentInChildren<TextMeshProUGUI>();
            int indexBotao = i;
            botoesResposta[indexBotao].onClick.RemoveAllListeners();
            botoesResposta[indexBotao].onClick.AddListener(() => AvaliarResposta(indexBotao));
        }

        // LIMPEZA CRÍTICA: Remove listeners antigos para evitar chamadas duplas ou fantasmas
        if (botaoTentarNovamente != null)
        {
            botaoTentarNovamente.onClick.RemoveAllListeners();
            botaoTentarNovamente.onClick.AddListener(TentarNovamente);
        }

        if (botaoProximaLenda != null)
        {
            botaoProximaLenda.onClick.RemoveAllListeners();
            botaoProximaLenda.onClick.AddListener(AbrirOpcoesDestino);
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
        // --- ESCUDO ANTI-CLIQUE FANTASMA ---
        // Se alguma tela já estiver na cara do jogador, ignora totalmente o clique no 3D
        if ((painelQuiz != null && painelQuiz.activeSelf) ||
            (painelErro != null && painelErro.activeSelf) ||
            (painelDestinos != null && painelDestinos.activeSelf) ||
            (painelInsignia != null && painelInsignia.activeSelf))
        {
            Debug.LogWarning("[QuizManager] Bloqueando interação 3D: O jogador clicou na UI e o clique atravessou!");
            return; // O "return" cancela a função aqui mesmo
        }

        // Salva a lenda clicada para usar nas outras funções
        lendaAtual = dadosRecebidos;

        if (lendaAtual == null)
        {
            Debug.LogError("[QuizManager] ERRO CRÍTICO: Dados recebidos da lenda estão nulos!");
            return;
        }

        // O PlayerPrefs usa o nome da insígnia como chave. Assim Saci e Iara não se misturam!
        int statusQuiz = PlayerPrefs.GetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 0);
        Debug.Log($"[QuizManager] Interagindo com: {lendaAtual.nomeInsignia}. Status do Save atual: {statusQuiz}");

        if (statusQuiz == 0)
        {
            AbrirQuiz();
        }
        else if (statusQuiz == 1)
        {
            MostrarInsigniaSalva(true);
        }
        else if (statusQuiz == 2)
        {
            MostrarInsigniaSalva(false);
        }
    }

    private void AbrirQuiz()
    {
        Debug.Log("[QuizManager] Abrindo tela principal do QUIZ.");
        PausarJogoEExibirMouse(true);
       
        if (painelErro != null) painelErro.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);

        textoPergunta.text = lendaAtual.pergunta;
        for (int i = 0; i < lendaAtual.respostas.Length; i++)
        {
            if (i < textosDosBotoes.Length) textosDosBotoes[i].text = lendaAtual.respostas[i];
        }
        painelQuiz.SetActive(true);
    }

    private void AvaliarResposta(int indiceEscolhido)
    {
        Debug.Log($"[QuizManager] Jogador escolheu a alternativa: {indiceEscolhido}. Resposta correta esperada: {lendaAtual.indiceRespostaCorreta}");

        if (indiceEscolhido == lendaAtual.indiceRespostaCorreta)
        {
            Debug.Log("<color=green>[QuizManager] Resposta CORRETA!</color>");
            PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 1);
            PlayerPrefs.Save();

            if (imagemInsigniaUI != null)
            {
                imagemInsigniaUI.sprite = lendaAtual.spriteInsignia;
                imagemInsigniaUI.color = Color.white;
            }
            if (textoNomeInsigniaUI != null) textoNomeInsigniaUI.text = lendaAtual.nomeInsignia;
            if (textoDescricaoInsigniaUI != null) textoDescricaoInsigniaUI.text = lendaAtual.descricaoInsignia;

            botaoContinuarInsignia.onClick.RemoveAllListeners();
            botaoContinuarInsignia.onClick.AddListener(AbrirOpcoesDestino);

            painelQuiz.SetActive(false);
            if (painelInsignia != null) painelInsignia.SetActive(true);
        }
        else
        {
            Debug.Log("<color=red>[QuizManager] Resposta ERRADA!</color>");
            painelQuiz.SetActive(false);
            if (painelErro != null) painelErro.SetActive(true);
        }
    }

    private void MostrarInsigniaSalva(bool acertouNoPassado)
    {
        Debug.Log($"[QuizManager] Mostrando insígnia já respondida anteriormente. Acertou? {acertouNoPassado}");
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

        botaoContinuarInsignia.onClick.RemoveAllListeners();
        botaoContinuarInsignia.onClick.AddListener(AbrirOpcoesDestino);

        if (painelInsignia != null) painelInsignia.SetActive(true);
    }

    public void TentarNovamente() 
    {
        Debug.Log("[QuizManager] Botão Tentar Novamente clicado. Resetando...");
        
        // Zera o fato de ter errado o quiz desta lenda
        PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 0);
        PlayerPrefs.Save();

        // Pede para o PuzzleManager devolver os itens e esconder a lenda
        if (lendaAtual.puzzleManager != null)
        {
            lendaAtual.puzzleManager.PrepararTentarNovamente();
        }
        else
        {
            Debug.LogWarning("[QuizManager] O PuzzleManager não foi linkado nos Dados da Lenda!");
        }

        FecharQuiz(); 
    }

    public void AbrirOpcoesDestino() // Mudei para public
    {
        Debug.Log("[QuizManager] Executando método AbrirOpcoesDestino().");
        if (painelErro != null && painelErro.activeSelf)
        {
            Debug.Log($"[QuizManager] Jogador optou por desistir/pular. Salvando status 2 para: {lendaAtual.nomeInsignia}");
            PlayerPrefs.SetInt("StatusQuiz_" + lendaAtual.nomeInsignia, 2);
            PlayerPrefs.Save();
        }

        AtualizarBotoesDestino();

        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
       
        if (painelDestinos != null)
        {
            painelDestinos.SetActive(true);
            Debug.Log("[QuizManager] SUCESSO: Painel de Destinos ativado.");
        }
        else
        {
            Debug.LogError("[QuizManager] ERRO: A variável 'painelDestinos' está nula no Inspector!");
        }

    }

    private void AtualizarBotoesDestino()
    {
        if (botaoLago != null)
        {
            // Lê o save: 1 significa que já está aberto
            bool lagoJaAberto = PlayerPrefs.GetInt("CaminhoAberto_Lago", 0) == 1;
            
            // Se já estiver aberto (true), interactable vira false (fica escuro e in-clicável)
            botaoLago.interactable = !lagoJaAberto; 
        }

        if (botaoFloresta != null)
        {
            bool florestaJaAberta = PlayerPrefs.GetInt("CaminhoAberto_Floresta", 0) == 1;
            botaoFloresta.interactable = !florestaJaAberta;
        }
    }

    private void PausarJogoEExibirMouse(bool pausar)
    {
        Cursor.visible = pausar;
        Cursor.lockState = pausar ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pausar ? 0f : 1f;
        if (playerMovementScript != null) playerMovementScript.enabled = !pausar;
    }

    private void IrParaLago()
    {
        Debug.Log("[QuizManager] Direcionando jogador para o Lago...");
        PlayerPrefs.SetInt("CaminhoAberto_Lago", 1);
        PlayerPrefs.Save();

        if (paredeInvisivelLago != null) paredeInvisivelLago.SetActive(false);
        if (cercaFechadaLago != null) cercaFechadaLago.SetActive(false);
        if (portaoAbertoLago != null) portaoAbertoLago.SetActive(true);

        FecharQuiz();
    }

    private void IrParaFloresta()
    {
        Debug.Log("[QuizManager] Direcionando jogador para a Floresta...");
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
        Debug.Log("[QuizManager] Fechando todas as interfaces do Quiz e resumindo jogo.");
        PausarJogoEExibirMouse(false);

        if (painelQuiz != null) painelQuiz.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
        if (painelDestinos != null) painelDestinos.SetActive(false);
        if (painelInsignia != null) painelInsignia.SetActive(false);
    }
}