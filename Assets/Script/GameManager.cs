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
    public int roomIndex;
    public int currentZombieCount = 0;
    public int maxZombieCount = 15;
    public int currentZombiesAlive = 0;

    [Header("Room Elements")]
    [SerializeField] private GameObject indicatorArrows;
    public List<RoomData> rooms;
    [SerializeField] private GameObject UI;

    private void Update()
    {
        if (currentZombieCount >= maxZombieCount && currentZombiesAlive == 0)
        {
            roomComplete = true;
        }

        if (roomComplete)
        {
            indicatorArrows.SetActive(true);
            foreach (var door in rooms[roomIndex].doorBoxes)
                door.GetComponent<BoxCollider>().enabled = true;

            UI.GetComponent<DoorArrowVisualisation>().PopulateDoorReference();
        }
        else if (!roomComplete)
        {
            foreach (var door in rooms[roomIndex].doorBoxes)
                door.GetComponent<BoxCollider>().enabled = false;

            UI.GetComponent<DoorArrowVisualisation>().ClearDoorReferences();
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
        ResetZombieSpawning();

        // Wait for the fade out
        yield return new WaitForSeconds(1.5f);

        // Deactivate Arrow Indicators
        indicatorArrows.SetActive(false);

        // When room index reaches the max number of rooms, loop back to the first room. 
        roomIndex = (roomIndex + 1) % rooms.Count;

        // Deactivate all rooms
        foreach(var room in rooms)
            room.roomRoot.SetActive(false);

        // Activate current room
        var nextRoom = rooms[roomIndex];
        nextRoom.roomRoot.SetActive(true);

        GameObject player = GameObject.Find("Player");
        player.transform.position = nextRoom.playerSpawnPoint.position;

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

[System.Serializable]
public class RoomData
{
    public GameObject roomRoot;
    public Transform playerSpawnPoint;
    public GameObject[] doorBoxes;
}
