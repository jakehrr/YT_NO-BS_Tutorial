using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float regenTime = 2.5f;
    [SerializeField] private int maxHealth = 5; 
    [SerializeField] private int health = 5;

    [Header("Sprint Stats")]
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 25f;  
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float regenCooldownTimer = 0f;
    [SerializeField] private float staminaRegenDelay = 1f; 

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject playerVisuals;
    [SerializeField] private Animator playerAnimation;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AudioSource playerSounds;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private ParticleSystem bloodVFX;
    [SerializeField] private TextMeshProUGUI zombiesKilledText;

    [Header("Private References")]
    private Rigidbody rb;
    private Vector3 currentInput;
    private bool isSprinting = false;
    private PauseMenu pauseMenuAccess;
    private PlayerStatTracking playerStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pauseMenuAccess = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
        playerStats = GameObject.Find("Player Stat Track").GetComponent<PlayerStatTracking>();

        StartCoroutine(RegenHealthOverTime());
    }

    private void Update()
    {
        if (pauseMenuAccess.isPaused) return;

        RotatePlayer();
        StoreInput();
        HandleSprint();
    }

    private void FixedUpdate()
    {
        if (pauseMenuAccess.isPaused) return;

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

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        rb.velocity = moveDirection * currentSpeed;

        if (currentInput == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // Player Sprint. 
    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f)
        {
            isSprinting = true;
            stamina -= staminaDrainRate * Time.deltaTime;
            if (stamina < 0f) stamina = 0f;

            regenCooldownTimer = staminaRegenDelay;
        }
        else
        {
            isSprinting = false;

            if (regenCooldownTimer > 0f)
            {
                regenCooldownTimer -= Time.deltaTime;
            }
            else
            {
                stamina += staminaRegenRate * Time.deltaTime;
                if (stamina > maxStamina) stamina = maxStamina;
            }
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

        // Play Blood Particle VFX.
        bloodVFX.Play();

        if (health <= 0)
        {
            // Play death sound on player death. 
            playerSounds.PlayOneShot(deathSFX);

            // Turn on game over screen.
            gameOverUI.SetActive(true);
            zombiesKilledText.text = "Zombies Killed: " + playerStats.currentZombiesKilled.ToString();

            // Play Death Animation.
            playerAnimation.SetBool("PlayerDead", true);

            // Disable all player animations.
            playerStats.ResetZombiesKilled();
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
