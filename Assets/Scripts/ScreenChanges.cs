﻿using UnityEngine;
using UnityEngine.SceneManagement;

//This script handles screen changes dynamically. It was based heavily off of code found
//on the topic in one of Unity's officical tutorials.

public class ScreenChanges : MonoBehaviour {

    public Texture2D fadeOutTexture;
    //Need an audio source for every piece of audio that plays between screens
    public static AudioSource audio1;
    public static AudioSource audio2;
    public static AudioSource music1;
    public static AudioSource music2;
    public static float fadeSpeed = 0.8f;

    private static int drawDepth = -1000;
    private float alpha = 1.0f;
    private static int fadeDir = -1;

    void Start() {
        //For all of the audio sources, mark them as DontDestroyOnLoad so they play between screens
        if (audio1 == null && GameObject.Find("Audio1") != null) { audio1 = GameObject.Find("Audio1").GetComponent<AudioSource>(); DontDestroyOnLoad(audio1); }
        if (audio2 == null && GameObject.Find("Audio2") != null) { audio2 = GameObject.Find("Audio2").GetComponent<AudioSource>(); DontDestroyOnLoad(audio2); }
        if (music1 == null && GameObject.Find("Music1") != null) { music1 = GameObject.Find("Music1").GetComponent<AudioSource>(); DontDestroyOnLoad(music1); }
        if (music2 == null && GameObject.Find("Music2") != null) { music2 = GameObject.Find("Music2").GetComponent<AudioSource>(); DontDestroyOnLoad(music2); }
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        //Play the first track on wake, but only if it is not playing and we're not on the gameplay screen
        if (music1 != null && !music1.isPlaying && SceneManager.GetActiveScene().name != "Gameplay") {
            music1.Play();
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        BeginFade(-1);
    }

    //Trigger fade
    public static float BeginFade(int direction) {
        fadeDir = direction;
        return (fadeSpeed);
    }


    //Go to the next scene
    public void NextScene() {
        //Play the bleeping between screens
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Manual_Click.reset();
        changeMusic();
    }

    //Go to the last scene
    public void LastScene() {
        //Play the bleeping between screens
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); ;
    }

    //Go to a specific scene by name - Use this if calling from Unity
    public void SpecificScene(string name) {
        //Play the bleeping between screens
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(name);
        changeMusic();
    }

    //Go to a specific scene by name - Use this if calling from within another script
    public static void staticSpecificScene(string name) {
        //Play the bleeping between screens
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(name);
        changeMusic();
    }

    //Change Music when you get to the gameplay, and also when you leave that screen
    //You have to check for the scene BEFORE the one you want to change the music for
    //because SceneManager.GetActiveScene().name does not immediately change with a
    //scene transition. This changes the music immediately, rather than after a delay
    public static void changeMusic() {
        if (SceneManager.GetActiveScene().name == "Debriefing_Mars" || SceneManager.GetActiveScene().name == "Debriefing_Shuttle" || SceneManager.GetActiveScene().name == "Debriefing_Satellite") {
            music1.Stop();
            music2.Play();
        }
        if (SceneManager.GetActiveScene().name == "Gameplay") {
            music2.Stop();
            music1.Play();
        }
    }

    //Play the rocket launch sounds if they are not already playing.
    //Also stop them if they are.
    public static void launch_sounds() {
        if (audio2.isPlaying == false) {
            audio2.Play();
        }
        else {
            audio2.Stop();
        }
    }
}
