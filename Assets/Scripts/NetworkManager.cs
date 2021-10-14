/*
 * Used tutorial provided by https://theslidefactory.com/using-unitywebrequest-to-download-resources-in-unity/
 * Modified methods found in tutorial
 * NOTE: Audio file format must match
 * 
 * Combined with array builder to generate arraylist 
 */

using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    private Texture2D texture;
    private AudioClip clip;

    // stores how many assets are expected at the URL - must be adjusted for more or less assets
    // **IMPORTANT**
    public int assetCount = 6; 

    // Break conditions for loops
    bool finish = false;

    // File location and general format ex https://drive.google.com/file/d/1mxF3pMkMNlIIMb2QWZIa1TVqGnGUJcB6/view?usp=sharing
    public string urlFrontPicture = "https://sites.bc.edu/her/files/2021/09/Quilt";
    public string urlBackPicture = "-1024x1024.jpg";
    // Sounds
    public string urlFrontSound = "https://sites.bc.edu/her/files/2021/10/TestAudio";
    public string urlBackSound = ".mp3";

    // lists to store imported values
    List<AudioClip> audioClips = new List<AudioClip>(); 
    List<Texture2D> texture2Ds = new List<Texture2D>();

    // Returns finished list 
    public List<Texture2D> getTextures()
    {
        return texture2Ds;
    }

    // Returns finished list
    public List<AudioClip> GetAudioClips()
    {
        return audioClips;
    }

    // returns true when the array has reached the specified length
    public bool importStatus()
    {
        return texture2Ds.Count == assetCount && audioClips.Count == assetCount;
    }


    // Generates a list of textures and returns them
    public void RetrieveTextures()
    {

        // stores url
        string url;

        // indicates image number
        int i = 1;

        while (i <= assetCount)
        {
            url = (urlFrontPicture + i.ToString() + urlBackPicture);
            DownloadImage(url);
            i++;
        }

        // when the textures are done return true
        finish = true;
    }

    public List<AudioClip> RetrieveSounds()
    {
        // stores url
        string url;

        // indicates image number
        int i = 1;

        // loop terminates when a clip value is null 
        while (i <= assetCount)
        {
            url = (urlFrontSound + i.ToString() + urlBackSound);
            DownloadSound(url);
            i++;
        }
        return audioClips;
    }


    // Returns image retrieved from server as a Texture2D 
    private void DownloadImage(string url)
    {
        StartCoroutine(ImageRequest(url, (UnityWebRequest req) =>
        {
            // if the image cannot sucessfully be retrieved pass a message to debug log and set texture to null
            if (req.result == UnityWebRequest.Result.ConnectionError) 
            {
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
                texture = null;
            }
            // Assign the image texture and add to list it
            else
            {
                texture = DownloadHandlerTexture.GetContent(req);
                texture2Ds.Add(texture);
            }
        }));
    }

    // Helper for Download Image
    IEnumerator ImageRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            callback(req);
        }
    }


    // returns an audioclip 
    public  void DownloadSound(string url)
    {
        StartCoroutine(SoundRequest(url));
    }

    // coroutine for downloading sounds - modified from Unity Scripting API : https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html
    IEnumerator SoundRequest(string url)
    { 
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log(myClip == null);
                audioClips.Add(myClip);
            }
        }
    }
}
