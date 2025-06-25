using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private GameObject playerVisuals;
    [SerializeField] private Animator playerAnimation;

    private Rigidbody rb;
    private Vector3 currentInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RotatePlayer();
        StoreInput();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    // Rotate the player
    private void RotatePlayer()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 lookDirection = hit.point - playerVisuals.transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.05f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                playerVisuals.transform.rotation = Quaternion.Slerp(playerVisuals.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void PlayerMovement()
    {
        Vector3 moveDirection = (playerVisuals.transform.forward * currentInput.z + playerVisuals.transform.right * currentInput.x).normalized;
        rb.velocity = moveDirection * moveSpeed;

        if(currentInput == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void StoreInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        currentInput = new Vector3(horizontal, 0f, vertical);

        playerAnimation.SetFloat("HorizontalInput", horizontal);
        playerAnimation.SetFloat("VerticalInput", vertical);
    }
}
