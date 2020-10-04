using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text textQuality;
    public Slider sliderVolume;
    public GameObject panelHowTo;

    int quality;
    string[] qualities = new string[] { "Low", "Medium", "High", "Ultra" };

    bool isOpen;

    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioListener.volume = 1f;
        }

        sliderVolume.value = AudioListener.volume;

        if (!PlayerPrefs.HasKey("Quality"))
        {
            PlayerPrefs.SetInt("Quality", 3);
        }

        quality = PlayerPrefs.GetInt("Quality");
        ChangeQuality(0);

        panelHowTo.SetActive(false);
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeQuality(int by)
    {
        quality += by;
        if (quality < 0)
        {
            quality = 0;
        }
        if (quality > 3)
        {
            quality = 3;
        }

        QualitySettings.SetQualityLevel(quality);
        PlayerPrefs.SetInt("Quality", quality);

        textQuality.text = qualities[quality];
    }

    public void OnSlider()
    {
        AudioListener.volume = sliderVolume.value;
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
    }

    public void OnHowTo()
    {
        if (isOpen)
        {
            panelHowTo.SetActive(false);
            isOpen = false;
        }
        else
        {
            panelHowTo.SetActive(true);
            isOpen = true;
        }
    }
}
