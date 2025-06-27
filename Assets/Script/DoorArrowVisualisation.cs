using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DoorArrowVisualisation : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform[] arrows;
    [SerializeField] private Transform[] doors;
    [SerializeField] private Transform player;

    [SerializeField] private float radiusFromPlayer = 100f;

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
}
