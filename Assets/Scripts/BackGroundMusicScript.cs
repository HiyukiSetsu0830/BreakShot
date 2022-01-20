using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicScript : MonoBehaviour
{

    private AudioSource audioSource;
    private AudioClip clip;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayBGM() {

        audioSource.Play();
    }
}
