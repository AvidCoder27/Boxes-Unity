using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] [Range(0, 1)]
    float musicVolume, soundVolume;

    [SerializeField] AudioMixer masterMixer;

    [SerializeField] AudioSource PlayStageMusic;

    private void Update()
    {
        bool music = masterMixer.SetFloat("musicVolume", LinearToDecibal(musicVolume));
        bool sound = masterMixer.SetFloat("soundVolume", LinearToDecibal(soundVolume));
    }

    private float LinearToDecibal(float x)
    {
        return Mathf.Log10(Mathf.Clamp(x, 0.00001f, 1)) * 20f;
    }
}
