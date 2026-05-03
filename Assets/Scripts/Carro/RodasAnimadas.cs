using UnityEngine;
using UnityEngine.AI;

public class RodasAnimadas : MonoBehaviour
{
    private NavMeshAgent agent;
    private float variacaoY;
    private float ultimaRotacaoY;
    
    [Header("Configurações")]
    public GameObject[] rodasQueGiram; 
    public GameObject[] rodasQueViram; 
    public float multiplicadorVelocidade = 100f;
    public float sensibilidadeCurva = 5f;
    public float anguloMaximoCurva = 30f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        ultimaRotacaoY = transform.eulerAngles.y;
    }

    void Update() {
        float velocidadeAtual = agent.velocity.magnitude;

        // 1. Giro constante das rodas (X)
        foreach (GameObject roda in rodasQueGiram) {
            roda.transform.Rotate(Vector3.forward * velocidadeAtual * multiplicadorVelocidade * Time.deltaTime);
        }

        // 2. Cálculo manual da "velocidade angular"
        float rotacaoAtualY = transform.eulerAngles.y;
        // Calcula o quanto o carro girou desde o último frame
        variacaoY = Mathf.DeltaAngle(ultimaRotacaoY, rotacaoAtualY);
        ultimaRotacaoY = rotacaoAtualY;

        // 3. Inclinação das rodas dianteiras (Y)
        foreach (GameObject roda in rodasQueViram) {
            // Primeiro gira o pneu (movimento de rodar no chão)
            roda.transform.Rotate(Vector3.up * velocidadeAtual * multiplicadorVelocidade * Time.deltaTime);
            
            // Depois calcula o ângulo visual da curva
            float anguloAlvo = (variacaoY / Time.deltaTime) * sensibilidadeCurva;
            anguloAlvo = Mathf.Clamp(anguloAlvo, -anguloMaximoCurva, anguloMaximoCurva);

            // Aplica a rotação local sem perder o giro do pneu
            Vector3 rot = roda.transform.localEulerAngles;
            roda.transform.localRotation = Quaternion.Euler(rot.x, anguloAlvo, 0);
        }
    }
}