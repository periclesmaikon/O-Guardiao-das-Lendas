using UnityEngine;
using System.Collections;

public class CurupiraWhistleVisualizer : MonoBehaviour
{
    [Header("Componentes")]
    public AudioSource whistleAudio;
    public ParticleSystem visualParticles;
    
    [Header("Configurações")]
    [Tooltip("Tempo em segundos entre cada assovio")]
    public float delayBetweenWhistles = 5f;

    void Start()
    {
        // Se os componentes não foram arrastados no Inspector, o código tenta achá-los no mesmo objeto
        if (whistleAudio == null) whistleAudio = GetComponent<AudioSource>();
        if (visualParticles == null) visualParticles = GetComponentInChildren<ParticleSystem>();

        // Inicia a rotina de tocar o som e a partícula
        StartCoroutine(PlayWhistleRoutine());
    }

    IEnumerator PlayWhistleRoutine()
    {
        while (true)
        {
            // Toca o som de assovio
            if (whistleAudio != null) whistleAudio.Play();
            
            // Emite as partículas de som visual
            if (visualParticles != null) visualParticles.Play();

            // Espera os segundos definidos antes de tocar novamente
            yield return new WaitForSeconds(delayBetweenWhistles);
        }
    }
}