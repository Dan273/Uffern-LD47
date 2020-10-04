using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PhaserController : MonoBehaviour
{
    public static PhaserController instance;

    public Text textCharge;

    public float depleteRate = 4;
    public float charge;

    bool audioPlayed;

    AudioSource phaserSound;

    void Awake()
    {
        instance = this;

        phaserSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (phaserSound.isPlaying)
        {
            if (Input.GetMouseButtonUp(0))
            {
                phaserSound.Stop();
                audioPlayed = false;
            }
        }    
    }

    public void FirePhaser()
    {
        if (charge > 0)
        {
            if (!audioPlayed)
                phaserSound.Play();
            audioPlayed = true;
            charge -= Time.deltaTime * depleteRate;
            textCharge.text = "Phaser: " + Mathf.RoundToInt(charge) + "%";
        }
        else if(charge <= 0)
        {
            charge = 0;
            textCharge.text = "Phaser: " + Mathf.RoundToInt(charge) + "%";
            phaserSound.Stop();
        }
    }
}
