using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light theLight;
    float startInt;

    public bool willFlicker = true;

    void Awake()
    {
        theLight = GetComponent<Light>();
        startInt = theLight.intensity;
    }

    void Update()
    {
        if(willFlicker)
            theLight.intensity = Mathf.Lerp(theLight.intensity, Random.Range(startInt/2, startInt), 10 * Time.deltaTime);
    }
}
