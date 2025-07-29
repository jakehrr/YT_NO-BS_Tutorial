using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer masterMixer;

    [Header("Resolutions Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] availableResolutions;

    [Header("Navigation")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject menuBackground;

    public bool isPaused;

    private bool optionsOpen;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        ResolutionHandler();
    }

    private void Update()
    {
        HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            PauseGame();
        }
        else if (!isPaused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        menuBackground.SetActive(true);

        if (!optionsOpen)
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        menuBackground.SetActive(false);
        isPaused = false;
    }

    #region Options Menu Methods
    
    public void DisplayOptions()
    {
        optionsOpen = true;
    }

    public void HideOptions()
    {
        optionsOpen = false;
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

    #endregion
}
