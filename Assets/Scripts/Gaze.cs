/*
 * Assigned to the camera object and handles gaze interactions with Animation Manager
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gaze : MonoBehaviour
{
    // holds references to all animation managers
    List<AnimationManager> infos = new List<AnimationManager>();

    public QuiltInit quilt;

    // Start is called before the first frame update
    void Start()
    {
        // verify all objects have been instanced before attempting to locate sound objects
        Coroutine running = StartCoroutine(findSoundObj());
    }

    // Update is called once per frame
    void Update()
    {
        // find the quilt object - will throw null pointer until found... We will see if it works
        quilt = FindObjectOfType<QuiltInit>();

        // Project a forward ray from the camera object, if it hits a square with an attatched sound play the sound 
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            GameObject go = hit.collider.gameObject;
            if (go.CompareTag("EnableAnim") && !go.GetComponent<AnimationManager>().audioSource.isPlaying)
            {
                PlaySounds(go.GetComponent<AnimationManager>());
            }

        }

        // if LOS is broken stop playing the sound
        else
        {
            StopAll();
        }
    }

    void PlaySounds(AnimationManager desiredSound)
    {
        foreach (AnimationManager info in infos)
        {
            if (info == desiredSound)
            {
                info.playSound();
            }
            else
            {
                info.stopSound();
            }
        }
    }
    void StopAll()
    {
        foreach (AnimationManager info in infos)
        {
            info.stopSound();
        }
    }

    // Adds a delay to identifying sound objects to allow for the assignment of all squares 
    IEnumerator findSoundObj()
    {
        yield return new WaitUntil(() => quilt != null);
        yield return new WaitUntil(() => quilt.isPlaced() == true);
        infos = FindObjectsOfType<AnimationManager>().ToList();
    }
}
