using UnityEngine;

public class PersonWalk : MonoBehaviour
{
    [Header("Configurações de Local")]
    public GameObject inicio;
    public GameObject fim;

    [Header("Configurações de Movimento")]
    public float velocidade = 0.7f;

    [Header("Configurações de Áudio")]
    private AudioSource audioSource;

    private Transform alvoAtual;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (inicio != null && fim != null)
        {
            transform.position = inicio.transform.position;
            alvoAtual = fim.transform;
            
            // Inicia o som se houver um alvo
            PlayFootsteps();
        }
        else
        {
            Debug.LogWarning("Por favor, arraste os GameObjects de Início e Fim no Inspector!");
        }
    }

    void Update()
    {
        if (alvoAtual == null) 
        {
            StopFootsteps();
            return;
        }

        // Movimentação
        transform.position = Vector3.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);
        transform.LookAt(alvoAtual);

        // Verificação de chegada
        if (Vector3.Distance(transform.position, alvoAtual.position) < 0.1f)
        {
            if (alvoAtual == fim.transform)
                alvoAtual = inicio.transform;
            else
                alvoAtual = fim.transform;
        }

        if (!audioSource.isPlaying)
        {
            PlayFootsteps();
        }
    }

    void PlayFootsteps()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void StopFootsteps()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}