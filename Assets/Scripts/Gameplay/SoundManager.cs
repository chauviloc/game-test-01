using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip backgroundClip;

    [Space]
    [Header("UI")]
    public AudioClip buttonClickClip;

    [Space] [Header("Gameplay")]
    public AudioClip hitAudioClip;
    public AudioClip winAudioClip;

    [SerializeField] private AudioSource _audioSourceBackground;
    [SerializeField] private AudioSource _audioSourcePrefab;
    [SerializeField] private Transform _audioSourceContainer;

    private readonly Queue<AudioSource> audioSources = new Queue<AudioSource>();
    private readonly LinkedList<AudioSource> inuse = new LinkedList<AudioSource>();
    private readonly Queue<LinkedListNode<AudioSource>> nodePool = new Queue<LinkedListNode<AudioSource>>();

    private bool hadInitSoundManager = false;


    private void Start()
    {
        AudioSettings.OnAudioConfigurationChanged += (value) =>
        {
            
        };
    }

    private int lastCheckFrame = -1;

    private void CheckInUse()
    {
        var node = inuse.First;
        while (node != null)
        {
            var current = node;
            node = node.Next;

            if (!current.Value.isPlaying)
            {
                audioSources.Enqueue(current.Value);
                inuse.Remove(current);
                nodePool.Enqueue(current);
            }
        }
    }

    public void PlayAtPoint(AudioClip clip, float volume = 1.0f, float Delay = 0)
    {
        
        AudioSource source;

        if (lastCheckFrame != Time.frameCount)
        {
            lastCheckFrame = Time.frameCount;
            CheckInUse();
        }

        if (audioSources.Count == 0)
            source = GameObject.Instantiate(_audioSourcePrefab, _audioSourceContainer);
        else
            source = audioSources.Dequeue();

        if (nodePool.Count == 0)
            inuse.AddLast(source);
        else
        {
            var node = nodePool.Dequeue();
            node.Value = source;
            inuse.AddLast(node);
        }

        source.transform.position = Vector3.zero;
        source.clip = clip;
        source.volume = volume;
        source.PlayDelayed(Delay);
    }

    public void PlayAtPoint(AudioClip clip, Vector3 pos, float volume = 1.0f, float Delay = 0)
    {
        
        AudioSource source;

        if (lastCheckFrame != Time.frameCount)
        {
            lastCheckFrame = Time.frameCount;
            CheckInUse();
        }

        if (audioSources.Count == 0)
            source = GameObject.Instantiate(_audioSourcePrefab, _audioSourceContainer);
        else
            source = audioSources.Dequeue();

        if (nodePool.Count == 0)
            inuse.AddLast(source);
        else
        {
            var node = nodePool.Dequeue();
            node.Value = source;
            inuse.AddLast(node);
        }

        source.transform.position = pos;
        source.clip = clip;
        source.volume = volume;
        source.PlayDelayed(Delay);
    }

    public void PlayBackgroundMusic()
    {
        
        _audioSourceBackground.loop = true;
        _audioSourceBackground.clip = backgroundClip;
        _audioSourceBackground.volume = 1f;
        _audioSourceBackground.Play();
        
    }

   
    public void StopBackgroundMusic()
    {
        _audioSourceBackground.Stop();
    }

    
    public void PlayButtonClickSFX()
    {
        PlayAtPoint(buttonClickClip);
    }

}
