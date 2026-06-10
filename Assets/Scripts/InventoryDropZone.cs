using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableFragment fragment = droppedItem.GetComponent<DraggableFragment>();

        if (fragment != null)
        {
            fragment.transform.SetParent(transform);
        }
    }
}