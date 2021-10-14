/*
 * Instantiates a quilt of tile objects and assigns them textures
 * utilizes netWorkManager to import assets and generate usable lists 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiltInit : MonoBehaviour
{

    // True when all tiles are placed
    bool placed = false;
    
    // quilt dimensions 
    public int X = 2;
    public int Y = 4;

    // Tile prefab
    public GameObject tilePrefab;

    // To contain positions of tiles for generation
    int[,] positions = new int[700,2];

    // To import files 
    public NetworkManager networkManager;

    // Contains images and audio
    List<Texture2D> imageLibrary;
    List<AudioClip> soundLibrary;

    // Contains tile objects
    List<AnimationManager> tiles = new List<AnimationManager>();

    void Start()
    {
        StartCoroutine(Begin());
    }

    // Setup and placement all in one - delayed to allow for download 
    IEnumerator Begin()
    {
        // Retrieves Textures and audio from web location
        networkManager.RetrieveTextures();
        networkManager.RetrieveSounds();

        // doesn't place squares until textures are done importing
        yield return new WaitUntil(() => networkManager.importStatus() == true);
        Debug.Log("Import Status" + networkManager.importStatus());
        imageLibrary = networkManager.getTextures();
        soundLibrary = networkManager.GetAudioClips();


        // Size can be modified based on set size etc
        SpiralPositions(X, Y);
        PlaceSquares();

    }

    // Genarates an array of indexes that result in a spiral placement 
    void SpiralPositions(int X, int Y)
    {
        int dx = 0, dy = -1, x = 0, y = 0, a;
        for (int i = 1; i < Mathf.Pow(Mathf.Max(X, Y), 2); i++)
        {
            if (((-X / 2) <= x) && (x <= (X / 2)) && (-Y / 2 <= y) && (y <= (Y / 2)))
            {
                positions[i - 1, 0] = x;
                positions[i - 1, 1] = y;
                Debug.Log(x + "," + y);
            }
            if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y))
            {
                a = dx;
                dx = -dy;
                dy = a;
            }
            x += dx;
            y += dy;
        }
    }

    // returns true when placement is finished
    public bool isPlaced()
    {
        return placed;
    }

    // Places squares

    void PlaceSquares()
    {
        // holds positions
        Vector3 position = Vector3.zero;
        // Holds reference to tile objects and components
        GameObject hold;
        AudioSource audioSource;
        MeshRenderer meshRenderer;

        for (int i = 0; i < imageLibrary.Count; i++)
        {
            // assign positions
            position.x = .15f*positions[i, 0];
            position.z = .15f *positions[i, 1];

            // Instantiate new tile obj
            hold = Instantiate(tilePrefab,this.transform);
            hold.transform.localPosition = position;
            Debug.Log(hold);

            // Sound to be implemented 
            audioSource = hold.AddComponent<AudioSource>();
            audioSource.clip = soundLibrary[i];

            // assign texture
            meshRenderer = hold.GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = imageLibrary[i];

            // add animation managers
            tiles.Add(hold.AddComponent<AnimationManager>());

            Debug.Log(tiles.Count);

            // tile start animation
            StartAnimation();

            // set tiles placed to true
            placed = true;
        }
    }

    // Start animation
    void StartAnimation()
    {
        StartCoroutine(aniHelp());
    }

    // adds delay between animations
    IEnumerator aniHelp()
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            yield return new WaitForSeconds(.3f);
            tiles[i].StartAnimation();
        }
    }
}
