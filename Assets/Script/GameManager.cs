using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Room Control Variables")]
    public bool roomComplete = false;
    [SerializeField] private Animator fadeAnimation;

    [Header("Current Wave Stats")]
    public int wave;
    public int currentZombieCount = 0;
    public int maxZombieCount = 15;
    public int currentZombiesAlive = 0;

    [Header("Room Elements")]
    [SerializeField] private GameObject indicatorArrows;
    [SerializeField] private GameObject[] doorBoxes;
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private Transform[] playerSpawnPoints;

    private void Update()
    {
        if(currentZombieCount >= maxZombieCount && currentZombiesAlive == 0) roomComplete = true;

        if (roomComplete)
        {
            indicatorArrows.SetActive(true);
            foreach(GameObject go in doorBoxes)
            {
                go.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    public void ResetZombieSpawning()
    {
        currentZombieCount = 0;
        maxZombieCount += 5;
    }

    public void BeginNextRoom()
    {
        StartCoroutine(BeginRoomTransition());
    }

    private IEnumerator BeginRoomTransition()
    {
        roomComplete = false;

        // Wait for the fade out
        yield return new WaitForSeconds(1.5f);

        // Deactivate Arrow Indicators
        indicatorArrows.SetActive(false);

        // Increase what wave we're on
        wave += 1;

        // Change what rooms active
        foreach (GameObject go in rooms) go.SetActive(false);
        rooms[wave].gameObject.SetActive(true);

        // Move player to next position
        GameObject player = GameObject.Find("Player");
        player.transform.position = playerSpawnPoints[wave].position;

        // Give 1 second for all the logic to finish firing for a smooth transition
        yield return new WaitForSeconds(1f); 

        // Fade back into the scene
        fadeAnimation.SetBool("FadeOut", false);
        fadeAnimation.SetBool("FadeIn", true);
    }

    public void FadeOutScene() 
    {
        // Fade back into the scene
        fadeAnimation.SetBool("FadeOut", true);
    }
}
