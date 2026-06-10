using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public Image On;
    public Image Off;
    public AudioSource somAmbiente; 
    
    private bool ativado;

    void Start()
    {
        // Lê a configuração salva. O '1' no final significa que, se não houver nada salvo, o padrão será 1 (ativado)
        int somSalvo = PlayerPrefs.GetInt("SomAtivado", 1);

        if (somSalvo == 1)
        {
            Ativar();
        }
        else
        {
            Desativar();
        }
    }

    public void Ativar()
    {
        ativado = true;
        On.gameObject.SetActive(true);
        Off.gameObject.SetActive(false);

        if (somAmbiente != null)
        {
            somAmbiente.mute = false;
        }
        
        // Salva a configuração com o valor 1 (Ativado)
        PlayerPrefs.SetInt("SomAtivado", 1);
        PlayerPrefs.Save();
    }

    public void Desativar()
    {
        ativado = false;
        On.gameObject.SetActive(false);
        Off.gameObject.SetActive(true);

        if (somAmbiente != null)
        {
            somAmbiente.mute = true;
        }
        
        // Salva a configuração com o valor 0 (Desativado)
        PlayerPrefs.SetInt("SomAtivado", 0);
        PlayerPrefs.Save();
    }
}