using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableFragment : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Dados do Fragmento")]
    public string fragmentID;
    
    private Transform originalParent;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(string id, string displayName)
    {
        fragmentID = id;
        GetComponentInChildren<TextMeshProUGUI>().text = displayName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        
        //Se o fragmento estava em um slot, avisa que ele saiu
        FragmentSlot slot = GetComponentInParent<FragmentSlot>();
        if (slot != null && slot.currentFragment == this)
        {
            slot.currentFragment = null;
        }
        
        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        
        // Se soltou fora de um slot válido, ele volta para onde estava
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            
            // Se voltou para o slot anterior, avisa o slot que a peça retornou
            FragmentSlot slot = originalParent.GetComponent<FragmentSlot>();
            if (slot != null)
            {
                slot.currentFragment = this;
            }
        }
    }
}