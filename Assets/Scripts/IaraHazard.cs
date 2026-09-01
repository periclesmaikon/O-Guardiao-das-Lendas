using UnityEngine;
using UnityEngine.Rendering;

public class IaraHazard : MonoBehaviour
{
    [Header("Configurações da Nausea")]
    [Tooltip("Quantos segundos o jogador aguenta ouvir o canto antes do Game Over")]
    public float tempoParaGameOver = 10f; 
    [Tooltip("O grau máximo que a câmera vai entortar")]
    public float intensidadeNausea = 10f;
    
    [Header("Efeitos Visuais")]
    [Tooltip("Volume da Vinheta")]
    public Volume iaraVignetteVolume;

    [Header("Movimentação (Vai e Vem)")]
    [Tooltip("Ativar movimento do objeto?")]
    public bool seMove = true;
    [Tooltip("Velocidade do movimento")]
    public float velocidadeMovimento = 2f;
    [Tooltip("Distância que o objeto percorre para frente e para trás")]
    public float distanciaMovimento = 5f;

    private float tempoExposto = 0f;
    private bool playerNaArea = false;
    private FirstPersonCamera playerCam;
    
    // Variável para guardar a posição inicial do objeto no mundo
    private Vector3 posicaoInicial; 

    void Start()
    {
        playerCam = Object.FindFirstObjectByType<FirstPersonCamera>();
        
        // Salva o ponto de partida do objeto assim que o jogo começa
        posicaoInicial = transform.position; 
    }

    void Update()
    {
        // 1. Lógica de Movimento
        if (seMove)
        {
            // Mathf.Sin cria uma onda que vai de -1 a 1 ao longo do tempo.
            // Multiplicamos pela distância desejada para definir o limite do vai e vem.
            float deslocamento = Mathf.Sin(Time.time * velocidadeMovimento) * distanciaMovimento;
            
            // Movemos o objeto a partir da posição inicial
            transform.position = posicaoInicial + (transform.right * deslocamento);
        }

        // 2. Lógica de Nausea / Game Over
        if (playerNaArea)
        {
            tempoExposto += Time.deltaTime;
            float perigo = tempoExposto / tempoParaGameOver;

            if (playerCam != null)
            {
                playerCam.nauseaTilt = Mathf.Sin(Time.time * 4f) * (intensidadeNausea * perigo);
            }

            if (iaraVignetteVolume != null)
            {
                iaraVignetteVolume.weight = perigo;
            }

            if (tempoExposto >= tempoParaGameOver)
            {
                AplicarGameOver();
            }
        }
        else
        {
            if (tempoExposto > 0)
            {
                tempoExposto -= Time.deltaTime * 2f; 
                if (tempoExposto < 0) tempoExposto = 0;

                float perigo = tempoExposto / tempoParaGameOver;
                
                if (playerCam != null)
                {
                    playerCam.nauseaTilt = Mathf.Lerp(playerCam.nauseaTilt, 0, Time.deltaTime * 5f);
                }

                if (iaraVignetteVolume != null)
                {
                    iaraVignetteVolume.weight = Mathf.Lerp(iaraVignetteVolume.weight, 0, Time.deltaTime * 5f);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNaArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNaArea = false;
    }

    private void AplicarGameOver()
    {
        playerNaArea = false;
        tempoExposto = 0f;
        if (playerCam != null) playerCam.nauseaTilt = 0f; 
        
        // Zera a vinheta ao morrer
        if (iaraVignetteVolume != null) iaraVignetteVolume.weight = 0f;

        if (GameResetManager.Instance != null)
        {
            GameResetManager.Instance.ResetGameProgress("Você foi hipnotizado pelo canto da Iara!");
        }
    }
}