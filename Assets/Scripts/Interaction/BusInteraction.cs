using UnityEngine;
using UnityEngine.SceneManagement;

public class BusInteraction : MonoBehaviour, IInteractable
{
    [Header("Configurações")]
    public string sceneToLoad = "Sitio";
    public string promptComMapa = "Pegar ônibus";

    public void Interact()
    {
        MapSystem mapSystem = Object.FindFirstObjectByType<MapSystem>();

        // Só executa a ação se mapa já tiver sido aberto
        if (mapSystem != null && mapSystem.wasMapOpened)
        {
            PlayerPrefs.SetInt("PegouOnibus", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public string GetInteractPrompt()
    {
        MapSystem mapSystem = Object.FindFirstObjectByType<MapSystem>();
        
        // Se o mapa foi aberto, retorna o texto para viajar
        if (mapSystem != null && mapSystem.wasMapOpened)
        {
            return promptComMapa;
        }
        
        // Se não, retorna vazio
        return string.Empty; 
    }
}