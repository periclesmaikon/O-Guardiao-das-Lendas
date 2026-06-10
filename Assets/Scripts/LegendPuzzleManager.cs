using UnityEngine;
using System.Collections.Generic;

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

    private bool isSolved = false;

    void Start()
    {
        // Ao carregar a página, verifica se o jogador já salvou essa lenda antes
        if (PlayerPrefs.GetInt("LegendSolved_" + legendName, 0) == 1)
        {
            LockPuzzleAsSolved(true);
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