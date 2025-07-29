using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 10;

    [Header("Attack")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Timing")]
    [SerializeField] private float spawnDelay = 3f; 

    [Header("Effects")]
    [SerializeField] private Animator zombieAnim;
    [SerializeField] private ParticleSystem bloodFX;
    [SerializeField] private AudioSource enemySoundEffects;
    [SerializeField] private AudioClip[] bulletHitSFX;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform zombieVisuals;

    private GameManager manager;
    private PlayerStatTracking playerStats;
    private bool isActive = false;
    private bool isDead = false;
    private bool isAttacking = false;

    private void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        playerStats = GameObject.Find("Player Stat Track").GetComponent<PlayerStatTracking>();
        StartCoroutine(ActivationDelay());
    }

    private IEnumerator ActivationDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        isActive = true;
    }

    private void Update()
    {
        if (!isActive || isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);  

        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.y = 0f;

        if(lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            zombieVisuals.rotation = Quaternion.Slerp(zombieVisuals.rotation, targetRotation * Quaternion.Euler(0, 180f, 0), Time.deltaTime * 5f); 
        }

        if(distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            zombieAnim.SetBool("InRange", false);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        agent.isStopped = true;
        zombieAnim.SetBool("InRange", true);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(bloodFX != null) bloodFX.Play();

        PlayBulletHitSounds();

        if(health <= 0)
        {
            Die();
        }
    }

    private void PlayBulletHitSounds()
    {
        if (bulletHitSFX.Length == 0 || enemySoundEffects == null) return;

        int index = Random.Range(0, bulletHitSFX.Length);
        enemySoundEffects.PlayOneShot(bulletHitSFX[index]);
    }

    private void Die()
    {
        if (isDead) return;

        playerStats.IncrementAllTimeZombiesKilled(playerStats.allTimeKillsString);
        playerStats.IncrementCurrentZombiesKilled();
        manager.currentZombiesAlive--;
        GetComponent<CapsuleCollider>().enabled = false;
        isDead = true;
        agent.isStopped = true;
        zombieAnim.SetBool("IsAlive", false);

        StartCoroutine(BeginDeath());
    }

    private IEnumerator BeginDeath()
    {
        zombieAnim.SetBool("IsAlive", false);

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
