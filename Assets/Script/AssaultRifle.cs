using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssaultRifle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shotPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shotAudioSource;
    [SerializeField] private AudioClip shotAudioClip;

    [Header("Shooting Settings")]
    [SerializeField] private float timeBetweenShots = 0.1f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private TextMeshProUGUI ammoUI;

    [SerializeField] private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToShoot = 0f;

    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private float bulletSpeed = 100f;

    private PauseMenu pauseMenuAccess;

    private void Start()
    {
        pauseMenuAccess = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();

        currentAmmo = magazineSize;
    }

    private void Update()
    {
        if (pauseMenuAccess.isPaused) return;

        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.time + timeBetweenShots;
            Shoot();
        }

        ammoUI.text = currentAmmo.ToString() + " / " + magazineSize.ToString();
    }

    private void Shoot()
    {
        currentAmmo--;

        if(muzzleFlash != null) muzzleFlash.Play();
        if(shotAudioSource != null) shotAudioSource.PlayOneShot(shotAudioClip);

        Vector3 origin = shotPoint.position;
        Vector3 direction = shotPoint.forward;
        direction.y = 0f; 
        direction.Normalize();

        Vector3 targetPoint;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, hitLayer))
        {
            targetPoint = hit.point;
            Debug.DrawLine(origin, hit.point, Color.red, 1f);
            Debug.Log("Hit at: " + hit.point + " Object Is: " + hit.collider.name);
            
            Enemy enemyHit = hit.transform.GetComponent<Enemy>();
            if (enemyHit != null)
            {
                enemyHit.TakeDamage(1);
            }
        }
        else
        {
            targetPoint = origin + direction * range;
            Vector3 endPoint = origin + direction * range;
            Debug.DrawLine(origin, endPoint, Color.yellow, 1f);
            Debug.Log("Shot hit nothing and went to: " + endPoint);
        }

        if(bulletTrailPrefab != null)
        {
            Quaternion rotation = Quaternion.LookRotation(targetPoint - origin);
            GameObject trail = Instantiate(bulletTrailPrefab, origin, rotation);
            StartCoroutine(MoveTrail(trail, targetPoint));
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Reloaded!");
    }

    private IEnumerator MoveTrail(GameObject trail, Vector3 target)
    {
        float distance = Vector3.Distance(trail.transform.position, target);
        float time = 0f;
        float duration = distance / bulletSpeed;

        Vector3 start = trail.transform.position;

        while(time < duration)
        {
            trail.transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = target; 
        Destroy(trail);
    }

}
