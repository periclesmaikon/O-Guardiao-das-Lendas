using UnityEngine;
using UnityEngine.UI;

public class ControladorDeVolume : MonoBehaviour
{
    [Header("Slider-Volume")]
    public Slider sliderVolume;

    void Start()
    {
        float volumeSalvo = PlayerPrefs.GetFloat("VolumeGlobal", 1f);

        // Atualiza a posição da bolinha do slider para bater com o volume salvo
        if (sliderVolume != null)
        {
            sliderVolume.value = volumeSalvo;
        }

        // Aplica o volume no jogo
        AudioListener.volume = volumeSalvo;
    }

    // Este método vai receber o valor do Slider toda vez que você arrastar ele
    public void AlterarVolume(float valor)
    {
        AudioListener.volume = valor; // Muda o volume global instantaneamente
        
        PlayerPrefs.SetFloat("VolumeGlobal", valor); // Salva o novo valor
        PlayerPrefs.Save();
    }
}