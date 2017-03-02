using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squish : MonoBehaviour {

    AudioSource squish;

	void Start () {
        squish = GetComponent<AudioSource>();
	}
	
    void OnCollisionEnter(Collision collision)
    {
        if (!squish.isPlaying)
        {
            squish.pitch = Random.Range(0.5f, 1f);
            squish.Play();
        }
    }
}
