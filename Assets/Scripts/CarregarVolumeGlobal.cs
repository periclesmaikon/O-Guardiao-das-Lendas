using UnityEngine;

public class CarregarVolumeGlobal : MonoBehaviour
{
    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("VolumeGlobal", 1f);
    }
}