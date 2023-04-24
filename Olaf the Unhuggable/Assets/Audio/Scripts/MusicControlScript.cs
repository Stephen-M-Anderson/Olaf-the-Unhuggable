/* 
To use the audio manager, create an empty game object in the scene and add the AudioManager script. 
Then, in the Inspector, add audio clips to the "sounds" array, and set their properties (eg. volume, pitch).

To play a sound, call the Play() function and pass in the name of the sound to play.
Example: AudioManager.instance.Play("explosion");
This will play the audio clip with the name "explosion" in the audio manager's "sounds" array.

using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = sound.playDefault;
            sound.source.outputAudioMixerGroup = mixerGroup;
        }
        foreach(Sound sound in sounds)
        {
            if(sound.source.playOnAwake == true)
            {
                Play(sound.source.name);
            }
        }
    }

    public void Play(string name)
    {
        Sound sound = null;
        foreach(Sound s in sounds)
        {
            if(s.name == name)
            {
                sound = s;
            }
        }
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.source.Play();
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;

    public bool playDefault = false;

    [HideInInspector]
    public AudioSource source;
}
*/

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