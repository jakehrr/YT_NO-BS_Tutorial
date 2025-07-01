using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            // Fade out the scene to begin transition
            manager.FadeOutScene();

            // Load the next room
            manager.BeginNextRoom();

            // Player has entered a doorway
            manager.ResetZombieSpawning();
        }
    }
}
