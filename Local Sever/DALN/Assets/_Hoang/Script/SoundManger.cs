using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManger : Singleton<SoundManger>
{
    // Start is called before the first frame update
    // Start is called before the first frame update

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // Cho background music
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource clickSource; // Cho sound effects

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip[] VFXSound;
    [SerializeField] private AudioClip ClickSound;


    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.3f;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float clickVolume = 0.5f;
    public bool TurnOn = true;


    protected override void Awake()
    {
        base.Awake();
        InitializeAudio();
        TurnOn = true;
    }
    private void Update()
    {
        if (TurnOn)
        {
            // Thiết lập volume
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;
            clickSource.volume = clickVolume;
        }
        else
        {
            musicSource.volume = 0;
            sfxSource.volume = 0;
            clickSource.volume = 0;
        }
       /* if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
        }*/

    }
    private void InitializeAudio()
    {
        // Tạo và thiết lập AudioSource cho music nếu chưa có
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }

        // Tạo và thiết lập AudioSource cho sfx nếu chưa có
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (clickSource == null)
        {
            clickSource = gameObject.AddComponent<AudioSource>();
            clickSource.loop = false;
            clickSource.playOnAwake = false;
        }


        // Thiết lập volume
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        clickSource.volume = clickVolume;

        // Bắt đầu phát nhạc nền
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        
    }

    public void PlayClickSound()
    {
        if (ClickSound != null)
        {
            clickSource.PlayOneShot(ClickSound, clickVolume);
          
            
        }
    }
    public void PlayVFXSound(int soundIndex)
    {
        if (VFXSound != null)
        {
            sfxSource.PlayOneShot(VFXSound[soundIndex], sfxVolume);
        }
    }
}
