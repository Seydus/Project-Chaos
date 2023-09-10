using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InteractionInstigator : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float interactionDistance;

    [Header("Interaction")]
    [SerializeField] private Image interactionProgress;
    [SerializeField] private GameObject interactionHoldGo;

    private bool canPressHold = false;
    private bool succesfulHit = false;

    private Ray ray;
    private RaycastHit hit;

    private Interactable interactableScript;
    private List<Interactable> m_NearbyInteractables = new List<Interactable>();

    [Header("PickUp")]
    [SerializeField] private float cameraZAxisDistance;
    [SerializeField] private float pickUpSpeed;
    [SerializeField] private float rotationPickUpSpeed;
    [SerializeField] private float objectLimitDistance;
    private float currentSpeed = 0f;

    private bool holdObject;

    private Vector3 direction;
    private Vector3 camPos;

    #region Input Interaction

    private void Awake()
    {
        canPressHold = true;        
    }

    private void Update()
    {
        PickUpInteraction();
        InputInteraction();
        StopHoldingInteraction();
    }

    public void InputInteraction()
    {
        ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (interactableScript != null && holdObject) return;

            interactableScript = hit.collider.GetComponent<Interactable>();

            if (interactableScript != null)
            {
                HandleInteraction(interactableScript);

                if (!succesfulHit)
                {
                    m_NearbyInteractables.Add(interactableScript);
                    succesfulHit = true;
                }

                interactionHoldGo.SetActive(interactableScript.interactionType == Interactable.InteractionType.Hold);
            }
        }
        else
        {
            if (succesfulHit && interactableScript != null)
            {
                m_NearbyInteractables.Clear();
                succesfulHit = false;
            }

            interactionHoldGo.SetActive(false);
        }
    }
    #endregion

    #region PickUpInteraction
    public void PickUpInteraction()
    {
        camPos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cameraZAxisDistance));

        if (interactableScript != null && holdObject)
        {
            interactableScript.Interact();
            PickUpObject();
        }

        if (interactableScript != null && holdObject)
        {
            //If player lets go or when object is too far from the player
            if (Input.GetButtonUp("E") || Vector3.Distance(camPos, interactableScript.transform.position) >= objectLimitDistance)
            {
                DropObject();
            }
        }
    }

    private void PickUpObject()
    {
        interactableScript.myBody.useGravity = false;

        interactableScript.myBody.constraints = RigidbodyConstraints.FreezeRotation;
        interactableScript.myBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void DropObject()
    {
        interactableScript.myBody.useGravity = true;

        interactableScript.myBody.constraints = RigidbodyConstraints.None;
        interactableScript.myBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

        interactableScript.myBody.velocity = Vector3.zero;
        interactableScript.myBody.angularVelocity = Vector3.zero;

        holdObject = false;
    }

    private void FixedUpdate()
    {
        if (holdObject)
        {
            currentSpeed = pickUpSpeed * Time.fixedDeltaTime;
            direction = camPos - interactableScript.transform.position;
            interactableScript.myBody.velocity = direction * currentSpeed;
            //interactableScript.transform.rotation = Quaternion.Slerp(interactableScript.transform.rotation, cam.transform.rotation, rotationPickUpSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region HandleInteraction
    public bool HasNearbyInteractables()
    {
        return m_NearbyInteractables.Count != 0;
    }

    public void HandleInteraction(Interactable interactable)
    {
        switch (interactable.interactionType)
        {
            case Interactable.InteractionType.Click:
                if (HasNearbyInteractables() && Input.GetButtonDown("E"))
                {
                    interactable.Interact();

                    //Ideally, we'd want to find the best possible interaction (ex: by distance & orientation).
                    m_NearbyInteractables[0].DoInteraction();
                }
                break;
            case Interactable.InteractionType.Hold:
                if (Input.GetButton("E") && canPressHold)
                {
                    interactable.IncreaseHoldTime();
                    if (interactable.GetHoldTime() > 1f)
                    {
                        interactable.Interact();

                        m_NearbyInteractables[0].DoInteraction();

                        interactable.ResetHoldTime();
                        canPressHold = false;
                    }
                }
                else
                {
                    interactable.ResetHoldTime();
                }
                interactionProgress.fillAmount = interactable.GetHoldTime();
                break;
            case Interactable.InteractionType.HoldClick:
                if (Input.GetButton("E"))
                {
                    holdObject = true;
                }
                break;
        }
    }

    private void StopHoldingInteraction()
    {
        //Avoids the player from always holding interaction.
        if (Input.GetButtonUp("E"))
        {
            canPressHold = true;
        }
    }
    #endregion
}