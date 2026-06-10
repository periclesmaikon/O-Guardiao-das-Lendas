using UnityEngine;
using TMPro;

public class FragmentUIManager : MonoBehaviour
{
    [Header("Configurações do Inventário")]
    public GameObject fragmentPrefab;
    public Transform fragmentListContainer;

    private void OnEnable()
    {
        UpdateFragmentListUI();
    }

    public void UpdateFragmentListUI()
    {
        foreach (Transform child in fragmentListContainer)
        {
            Destroy(child.gameObject);
        }

        string rawData = PlayerPrefs.GetString("CollectedFragmentsList", "");
        if (string.IsNullOrEmpty(rawData)) return;

        string[] fragments = rawData.Split(';');

        foreach (string fragment in fragments)
        {
            if (string.IsNullOrEmpty(fragment)) continue;

            string[] details = fragment.Split(':');
            if (details.Length == 2)
            {
                GameObject newBtn = Instantiate(fragmentPrefab, fragmentListContainer);
                
                DraggableFragment dragScript = newBtn.GetComponent<DraggableFragment>();
                if (dragScript != null)
                {
                    dragScript.Setup(details[0], details[1]);
                }
            }
        }
    }
}