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

    private float tempoExposto = 0f;
    private bool playerNaArea = false;
    private FirstPersonCamera playerCam;

    void Start()
    {
        playerCam = Object.FindFirstObjectByType<FirstPersonCamera>();
    }

    void Update()
    {
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
            GameResetManager.Instance.ResetGameProgress("Você foi hipnotizado pelo canto!");
        }
    }
}