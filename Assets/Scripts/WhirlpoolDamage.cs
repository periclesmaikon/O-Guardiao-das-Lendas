using UnityEngine;

public class WhirlpoolDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameResetManager.Instance != null)
            {
                GameResetManager.Instance.ResetGameProgress("Você foi atingido pelo redemoinho.");
            }
            else
            {
                Debug.LogError("GameResetManager não encontrado na cena!");
            }
        }
    }
}