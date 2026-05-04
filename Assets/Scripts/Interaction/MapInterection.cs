using UnityEngine;

public class MapInteraction : MonoBehaviour, IInteractable
{
    [Header("Configurações Básicas")]
    public string promptMessage = "Pegar Mapa";

    public void Interact()
    {
        // Procura o sistema do mapa ativo na cena
        MapSystem mapSystem = Object.FindFirstObjectByType<MapSystem>();
        
        if (mapSystem != null)
        {
            // Avisa o sistema que o jogador pegou o mapa
            mapSystem.CollectMap();
        }
        else
        {
            Debug.LogWarning("MapSystem não encontrado na cena! O mapa foi destruído mas não foi coletado.");
        }

        // Destrói o objeto 3D do chão
        Destroy(gameObject);
    }

    public string GetInteractPrompt()
    {
        return promptMessage;
    }
}