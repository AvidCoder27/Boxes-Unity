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
        _masterVolume = SettingsHandler.MasterVolume / 20;
        _musicVolume = SettingsHandler.MusicVolume / 20;
        _soundVolume = SettingsHandler.SoundVolume / 20;

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

    private float LinearToDecibal(float x)
    {
        return Mathf.Log10(Mathf.Clamp(x, 0.00001f, 1)) * 20f;
    }
}
