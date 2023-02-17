using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _masterVolume, _musicVolume, _soundVolume;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioSource PrepStageMusic;
    [SerializeField] private AudioSource PlayStageMusic;

    private void OnEnable()
    {
        Actions.OnSceneSwitchStart += HandleSceneSwitchStart;
        Actions.OnSceneSwitchEnd += HandleSceneSwitchEnd;
    }

    private void OnDisable()
    {
        Actions.OnSceneSwitchStart -= HandleSceneSwitchStart;
        Actions.OnSceneSwitchEnd -= HandleSceneSwitchEnd;
    }

    private void Start()
    {
        PrepStageMusic.Play();
    }

    private void Update()
    {
        masterMixer.SetFloat("musicVolume", LinearToDecibal(_musicVolume * _masterVolume));
        masterMixer.SetFloat("soundVolume", LinearToDecibal(_soundVolume * _masterVolume));
    }

    private void HandleSceneSwitchStart()
    {
        StartCoroutine(FadeAudioSource.StartFade(PrepStageMusic, 2, 0));
    }

    private void HandleSceneSwitchEnd()
    {
        PrepStageMusic.Stop();
        PlayStageMusic.Play();
        PlayStageMusic.volume = 0;
        StartCoroutine(FadeAudioSource.StartFade(PlayStageMusic, 2, 1));
    }

    /// <summary>
    /// Set the master volume
    /// </summary>
    /// <param name="volume">volume, from 0-20</param>
    public void SetMasterVolume(float volume)
    {
        _masterVolume = volume / 20;
    }
    /// <summary>
    /// Set the volume for the music
    /// </summary>
    /// <param name="volume">volume, from 0-20</param>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume / 20;
    }
    /// <summary>
    /// Set the volume for the sound effects
    /// </summary>
    /// <param name="volume">volume, from 0-20</param>
    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume / 20;
    }

    private float LinearToDecibal(float x)
    {
        return Mathf.Log10(Mathf.Clamp(x, 0.00001f, 1)) * 20f;
    }
}
