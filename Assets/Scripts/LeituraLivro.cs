using UnityEngine;
using TMPro;

public class LeituraLivro : MonoBehaviour
{
    [Header("UI References")]
    public GameObject painelLeitura;
    public TextMeshProUGUI textoLegivel;

    [Header("Conteúdo da Página")]
    [TextArea(5, 10)]
    public string textoDaLenda;

    public void AbrirLeituraLegivel()
    {
        textoLegivel.text = textoDaLenda;
        painelLeitura.SetActive(true);
    }

    public void FecharLeituraLegivel()
    {
        painelLeitura.SetActive(false);
    }
}