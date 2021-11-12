using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringCandlelight : MonoBehaviour
{
    [SerializeField] 
    private float flickerAmplitude = 0.1f;

    [SerializeField] 
    private float lerpSpeed = 0.15f;

    private float intensityBase = 1.0f;

    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
        intensityBase = light.intensity;
        StartCoroutine(flickerCoroutine());
    }

    private IEnumerator flickerCoroutine()
    {
        while (true)
        {
            light.intensity = Mathf.Lerp(light.intensity, intensityBase + Random.Range(-flickerAmplitude, flickerAmplitude), lerpSpeed);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }
}
