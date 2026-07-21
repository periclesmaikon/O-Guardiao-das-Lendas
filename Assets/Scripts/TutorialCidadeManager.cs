using UnityEngine;
using TMPro; // Usando TextMeshPro para a UI

public class TutorialCidadeManager : MonoBehaviour
{
    [Header("UI do Tutorial")]
    public TextMeshProUGUI textoTutorial;

    void Update()
    {
        AtualizarTextoTutorial();
        ChecarInputsTutorial();
    }

    private void AtualizarTextoTutorial()
    {
        // 1. Verifica se o livro já foi aberto
        if (PlayerPrefs.GetInt("Tutorial_LivroAberto", 0) == 0)
        {
            textoTutorial.text = "Abra o livro pressionando L";
            return; // Interrompe a verificação para focar neste objetivo
        }

        // 2. Verifica se o item (mapa) foi coletado. 
        // Usando a chave "MapCollected"
        if (PlayerPrefs.GetInt("MapCollected", 0) == 0)
        {
            textoTutorial.text = "Algo caiu no chão, colete";
            return;
        }

        // 3. Verifica se o mapa já foi aberto após ser coletado
        if (PlayerPrefs.GetInt("Tutorial_MapaAberto", 0) == 0)
        {
            textoTutorial.text = "Abra o mapa pressionando M";
            return;
        }

        // 4. Último passo: ir para o sítio
        textoTutorial.text = "Pegue o ônibus para o sítio";
    }

    private void ChecarInputsTutorial()
    {
        // Simula o registro de que o livro foi aberto
        if (Input.GetKeyDown(KeyCode.L) && PlayerPrefs.GetInt("Tutorial_LivroAberto", 0) == 0)
        {
            PlayerPrefs.SetInt("Tutorial_LivroAberto", 1);
            PlayerPrefs.Save();
        }

        // Simula o registro de que o mapa foi aberto
        if (Input.GetKeyDown(KeyCode.M) && PlayerPrefs.GetInt("MapCollected", 0) == 1 && PlayerPrefs.GetInt("Tutorial_MapaAberto", 0) == 0)
        {
            PlayerPrefs.SetInt("Tutorial_MapaAberto", 1);
            PlayerPrefs.Save();
        }
    }
}