using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioToPlay;

    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioToPlay = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip clip)
    {
        audioToPlay.PlayOneShot(clip);
    }

}
