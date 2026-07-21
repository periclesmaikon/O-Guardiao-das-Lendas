using UnityEngine;
using TMPro;

public class TutorialSaciManager : MonoBehaviour
{
    [Header("UI do Tutorial")]
    public TextMeshProUGUI textoTutorial;

    void Update()
    {
        AtualizarTextoTutorialSaci();
        ChecarInputsTutorialSaci();
    }

    private void AtualizarTextoTutorialSaci()
    {
        // Verifica se o tutorial inteiro já foi finalizado
        if (PlayerPrefs.GetInt("Tutorial_SaciConcluido", 0) == 1)
        {
            // Desativa o texto
            textoTutorial.gameObject.SetActive(false);
            return;
        }

        // Garante que o texto esteja ativo enquanto o tutorial não acaba
        if (!textoTutorial.gameObject.activeSelf)
        {
            textoTutorial.gameObject.SetActive(true);
        }

        // 1. Verifica se pegou pelo menos um fragmento
        if (PlayerPrefs.GetInt("Tutorial_PrimeiroFragmentoSaci", 0) == 0)
        {
            textoTutorial.text = "Explore o sítio e colete fragmentos";
            return; 
        }

        // 2. Verifica se o livro foi aberto
        if (PlayerPrefs.GetInt("Tutorial_LivroSaciAberto", 0) == 0)
        {
            textoTutorial.text = "Adicione o fragmento ao livro pressionando L";
            return;
        }

        // 3. Verifica se a história do Saci foi completada
        if (PlayerPrefs.GetInt("LegendSolved_Saci", 0) == 0)
        {
            textoTutorial.text = "Complete a história corretamente";
            return;
        }

        // 4. Falar com o Saci
        textoTutorial.text = "Converse com a lenda";
    }

    private void ChecarInputsTutorialSaci()
    {
        if (Input.GetKeyDown(KeyCode.L) && 
            PlayerPrefs.GetInt("Tutorial_PrimeiroFragmentoSaci", 0) == 1 && 
            PlayerPrefs.GetInt("Tutorial_LivroSaciAberto", 0) == 0)
        {
            PlayerPrefs.SetInt("Tutorial_LivroSaciAberto", 1);
            PlayerPrefs.Save();
        }
    }
}