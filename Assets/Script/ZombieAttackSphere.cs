using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackSphere : MonoBehaviour
{
    [SerializeField] private GameObject attackSphere;

    public void EnableAttackSphere()
    {
        attackSphere.GetComponent<SphereCollider>().enabled = true;
    }

    public void DisableAttackSphere()
    {
        attackSphere.GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            // make the player take damage
            PlayerController playerRef = other.gameObject.GetComponent<PlayerController>();
            playerRef.PlayerTakeDamage();
        }
    }
}
