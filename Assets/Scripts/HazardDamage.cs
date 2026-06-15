using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    [Header("Configuração do Perigo")]
    [Tooltip("Frase que aparecerá na tela de Game Over")]
    public string mensagemDefeita = "Você foi atingido por um redemoinho!";

    private void OnTriggerEnter(Collider other)
    {
        // Se o jogador encostar no perigo
        if (other.CompareTag("Player"))
        {
            if (GameResetManager.Instance != null)
            {
                // Envia a frase customizada que você digitou no Inspector
                GameResetManager.Instance.ResetGameProgress(mensagemDefeita);
            }
        }
    }
}