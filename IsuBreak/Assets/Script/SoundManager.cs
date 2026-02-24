using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public TMP_Text volumeText;
    public Slider volumeSlider;

    private static SoundManager instance;

    void Awake()
    {
        // Tek bir SoundManager kalmasýný saðlar
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Eðer kayýt yoksa varsayýlan 0.5 sesi ayarla
        if (!PlayerPrefs.HasKey("audioVolume"))
        {
            PlayerPrefs.SetFloat("audioVolume", 0.5f);
        }

        // Kaydedilmiþ sesi yükle
        float savedVolume = PlayerPrefs.GetFloat("audioVolume");

        // Slider ve ses deðerini eþitle
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateText(savedVolume);

        // Slider hareket ettikçe sesi deðiþtir
        volumeSlider.onValueChanged.AddListener(SetAudio);
    }

    public void SetAudio(float value)
    {
        AudioListener.volume = value;
        UpdateText(value);
        PlayerPrefs.SetFloat("audioVolume", value);
        PlayerPrefs.Save();
    }

    private void UpdateText(float value)
    {
        if (volumeText != null)
            volumeText.text = "%" + Mathf.RoundToInt(value * 100);
    }
}
