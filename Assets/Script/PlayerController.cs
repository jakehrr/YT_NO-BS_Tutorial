using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float regenTime = 2.5f;
    [SerializeField] private int maxHealth = 5; 
    [SerializeField] private int health = 5; 

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject playerVisuals;
    [SerializeField] private Animator playerAnimation;
    [SerializeField] private GameObject gameOverUI;

    [Header("Private References")]
    private Rigidbody rb;
    private Vector3 currentInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(RegenHealthOverTime());
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

    // Rotate the player.
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

    // Move the player.
    private void PlayerMovement()
    {
        Vector3 moveDirection = (playerVisuals.transform.forward * currentInput.z + playerVisuals.transform.right * currentInput.x).normalized;
        rb.velocity = moveDirection * moveSpeed;

        if(currentInput == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // Get input from the player.
    private void StoreInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        currentInput = new Vector3(horizontal, 0f, vertical);

        playerAnimation.SetFloat("HorizontalInput", horizontal);
        playerAnimation.SetFloat("VerticalInput", vertical);
    }

    // Player takes damage.
    public void PlayerTakeDamage()
    {
        health--;

        if (health <= 0)
        {
            // Turn on game over screen
            gameOverUI.SetActive(true);

            // Play Death Animation.
            playerAnimation.SetBool("PlayerDead", true);

            // Disable all player animations.
            GetComponent<PlayerController>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponentInChildren<AssaultRifle>().enabled = false;
            Destroy(GetComponent<Rigidbody>());
        }
    }

    // Heal player over time.
    private IEnumerator RegenHealthOverTime()
    {
        yield return new WaitForSeconds(regenTime);

        if (health >= maxHealth)
            health = maxHealth;
        else
            health++;

        StartCoroutine(RegenHealthOverTime());  
    }
}
