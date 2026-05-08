using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float interactRange = 3f;
    public LayerMask interactableLayer;

    [Header("Configurações da Crosshair")]
    public RectTransform crosshairUI;
    public Vector3 normalSize = new Vector3(1f, 1f, 1f);
    public Vector3 hoverSize = new Vector3(1.5f, 1.5f, 1f);
    public float animationSpeed = 10f;

    private IInteractable currentTarget;
    public TextMeshProUGUI promptTextUI;

    void Update()
    {
        CheckForInteractable();
        AnimateCrosshair();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                string prompt = interactable.GetInteractPrompt();

                // Só considera como alvo se o texto NÃO for vazio
                if (!string.IsNullOrEmpty(prompt))
                {
                    currentTarget = interactable;
                    if (promptTextUI != null)
                    {
                        promptTextUI.text = prompt;
                        promptTextUI.gameObject.SetActive(true);
                    }
                    return; // Achou o alvo válido, sai da função
                }
            }
        }
        
        currentTarget = null;
        if (promptTextUI != null)
        {
            promptTextUI.text = "";
            promptTextUI.gameObject.SetActive(false);
        }
    }

    private void AnimateCrosshair()
    {
        if (crosshairUI != null)
        {
            Vector3 targetSize = currentTarget != null ? hoverSize : normalSize;
            crosshairUI.localScale = Vector3.Lerp(crosshairUI.localScale, targetSize, Time.deltaTime * animationSpeed);
        }
    }
}