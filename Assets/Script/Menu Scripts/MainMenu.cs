using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer masterMixer;

    [Header("Resolutions Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] availableResolutions;

    [Header("Navigation")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        ResolutionHandler();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void DisplayOptions()
    {
        // Deactivate main menu UI and active an options page
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void DisplayMainMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Player has quit the game");
    }

    public void SetVolume(float volume)
    {
        float dB;

        if (volume <= 0f)
            dB = -80f; 
        else
            dB = Mathf.Log10(volume) * 20f;

        masterMixer.SetFloat("Master Volume", dB);

        Debug.Log("Current Decible Value Is: " + dB);
    }

    private void ResolutionHandler()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution res = availableResolutions[i];
            string option = res.width + " x " + res.height;
            resolutionOptions.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        Resolution selected = availableResolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen); // keep full screen mode
    }
}
