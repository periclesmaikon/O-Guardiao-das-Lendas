using UnityEngine;
using System.Collections; // Importante: Necessário para usar Corrotinas (IEnumerator)

[RequireComponent(typeof(AudioSource))]
public class DogInteraction : MonoBehaviour, IInteractable
{
    [Header("Configurações de Animação")]
    private Animator dogAnimator; 
    public string animationTriggerName = "Carinho"; 

    [Header("Configurações de Áudio")]
    public AudioClip barkSound;
    private AudioSource audioSource;
    
    // Nova variável: controla o tempo de espera em segundos
    [Tooltip("Tempo em segundos para esperar antes de tocar o latido")]
    public float barkDelay = 0.5f; 

    void Start()
    {
        dogAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    public void Interact()
    {
        // --- 1. Lógica da Animação (Acontece imediatamente) ---
        if (dogAnimator != null)
        {
            dogAnimator.SetTrigger(animationTriggerName);
        }
        else
        {
            Debug.LogWarning("O script de interação não encontrou um Animator no cachorro!");
        }

        // --- 2. Lógica do Áudio ---
        if (audioSource != null && barkSound != null)
        {
            // Em vez de tocar direto, iniciamos a contagem de tempo
            StartCoroutine(PlayBarkWithDelay()); 
        }
        else
        {
            Debug.LogWarning("Faltando o AudioClip de latido ou o componente AudioSource!");
        }
    }

    // --- 3. A Corrotina do Tempo de Espera ---
    private IEnumerator PlayBarkWithDelay()
    {
        // Pausa a execução DENTRO desta função pelo tempo configurado
        yield return new WaitForSeconds(barkDelay);
        
        // Depois que o tempo passar, o som é tocado
        audioSource.PlayOneShot(barkSound); 
    }
}