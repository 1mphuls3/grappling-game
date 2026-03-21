using UnityEngine;
using UnityEngine.UI;

public class AudioSourceManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider.value = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : 1f;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }

    public void VolumeUpdate()
    {
        audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }
}
