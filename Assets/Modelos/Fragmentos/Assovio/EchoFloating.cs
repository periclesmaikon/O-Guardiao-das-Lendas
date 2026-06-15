using UnityEngine;

public class EchoFloating : MonoBehaviour
{
    [Header("Configurações de Flutuação")]
    public float amplitude = 0.05f; // Altura máxima que a esfera sobe e desce
    public float velocidade = 0.5f; // Rapidez do movimento

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.localPosition;
    }

    void Update()
    {
        float novoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.localPosition = new Vector3(posicaoInicial.x, novoY, posicaoInicial.z);
    }
}