using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DoorArrowVisualisation : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform[] arrows;
    public Transform[] doors;
    [SerializeField] private Transform player;

    [SerializeField] private float radiusFromPlayer = 100f;

    private bool hasPopulated;
    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);

        for(int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == null || arrows[i] == null) continue;

            Vector3 doorScreenPosition = mainCamera.WorldToScreenPoint(doors[i].position);

            Vector2 direction = (doorScreenPosition - playerScreenPosition).normalized;

            arrows[i].position = playerScreenPosition + (Vector3)(direction * radiusFromPlayer);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrows[i].rotation = Quaternion.Euler(0, 0, angle + -90);
        }
    }

    private void UpdateArrowsVisibility()
    {
        int numDoors = doors != null ? doors.Length : 0;

        for (int i = 0; i < arrows.Length; i++)
        {
            if (i < numDoors)
                arrows[i].gameObject.SetActive(true);
            else
                arrows[i].gameObject.SetActive(false);
        }
    }

    public void ClearDoorReferences()
    {
        if (doors == null) return;

        Array.Clear(doors, 0, doors.Length);
        Array.Resize(ref doors, 0);
        hasPopulated = false;
    }

    public void PopulateDoorReference()
    {
        if (!hasPopulated)
        {
            GameObject[] doorBoxes = manager.rooms[manager.roomIndex].doorBoxes;

            doors = new Transform[doorBoxes.Length];

            for (int i = 0; i < doorBoxes.Length; i++)
            {
                doors[i] = doorBoxes[i].transform;
            }

            hasPopulated = true;

            UpdateArrowsVisibility();
        }
    }
}
