using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class CarNavigation : MonoBehaviour
{
    [Header("Configurações de Espera")]
    public float delayBeforeRespawn = 2.0f; 

    [Header("Configurações de Travamento (Engarrafamento)")]
    public float maxStuckTime = 1.5f; // Tempo máximo que o carro pode ficar parado
    public float minVelocity = 1f;  // Velocidade mínima para considerar que está andando

    [Header("Customização Visual")]
    public Texture2D[] carTextures; 
    public Renderer bodyRenderer;   

    private NavMeshAgent agent;
    private Transform[] waypoints;
    private int currentIndex = 0;
    private bool initialized = false;
    private bool isWaiting = false; 
    
    // Novo timer para controlar o tempo parado
    private float stuckTimer = 0f; 

    public void Initialize(Transform routeParent)
    {
        agent = GetComponent<NavMeshAgent>();
        
        int childCount = routeParent.childCount;
        waypoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            waypoints[i] = routeParent.GetChild(i);
        }

        if (waypoints.Length > 0)
        {
            initialized = true;
            RandomizeTexture(); 
            TeleportToStart();
        }
    }

    void Update()
    {
        if (!initialized || isWaiting) return;

        // --- NOVA LÓGICA DE DETECÇÃO DE TRAVAMENTO ---
        // Verifica se a velocidade atual é muito baixa (quase parado)
        if (agent.velocity.magnitude < minVelocity)
        {
            stuckTimer += Time.deltaTime; // Começa a contar o tempo

            if (stuckTimer >= maxStuckTime)
            {
                // Carro ficou muito tempo parado. Reseta o timer e manda dar respawn!
                stuckTimer = 0f;
                StartCoroutine(WaitAndRespawn());
                return; // Encerra o Update neste frame para não conflitar com o GoToNextPoint
            }
        }
        else
        {
            // Se o carro andou, zera o timer de travamento
            stuckTimer = 0f;
        }
        // ---------------------------------------------

        // Lógica original de ir para o próximo ponto
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        currentIndex++;
        
        if (currentIndex >= waypoints.Length)
        {
            StartCoroutine(WaitAndRespawn());
        }
        else
        {
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }

    IEnumerator WaitAndRespawn()
    {
        isWaiting = true;
        ToggleVisuals(false);
        agent.isStopped = true;

        yield return new WaitForSeconds(delayBeforeRespawn);

        RandomizeTexture();
        TeleportToStart();
        
        ToggleVisuals(true);
        agent.isStopped = false;
        isWaiting = false;
    }

    void RandomizeTexture()
    {
        if (carTextures.Length > 0 && bodyRenderer != null)
        {
            int randomIndex = Random.Range(0, carTextures.Length);
            bodyRenderer.material.SetTexture("_BaseMap", carTextures[randomIndex]);
        }
    }

    void TeleportToStart()
    {
        currentIndex = 0;
        stuckTimer = 0f; // Garante que o timer comece zerado no respawn
        
        agent.enabled = false;
        transform.position = waypoints[0].position;
        transform.rotation = waypoints[0].rotation;
        agent.enabled = true;

        if (waypoints.Length > 1)
        {
            currentIndex = 1;
            agent.SetDestination(waypoints[1].position);
        }
    }

    void ToggleVisuals(bool show)
    {
        if(TryGetComponent<MeshRenderer>(out MeshRenderer mr)) mr.enabled = show;
        
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = show;
        }
    }
}