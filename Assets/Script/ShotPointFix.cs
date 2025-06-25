using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPointFix : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask layerMask;

    private void LateUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0f;

            if(lookDirection.sqrMagnitude > 0.05f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }
    }
}
