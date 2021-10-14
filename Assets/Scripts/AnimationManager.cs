/*
 * Script applied to tile objects to create generation and gaze interactions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // square image
    public Texture2D texture;

    // determines if coroutine is already running 
    bool growScale = false;
    Coroutine coScale; 

    // sound 
    public AudioSource audioSource; 

    // Original position - used with view commands 
    Vector3 originalPosition;
    Vector3 originalScale; 

    //Start 
    void Start()
    {
        // get Vector 3D set scale to zero
        this.transform.localScale = Vector3.zero;
    }


    // Bool that returns true when texture is assigned 
    public bool PropAssigned()
    {
        return texture != null;
    }

    // Start up  animation 
    public void StartAnimation()
    {
        StartCoroutine(StartHelper());
    }

    // Start animation coroutine
    IEnumerator StartHelper()
    {
        // Small hold to stagger animations
        yield return new WaitForSeconds(.2f);

        // get position
        originalPosition = this.transform.localPosition;
        originalScale = new Vector3(.1f, .1f, .001f);

        // Assigns sound obj
        audioSource = this.GetComponent<AudioSource>();

        // initial values 
        float timeElapsed = 0;
        float lerpDuration = 1;

        // Final scale 
        Vector3 finalScale = new Vector3();

        while (timeElapsed < lerpDuration)
        {
            finalScale.x = (Mathf.Lerp(0, .1f, timeElapsed / lerpDuration));
            finalScale.y = (Mathf.Lerp(0, .1f, timeElapsed / lerpDuration));
            finalScale.z = (Mathf.Lerp(0, .001f, timeElapsed / lerpDuration));
            this.transform.localScale = finalScale;

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    // Grow / Display animation if a is -1 return to original orientation
    public void Display(int a)
    {
        coScale = StartCoroutine(DisplayHelperScale(a));
        growScale = true;
    }

    IEnumerator DisplayHelperScale(int a)
    {
        // initial values 
        float timeElapsed = 0;
        float lerpDuration = .25f;
        Vector3 currentScale = this.transform.localScale;

        Vector3 finalScale;
        if (a == -1)
            finalScale = originalScale;
        else
        {
            finalScale.x = originalScale.x + .05f;
            finalScale.y = originalScale.y + .05f;
        }

        // Modifies position
        while (timeElapsed < lerpDuration)
        {
            currentScale.x = (Mathf.Lerp(currentScale.x, finalScale.x, timeElapsed / lerpDuration));
            currentScale.y = (Mathf.Lerp(currentScale.y, finalScale.y, timeElapsed / lerpDuration));
            this.transform.localScale = currentScale;

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    // Play sound
    public void playSound()
    {
        audioSource.Play();
        Display(1);
    }

    // Stop playing sound 
    public void stopSound()
    {
        audioSource.Stop();
        if (growScale == true)
        {
            StopCoroutine(coScale);
            growScale = false;
        }
        Display(-1);
    }
    
}
