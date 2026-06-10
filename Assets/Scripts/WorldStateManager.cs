using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    [System.Serializable]
    public class LegendWorldData
    {
        public string nomeLenda;
        public GameObject lendaObject3D;
        public GameObject perigosDaLenda;
    }

    [Header("Estado do Mundo")]
    public LegendWorldData[] lendas;

    void Start()
    {
        // Assim que o jogo começa, ele varre a lista de lendas
        foreach (var lenda in lendas)
        {
            // Verifica no banco de dados se essa lenda específica já foi resolvida
            if (PlayerPrefs.GetInt("LegendSolved_" + lenda.nomeLenda, 0) == 1)
            {
                // Se foi resolvida, aplica as mudanças no cenário imediatamente
                if (lenda.lendaObject3D != null) lenda.lendaObject3D.SetActive(true);
                if (lenda.perigosDaLenda != null) lenda.perigosDaLenda.SetActive(false);
            }
        }
    }
}