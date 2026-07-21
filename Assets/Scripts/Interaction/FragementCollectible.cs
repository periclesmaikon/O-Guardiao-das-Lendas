using UnityEngine;

public class FragmentCollectible : MonoBehaviour, IInteractable
{
    [Header("Configurações do Fragmento")]
    [Tooltip("ID único para este fragmento")]
    public string fragmentID;
    public string fragmentName = "Nome";
    public string interactPrompt = "Coletar Fragmento";

    private void Start()
    {
        CheckStatus();
    }

    private void OnEnable()
    {
        CheckStatus();
    }

    private void CheckStatus()
    {
        // 1. TRAVA PERMANENTE: Se a lenda já foi salva e consumiu essa peça, some para sempre.
        if (PlayerPrefs.GetInt("Consumed_" + fragmentID, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // 2. TRAVA TEMPORÁRIA: Se está no inventário agora (mas a lenda não foi salva ainda)
        if (PlayerPrefs.GetInt("Fragment_" + fragmentID, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        // Salva que o fragmento foi pego
        PlayerPrefs.SetInt("Fragment_" + fragmentID, 1);
        
        // Salva o nome dele em uma lista separada para a UI
        string currentCollected = PlayerPrefs.GetString("CollectedFragmentsList", "");
        if (!currentCollected.Contains(fragmentID))
        {
            // Guarda o ID e o Nome separados por dois-pontos e ponto-e-vírgula
            string updatedList = currentCollected + fragmentID + ":" + fragmentName + ";";
            PlayerPrefs.SetString("CollectedFragmentsList", updatedList);
        }

        PlayerPrefs.SetInt("Tutorial_PrimeiroFragmentoSaci", 1); //Para tutorial

        PlayerPrefs.Save();

        // Faz o objeto sumir da cena imediatamente
        gameObject.SetActive(false);
    }

    public string GetInteractPrompt()
    {
        return interactPrompt;
    }
}