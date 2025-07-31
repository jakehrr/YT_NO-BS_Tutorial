using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float regenTime = 2.5f;
    [SerializeField] private int maxHealth = 5; 
    [SerializeField] private int health = 5;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float speedBlendRate; 

    [Header("Animation Variables")]
    [SerializeField] private float animationBlendSpeed = 0.5f;
    [SerializeField] private float smoothedHorizontal;
    [SerializeField] private float smoothedVertical;

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

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;

    [Header("FX And Sounds")]
    [SerializeField] private AudioSource playerSounds;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private ParticleSystem bloodVFX;
    [SerializeField] private TextMeshProUGUI zombiesKilledText;

    [Header("Private References")]
    private Rigidbody rb;
    private Vector3 currentInput;
    private bool isSprinting = false;
    private bool isDead;
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
        InputAndAnimation();
        HandleSprint();
        DisplayStatsInUI();
    }

    private void FixedUpdate()
    {
        if (pauseMenuAccess.isPaused) return;

        PlayerMovement();
    }

    #region Core Player Functionality.
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
        // Calculate direction and set speed. 
        //
        Vector3 moveDirection = (playerVisuals.transform.forward * currentInput.z + playerVisuals.transform.right * currentInput.x).normalized;
        float targetSpeed = (currentInput == Vector3.zero) ? 0f : (isSprinting ? sprintSpeed : moveSpeed);

        // Smooth players speed and add that velocity. 
        //
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedBlendRate);
        rb.velocity = moveDirection * currentSpeed;
    }

    // Player Sprint. 
    private void HandleSprint()
    {
        // Check if sprinting, change player speed & handle stamina 
        //
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
    private void InputAndAnimation()
    {
        // Get player input and store a current input. 
        //
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        currentInput = new Vector3(horizontal, 0f, vertical);

        // Smoothly interpolate values for animation
        //
        smoothedHorizontal = Mathf.Lerp(smoothedHorizontal, horizontal, Time.deltaTime * animationBlendSpeed);
        smoothedVertical = Mathf.Lerp(smoothedVertical, vertical, Time.deltaTime * animationBlendSpeed);

        // Pass smoothed values to the Animator
        //
        playerAnimation.SetFloat("HorizontalInput", smoothedHorizontal);
        playerAnimation.SetFloat("VerticalInput", smoothedVertical);
    }

    #endregion

    #region Player Statistic Handlers

    // Player takes damage.
    public void PlayerTakeDamage()
    {
        health--;

        // Play Blood Particle VFX.
        bloodVFX.Play();

        if (health <= 0)
        {
            isDead = true;

            healthSlider.value = 0;

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
        if (isDead) yield return null;

        yield return new WaitForSeconds(regenTime);

        if (health >= maxHealth)
            health = maxHealth;
        else
            health++;

        StartCoroutine(RegenHealthOverTime());  
    }

    // Parse player stats to player stats script.
    private void DisplayStatsInUI()
    {
        float targetHealth = Mathf.InverseLerp(0, maxHealth, health);
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * 10f);

        float targetStamina = Mathf.InverseLerp(0, maxStamina, stamina);
        staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStamina, Time.deltaTime * 10f);
    }

    #endregion
}
