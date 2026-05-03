using UnityEngine;
using UnityEngine.UI; // Necessário para acessar a UI

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float interactRange = 3f; // Distância máxima para interagir
    public LayerMask interactableLayer; // Crie uma Layer "Interactable" na Unity e selecione aqui

    [Header("Configurações da Crosshair")]
    public RectTransform crosshairUI; // Arraste a sua imagem PNG da UI aqui
    public Vector3 normalSize = new Vector3(1f, 1f, 1f);
    public Vector3 hoverSize = new Vector3(1.5f, 1.5f, 1f); // Tamanho quando passar o mouse
    public float animationSpeed = 10f; // Velocidade de transição do tamanho

    private IInteractable currentTarget;

    void Update()
    {
        CheckForInteractable();
        AnimateCrosshair();

        // Se clicou com o botão esquerdo e tem um alvo válido
        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        // Cria um raio saindo do centro da câmera para frente
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Se o raio bater em algo que esteja na Layer "Interactable"
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            // Tenta pegar qualquer script nesse objeto que use a interface IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                currentTarget = interactable;
                return; // Achou o alvo, sai da função
            }
        }
        
        // Se não bateu em nada ou o objeto não tem script de interação, zera o alvo
        currentTarget = null;
    }

    private void AnimateCrosshair()
    {
        if (crosshairUI != null)
        {
            // Define o tamanho alvo dependendo se está olhando para um objeto interativo ou não
            Vector3 targetSize = currentTarget != null ? hoverSize : normalSize;
            
            // Faz uma transição suave entre o tamanho atual e o alvo
            crosshairUI.localScale = Vector3.Lerp(crosshairUI.localScale, targetSize, Time.deltaTime * animationSpeed);
        }
    }
}