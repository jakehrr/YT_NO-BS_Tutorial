using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int wave;
    public int currentZombieCount = 0;
    public int maxZombieCount = 15;

    public bool roomComplete = false;
    public int currentZombiesAlive = 0;
    [SerializeField] private GameObject indicatorArrows;
    [SerializeField] private Animator fadeAnimation;
    [SerializeField] private GameObject[] doorBoxes;

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
        wave += 1;

        fadeAnimation.SetBool("FadeOut", true);
    }
}
