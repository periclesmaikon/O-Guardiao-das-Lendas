using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LegendPuzzleManager : MonoBehaviour
{
    [Header("Configurações da Lenda")]
    public string legendName = "Saci";
    public List<FragmentSlot> puzzleSlots; 
    
    [Header("Lenda e Perigos")]
    [Tooltip("Modelo 3D da Lenda")]
    public GameObject legendObject3D; 
    
    [Tooltip("Objeto pai que contém os perigos")]
    public GameObject perigosDaLenda;
    
    [Tooltip("IDs de TODOS os 6 fragmentos")]
    public List<string> allLegendFragmentIDs; 
    [Tooltip("Prefab da Etiqueta do Inventário")]
    public GameObject uiFragmentPrefab;

    [Header("Feedback Visual e Sonoro")]
    [Tooltip("Objeto de UI (Texto ou Painel) que diz 'LENDA SALVA'")]
    public GameObject lendaSalvaUI;
    [Tooltip("Efeito sonoro de acerto")]
    public AudioClip somAprovacao;
    [Tooltip("Componente AudioSource para tocar o som")]
    public AudioSource audioSource;
    [Tooltip("Tempo que a mensagem fica na tela (em segundos)")]
    public float tempoExibicaoUI = 2f;

    [Tooltip("Duração do Fade In/Out (em segundos)")]
    public float tempoFade = 0.5f;

    private bool isSolved = false;

    void Start()
    {
        // Ao carregar a página, verifica se o jogador já salvou essa lenda antes
        if (PlayerPrefs.GetInt("LegendSolved_" + legendName, 0) == 1)
        {
            LockPuzzleAsSolved(true);
        }
        if (lendaSalvaUI != null) lendaSalvaUI.SetActive(false);
    }

    private void OnDisable()
    {
        if (!isSolved)
        {
            ClearPuzzleSlots();
        }
    }

    public void CheckPuzzleCompletion()
    {
        if (isSolved) return; 

        foreach (var slot in puzzleSlots)
        {
            if (slot.currentFragment == null) return; 
        }

        bool isCorrect = true;
        foreach (var slot in puzzleSlots)
        {
            if (slot.currentFragment.fragmentID != slot.expectedFragmentID)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect) ResolvePuzzleSuccess();
        else ResolvePuzzleFailure();
    }

    private void ResolvePuzzleSuccess()
    {
        // 1. Marca no sistema que a lenda está salva
        PlayerPrefs.SetInt("LegendSolved_" + legendName, 1);
        
        // 2. Tira os 6 itens do inventário
        string currentInventory = PlayerPrefs.GetString("CollectedFragmentsList", "");
        List<string> inventoryList = new List<string>(currentInventory.Split(';'));

        foreach(string id in allLegendFragmentIDs)
        {
            PlayerPrefs.SetInt("Consumed_" + id, 1); 
            inventoryList.RemoveAll(item => item.StartsWith(id + ":"));
            PlayerPrefs.DeleteKey("Fragment_" + id);
        }

        PlayerPrefs.SetString("CollectedFragmentsList", string.Join(";", inventoryList));
        PlayerPrefs.Save();

        // 3. Atualiza o visual do inventário
        FragmentUIManager uiManager = Object.FindFirstObjectByType<FragmentUIManager>();
        if (uiManager != null) uiManager.UpdateFragmentListUI();

        // 4. Trava as peças na página permanentemente
        LockPuzzleAsSolved(false);

        TocarFeedbackDeSucesso();
    }

    private void TocarFeedbackDeSucesso()
    {
        Debug.Log("Puzzle resolvido com sucesso! Iniciando feedbacks com fade.");

        if (somAprovacao != null && audioSource != null)
        {
            audioSource.PlayOneShot(somAprovacao);
        }

        if (lendaSalvaUI != null)
        {
            lendaSalvaUI.SetActive(true);
            // Inicia a nova Coroutine que faz o Fade In, espera e faz o Fade Out
            StartCoroutine(AnimarFadeUI());
        }
    }

    private IEnumerator AnimarFadeUI()
    {
        // Pega o CanvasGroup. Se você esquecer de colocar na Unity, o script cria um sozinho!
        CanvasGroup cg = lendaSalvaUI.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = lendaSalvaUI.AddComponent<CanvasGroup>();
        }

        // --- FADE IN ---
        float tempoDecorrido = 0f;
        while (tempoDecorrido < tempoFade)
        {
            tempoDecorrido += Time.unscaledDeltaTime; 
            cg.alpha = Mathf.Clamp01(tempoDecorrido / tempoFade);
            yield return null;
        }
        cg.alpha = 1f;

        // --- TEMPO DE ESPERA NA TELA ---
        yield return new WaitForSecondsRealtime(tempoExibicaoUI);

        // --- FADE OUT ---
        tempoDecorrido = 0f;
        while (tempoDecorrido < tempoFade)
        {
            tempoDecorrido += Time.unscaledDeltaTime;
            cg.alpha = 1f - Mathf.Clamp01(tempoDecorrido / tempoFade);
            yield return null;
        }
        cg.alpha = 0f;

        lendaSalvaUI.SetActive(false);
    }

    private void LockPuzzleAsSolved(bool isLoadingSave)
    {
        isSolved = true;
        
        // Faz a lenda aparecer
        if (legendObject3D != null) legendObject3D.SetActive(true);
        
        if (perigosDaLenda != null) perigosDaLenda.SetActive(false);

        foreach (var slot in puzzleSlots)
        {
            if (isLoadingSave && uiFragmentPrefab != null)
            {
                GameObject novaEtiqueta = Instantiate(uiFragmentPrefab, slot.transform);
                DraggableFragment drag = novaEtiqueta.GetComponent<DraggableFragment>();
                drag.Setup(slot.expectedFragmentID, slot.expectedDisplayName);
                
                novaEtiqueta.transform.localPosition = Vector3.zero;
                slot.currentFragment = drag;
            }

            if (slot.currentFragment != null)
            {
                slot.currentFragment.enabled = false; 
                CanvasGroup cg = slot.currentFragment.GetComponent<CanvasGroup>();
                if (cg != null) cg.blocksRaycasts = false; 
            }
        }
    }

    private void ResolvePuzzleFailure()
    {
        ClearPuzzleSlots();

        BookSystem book = Object.FindFirstObjectByType<BookSystem>();
        if (book != null && book.isBookOpen)
        {
            book.ToggleBook();
        }

        if (GameResetManager.Instance != null)
        {
            GameResetManager.Instance.ResetGameProgress("Você reescreveu errado!");
        }
    }

    public void PrepararTentarNovamente()
    {
        isSolved = false;

        // 1. Remove o status de lenda resolvida no sistema
        PlayerPrefs.SetInt("LegendSolved_" + legendName, 0);

        // 2. Pega a string principal do inventário para limpar
        string currentInventory = PlayerPrefs.GetString("CollectedFragmentsList", "");
        List<string> inventoryList = new List<string>(currentInventory.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

        // 3. Limpa o save APENAS dos fragmentos desta lenda
        if (allLegendFragmentIDs != null)
        {
            foreach (string id in allLegendFragmentIDs)
            {
                PlayerPrefs.DeleteKey("Consumed_" + id);
                PlayerPrefs.DeleteKey("Fragment_" + id);
                PlayerPrefs.DeleteKey("Collected_" + id); 

                // Remove o item da lista geral do inventário
                inventoryList.RemoveAll(item => item.StartsWith(id + ":") || item == id);
            }
        }
        
        PlayerPrefs.SetString("CollectedFragmentsList", string.Join(";", inventoryList));
        PlayerPrefs.Save();

        ClearPuzzleSlots();

        FragmentUIManager uiManager = Object.FindFirstObjectByType<FragmentUIManager>();
        if (uiManager != null) uiManager.UpdateFragmentListUI();

        // 4. Busca TODOS os fragmentos na cena inteira
        FragmentCollectible[] todosOsFragmentos = Object.FindObjectsByType<FragmentCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        int fragmentosReativados = 0;

        foreach (FragmentCollectible frag in todosOsFragmentos)
        {
            // Verifica se a lista de IDs desta lenda contém o ID que está dentro do script daquele objeto
            if (allLegendFragmentIDs.Contains(frag.fragmentID))
            {
                frag.gameObject.SetActive(true); // Acorda o objeto!
                fragmentosReativados++;
                Debug.Log($"[PuzzleManager] Fragmento reativado com sucesso: {frag.fragmentName} (ID: {frag.fragmentID})");
            }
        }

        if (fragmentosReativados == 0)
        {
            Debug.LogWarning($"[PuzzleManager] AVISO: Nenhum fragmento da lenda {legendName} foi reativado. Verifique se a variável 'Fragment ID' no Inspector das peças bate com a lista da lenda.");
        }

        // 5. Esconde a Lenda e faz os perigos voltarem para o mapa
        if (legendObject3D != null) legendObject3D.SetActive(false);
        if (perigosDaLenda != null) perigosDaLenda.SetActive(true);

        Debug.Log($"[PuzzleManager] A lenda {legendName} foi resetada para o Tentar Novamente!");
    }
    private void ClearPuzzleSlots()
    {
        foreach (var slot in puzzleSlots)
        {
            if (slot.currentFragment != null)
            {
                Destroy(slot.currentFragment.gameObject);
                slot.currentFragment = null;
            }
        }
    }
}