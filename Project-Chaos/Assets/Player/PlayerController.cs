using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;

    [Header("Settings")]
    [SerializeField] private PlayerSettingsModel playerSettings;

    [Space]
    [SerializeField] private float viewClampYMin = -70;
    [SerializeField] private float viewClampYMax = 80;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    //[SerializeField] public float gravity = -9.81f;
    //private Vector3 velocity;

    [Header("Jump Detector")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float sphereRadius;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stance")]
    //[SerializeField] private PlayerStance playerStance;
    //[SerializeField] private float playerStanceSmoothing;
    //[SerializeField] private CharacterStance playerStandStance;
    //[SerializeField] private CharacterStance playerCrouchStance;

    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;

    private float stanceCapsuleHeightVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        //CalculateStance();
    }

    private void CalculateView()
    {
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");

        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -rotateHorizontal : rotateHorizontal) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted ? rotateVertical : -rotateVertical) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }
    private void CalculateMovement()
    {
        float horizontalSpeed = Input.GetAxisRaw("Horizontal") * playerSettings.WalkingStrafeSpeed * Time.deltaTime;
        float verticalSpeed = Input.GetAxisRaw("Vertical") * playerSettings.WalkingForwardSpeed * Time.deltaTime;

        Vector3 newMovementSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        newMovementSpeed = transform.TransformDirection(newMovementSpeed);

        #region Jump
        if(playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if(playerGravity < -0.1f && isGrounded)
        {
            playerGravity = -0.1f;
        }

        newMovementSpeed.y += playerGravity;
        newMovementSpeed += jumpingForce * Time.deltaTime;

        #endregion

        characterController.Move(newMovementSpeed);
    }
    private void CalculateJump()
    {
        isGrounded = Physics.CheckSphere(groundPosition.position, sphereRadius, groundLayer);

        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFalloff);

        #region Jump2.0
        //if (characterController.isGrounded && velocity.y < 0)
        //{
        //    velocity.y = -1f;
        //}

        //velocity.y += gravity * Time.deltaTime;

        //characterController.Move(velocity * Time.deltaTime);
        #endregion

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private void Jump()
    {
        if (!isGrounded) return;

        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0f;
        #region Jump2.0
        //velocity.y = Mathf.Sqrt(playerSettings.JumpingHeight * -2f * gravity);
        #endregion
    }

    //private void CalculateStance()
    //{
    //    CharacterStance currentStance = playerStandStance;

    //    if(playerStance == PlayerStance.Crouch)
    //    {
    //        currentStance = playerCrouchStance;
    //    }

    //    cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
    //    cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

    //    characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
    //    characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    //}

    //private void Crouch()
    //{
    //    if (playerStance == PlayerStance.Crouch)
    //    {
    //        playerStance = PlayerStance.Stand;
    //        return;
    //    }

    //    playerStance = PlayerStance.Crouch;
    //}

    public void CursorLockedMode(bool CursorLockedEnable)
    {
        if(CursorLockedEnable)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPosition.position, sphereRadius);
    }
}
