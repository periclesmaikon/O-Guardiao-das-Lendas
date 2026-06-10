using UnityEngine;

public class TeleportInteraction : MonoBehaviour, IInteractable
{
    [Header("Configurações de Teleporte")]
    [Tooltip("Ponto de destino")]
    public Transform destinationPoint;
    public string promptText = "Entrar na casa";

    public void Interact()
    {
        // Busca o jogador na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && destinationPoint != null)
        {
            // Pega o componente CharacterController
            CharacterController cc = player.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                // Desativa o controle de colisão/física temporariamente
                cc.enabled = false;
                
                player.transform.position = destinationPoint.position;
                player.transform.rotation = destinationPoint.rotation;
                
                cc.enabled = true;
            }
            else
            {
                // Fallback caso não encontre o CharacterController
                player.transform.position = destinationPoint.position;
                player.transform.rotation = destinationPoint.rotation;
            }
        }
        else
        {
            Debug.LogWarning("Jogador não encontrado ou Ponto de Destino não configurado!");
        }
    }

    public string GetInteractPrompt()
    {
        return promptText;
    }
}