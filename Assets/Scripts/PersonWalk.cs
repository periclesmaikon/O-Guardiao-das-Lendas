using UnityEngine;

public class PersonWalk : MonoBehaviour
{
    [Header("Configurações de Local")]
    public GameObject inicio;
    public GameObject fim;

    [Header("Configurações de Movimento")]
    public float velocidade = 0.7f; // Velocidade que a pessoa vai andar

    // Variável para guardar para onde a pessoa está indo no momento
    private Transform alvoAtual;

    void Start()
    {
        // Prevenção de erros: garante que os objetos foram colocados no Inspector
        if (inicio != null && fim != null)
        {
            // Opcional: Garante que a pessoa comece exatamente na posição do "inicio"
            transform.position = inicio.transform.position;
            
            // Define que o primeiro destino é o "fim"
            alvoAtual = fim.transform;
        }
        else
        {
            Debug.LogWarning("Por favor, arraste os GameObjects de Início e Fim no Inspector!");
        }
    }

    void Update()
    {
        // Se não tiver alvo, não faz nada
        if (alvoAtual == null) return;

        // 1. Move a pessoa na direção do alvo atual
        transform.position = Vector3.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);
        transform.LookAt(alvoAtual);

        // 2. Verifica se a pessoa chegou ao destino
        // Usamos uma distância pequena (0.1f) em vez de "==" porque cálculos de física e posição nem sempre cravam no zero exato
        if (Vector3.Distance(transform.position, alvoAtual.position) < 0.1f)
        {
            // 3. Inverte o alvo: se chegou no fim, volta pro início. Se chegou no início, vai pro fim.
            if (alvoAtual == fim.transform)
            {
                alvoAtual = inicio.transform;
            }
            else
            {
                alvoAtual = fim.transform;
            }
        }
    }
}