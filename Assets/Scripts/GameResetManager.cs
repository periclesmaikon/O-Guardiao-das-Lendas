using UnityEngine;
using TMPro;

public class GameResetManager : MonoBehaviour
{
    public static GameResetManager Instance { get; private set; }

    [Header("Referências da Cena")]
    public GameObject player;                  
    public Transform casaSpawnPoint;           
    public GameObject fragmentsContainer;     

    [Header("UI de Game Over")]
    public GameObject gameOverPanel; 
    [Tooltip("Texto do Game Over")]
    public TextMeshProUGUI textoMotivo; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
{
    // Se o painel de Game Over estiver ligado na tela, força o mouse a aparecer a cada frame
    if (gameOverPanel != null && gameOverPanel.activeSelf)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

    public void ResetGameProgress(string motivoGameOver)
    {
        if (textoMotivo != null)
        {
            textoMotivo.text = "VOCÊ PERDEU!\n" + motivoGameOver;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        // Libera o mouse para o jogador conseguir clicar no botão
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 0f; // Congela o jogo
    }

    public void BotaoTentarNovamente()
    {
        ResetFragmentData();
        ReactivateSceneFragments();
        TeleportPlayer();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // Esconde o mouse e trava na tela para voltar a andar
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Time.timeScale = 1f; // Descongela o jogo
    }

    private void ResetFragmentData()
    {
        string currentCollected = PlayerPrefs.GetString("CollectedFragmentsList", "");
        if (!string.IsNullOrEmpty(currentCollected))
        {
            string[] fragments = currentCollected.Split(';');
            foreach (string fragment in fragments)
            {
                if (string.IsNullOrEmpty(fragment)) continue;
                string[] details = fragment.Split(':');
                if (details.Length == 2)
                {
                    string idToClear = details[0];
                    PlayerPrefs.DeleteKey("Fragment_" + idToClear);
                }
            }
        }
        
        PlayerPrefs.DeleteKey("CollectedFragmentsList");
        PlayerPrefs.Save();
    }

    private void ReactivateSceneFragments()
    {
        if (fragmentsContainer == null) return;
        Transform[] allFragments = fragmentsContainer.GetComponentsInChildren<Transform>(true);

        foreach (Transform fragmentTransform in allFragments)
        {
            if (fragmentTransform.gameObject == fragmentsContainer) continue;
            fragmentTransform.gameObject.SetActive(true);
        }
    }

    private void TeleportPlayer()
    {
        if (player == null || casaSpawnPoint == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        player.transform.position = casaSpawnPoint.position;
        player.transform.rotation = casaSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;
        if (rb != null) rb.isKinematic = false; 
    }
}