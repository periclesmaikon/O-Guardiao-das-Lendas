using UnityEngine;

[System.Serializable]
public class LegendData
{
    [Header("Conexão com o Livro")]
    [Tooltip("Arraste o objeto que tem o LegendPuzzleManager desta lenda")]
    public LegendPuzzleManager puzzleManager;

    [Header("Quiz")]
    public string pergunta = "Sua pergunta aqui?";
    public string[] respostas = new string[3];
    public int indiceRespostaCorreta = 0;

    [Header("Insígnia")]
    public string nomeInsignia = "Nome da Lenda";
    [TextArea(3, 5)]
    public string descricaoInsignia = "Descrição da insígnia...";
    public Sprite spriteInsignia;
}

public class LegendInteraction : MonoBehaviour, IInteractable
{
    [Header("Configurações da Interação")]
    public string promptMessage = "Falar com a Lenda";
    public LegendQuizManager quizManager; 
    
    [Header("Dados Desta Lenda Específica")]
    public LegendData dadosDaLenda;

    public void Interact()
    {
        if (quizManager != null)
        {
            // O modelo 3D envia SEUS dados específicos para o gerenciador central
            quizManager.VerificarEstadoEInteragir(dadosDaLenda);
            PlayerPrefs.SetInt("Tutorial_SaciConcluido", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("O QuizManager não foi linkado no script da Lenda!");
        }
    }

    public string GetInteractPrompt()
    {
        return promptMessage;
    }
}