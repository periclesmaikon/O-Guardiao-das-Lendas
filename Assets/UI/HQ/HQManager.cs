using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HQManager : MonoBehaviour
{
    [Header("Componentes")]
    public RectTransform containerImagens;
    
    [Header("Configuração dos Quadrinhos")]
    [Tooltip("Adicione a posição X e Y de cada quadrinho na ordem de leitura.")]
    public Vector2[] posicoesQuadrinhos;
    public float velocidadeDeslize = 5f;

    [Header("Cena Destino")]
    public string nomeDaCenaDestino;

    private int indiceAtual = 0; 
    private bool estaMovendo = false;

    void OnEnable()
    {
        indiceAtual = 0;
        estaMovendo = false;
        
        if (containerImagens != null && posicoesQuadrinhos.Length > 0)
        {
            containerImagens.anchoredPosition = posicoesQuadrinhos[0];
        }
    }

    public void AvancarLeitura()
    {
        if (estaMovendo) return; 

        if (indiceAtual < posicoesQuadrinhos.Length - 1)
        {
            indiceAtual++;
            StartCoroutine(DeslizarPara(posicoesQuadrinhos[indiceAtual]));
        }
        else
        {
            FinalizarAto1();
        }
    }

    private IEnumerator DeslizarPara(Vector2 posicaoDestino)
    {
        estaMovendo = true;
        
        while (Vector2.Distance(containerImagens.anchoredPosition, posicaoDestino) > 1f)
        {
            containerImagens.anchoredPosition = Vector2.Lerp(containerImagens.anchoredPosition, posicaoDestino, Time.deltaTime * velocidadeDeslize);
            yield return null; 
        }
        
        containerImagens.anchoredPosition = posicaoDestino;
        estaMovendo = false;
    }

    private void FinalizarAto1()
    {
        SceneManager.LoadScene(nomeDaCenaDestino);
    }
}