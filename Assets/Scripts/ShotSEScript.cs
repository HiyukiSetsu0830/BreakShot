using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShotSEScript : MonoBehaviour {

    //足音を入れる
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private bool randomizePitch = true;
    [SerializeField] private float pitchRange = 0.1f;

    protected AudioSource audioSource;


    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponents<AudioSource>()[0];
        audioSource.volume = 0.1f;
    }

    public void PlayShotSE() {

        if (randomizePitch) audioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);

        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }


}
