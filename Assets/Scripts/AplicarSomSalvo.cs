using UnityEngine;

public class AplicarSomSalvo : MonoBehaviour
{
    private AudioSource somAmbiente;

    void Start()
    {
        somAmbiente = GetComponent<AudioSource>();

        // Lê a mesma configuração salva no Menu
        int somSalvo = PlayerPrefs.GetInt("SomAtivado", 1);

        if (somAmbiente != null)
        {
            if (somSalvo == 1)
            {
                somAmbiente.mute = false; // Som rolando
            }
            else
            {
                somAmbiente.mute = true;  // Som mutado
            }
        }
    }
}