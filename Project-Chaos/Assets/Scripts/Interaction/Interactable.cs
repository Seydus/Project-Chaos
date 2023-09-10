using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    #region Interaction
    public enum InteractionType
    {
        Click,
        Hold,
        HoldClick
    }

    [Header("Settings")]
    public InteractionType interactionType;

    public bool useInteraction;
    private float holdTime;
    public abstract void Interact();
    public bool UseInteraction() => useInteraction;
    public float GetHoldTime() => holdTime;
    public void IncreaseHoldTime() => holdTime += Time.deltaTime;
    public void ResetHoldTime() => holdTime = 0f;
    #endregion

    #region PickUp

    [HideInInspector] public Rigidbody myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }
    #endregion

    #region Dialogue
    public bool useDialogue;

    [HideIf("useDialogue", false)]
    public UnityEvent m_OnInteraction;

    public void DoInteraction()
    {
        if (useDialogue)
            m_OnInteraction.Invoke();
    }
    #endregion
}
