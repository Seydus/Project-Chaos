using UnityEngine;
using TMPro;

public class UIInteractionTextController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Text;
    [SerializeField]
    private InteractionInstigator m_WatchedInteractionInstigator;

    void Update()
    {
        m_Text.enabled = m_WatchedInteractionInstigator.enabled && m_WatchedInteractionInstigator.HasNearbyInteractables();
    }
}
