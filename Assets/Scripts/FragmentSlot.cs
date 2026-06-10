using UnityEngine;
using UnityEngine.EventSystems;

public class FragmentSlot : MonoBehaviour, IDropHandler
{
    [Header("Configuração do Enigma")]
    [Tooltip("ID exato")]
    public string expectedFragmentID;
    [Tooltip("Nome exato")]
    public string expectedDisplayName; // Para o sistema carregar o save
    
    [Header("Estado Atual")]
    public DraggableFragment currentFragment;
    public LegendPuzzleManager puzzleManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableFragment fragment = droppedItem.GetComponent<DraggableFragment>();

        if (fragment != null && currentFragment == null)
        {
            fragment.transform.SetParent(transform);
            fragment.transform.localPosition = Vector3.zero;
            currentFragment = fragment;

            if (puzzleManager != null)
            {
                puzzleManager.CheckPuzzleCompletion();
            }
        }
    }
}