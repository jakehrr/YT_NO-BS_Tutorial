using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatTracking : MonoBehaviour
{
    public static PlayerStatTracking instance;

    public int currentZombiesKilled = 0; // How many zombies has the player killed in just this game. 
    public int allTimeZombiesKilled; // How many zombies has the player killed across all their games. 

    [HideInInspector]
    public string allTimeKillsString = "AllTimeKills"; // Hard set what the variable name is. 

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);

        GetAllTimeZombiesKilled(allTimeKillsString);
    }



    public void IncrementCurrentZombiesKilled()
    {
        currentZombiesKilled++;
    }

    public void IncrementAllTimeZombiesKilled(string keyName)
    {
        int currentValue = PlayerPrefs.GetInt(keyName, 0);
        currentValue++;
        PlayerPrefs.SetInt(keyName, currentValue);
        PlayerPrefs.Save();

        Debug.Log(PlayerPrefs.GetInt("AllTimeKills"));
    }

    public void GetAllTimeZombiesKilled(string savedVariableName)
    {
        allTimeZombiesKilled = PlayerPrefs.GetInt(savedVariableName);
    }

    public void ResetZombiesKilled()
    {
        currentZombiesKilled = 0;
    }
}
