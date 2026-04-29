using UnityEngine;
using System.Collections;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Configurações do Prefab")]
    public GameObject carPrefab;    // O Prefab do seu carro
    public Transform routeParent;   // O objeto pai dos pontos (a rota)

    [Header("Configurações de Spawn")]
    public int maxCars = 5;         // N carros no total
    public float spawnInterval = 3f; // Tempo entre cada spawn

    private int carsSpawned = 0;

    void Start()
    {
        // Inicia a rotina de criação de carros
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (carsSpawned < maxCars)
        {
            SpawnCar();
            carsSpawned++;

            // Espera o tempo definido antes de gerar o próximo
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCar()
    {
        // Instancia o carro na posição do Spawner (ou qualquer posição, já que o script de navegação dá teleporte)
        GameObject newCar = Instantiate(carPrefab, transform.position, Quaternion.identity);
        
        // Pega o componente de navegação e passa a rota para ele
        CarNavigation nav = newCar.GetComponent<CarNavigation>();
        if (nav != null)
        {
            nav.Initialize(routeParent);
        }
    }
}