using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshRedemoinho : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float wanderRadius = 15f;    // Quão longe ele pode ir do ponto atual
    public float minWanderDelay = 1f;   // Tempo mínimo parado antes de escolher novo ponto
    public float maxWanderDelay = 4f;   // Tempo máximo parado

    private NavMeshAgent agent;
    private float timer;
    private float waitTime; // Variável para segurar o tempo de espera sorteado

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Sorteia o tempo de espera inicial
        waitTime = Random.Range(minWanderDelay, maxWanderDelay);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Se o tempo passou e ele já chegou ao destino
        if (timer >= waitTime && agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newPos = GetRandomNavMeshPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            
            // Zera o cronômetro e sorteia um novo tempo para a próxima parada
            timer = 0;
            waitTime = Random.Range(minWanderDelay, maxWanderDelay);
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit navHit;
        
        if (NavMesh.SamplePosition(randomDirection, out navHit, radius, agent.areaMask))
        {
            return navHit.position;
        }
        
        // Se por acaso não achar um ponto válido na área restrita, ele fica onde está
        return transform.position;
    }
}