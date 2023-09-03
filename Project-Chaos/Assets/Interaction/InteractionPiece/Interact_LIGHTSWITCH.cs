using UnityEngine;

public class Interact_LIGHTSWITCH : Interactable
{
    [Header("Modifiers")]
    [SerializeField] private Light lightType;
    private bool isOn;

    private void Awake()
    {
        isOn = true;
        UpdateObject();
    }

    private void UpdateObject()
    {
        lightType.enabled = isOn;
    }

    public override void Interact()
    {
        if (!useInteraction) return;

        isOn = !isOn;
        UpdateObject();
    }
}
