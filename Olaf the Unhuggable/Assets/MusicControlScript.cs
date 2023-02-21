using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControlScript : MonoBehaviour
{
    public static MusicControlScript instance;

    // this code makes sure the music control game object is not deleted between scenes, 
    // and also makes sure there isn't a duplicate made when a scene is reloaded 
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
