using UnityEngine;
using UnityEngine.SceneManagement;

//This script handles screen changes dynamically. It was based heavily off of code found
//on the topic in one of Unity's officical tutorials.

public class ScreenChanges : MonoBehaviour
{

    public Texture2D fadeOutTexture;
    public static AudioSource audio1;
    public static AudioSource audio2;
    public static AudioSource music1;
    public static AudioSource music2;
    public static float fadeSpeed = 0.8f;

    private static int drawDepth = -1000;
    private float alpha = 1.0f;
    private static int fadeDir = -1;

    void Start()
    {
        if (audio1 == null) { audio1 = GameObject.Find("Audio1").GetComponent<AudioSource>(); DontDestroyOnLoad(audio1); }
        if (audio2 == null) { audio2 = GameObject.Find("Audio2").GetComponent<AudioSource>(); DontDestroyOnLoad(audio2); }
        if (music1 == null) { music1 = GameObject.Find("Music1").GetComponent<AudioSource>(); DontDestroyOnLoad(music1); }
        if (music2 == null) { music2 = GameObject.Find("Music2").GetComponent<AudioSource>(); DontDestroyOnLoad(music2); }
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        if (!music1.isPlaying && SceneManager.GetActiveScene().name != "Gameplay")
        {
            music1.Play();
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        BeginFade(-1);
    }

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public static float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }


    //Go to the next scene
    public void NextScene()
    {
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Manual_Click.reset();
        changeMusic();
    }

    //Go to the last scene
    public void LastScene()
    {
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); ;
    }

    //Go to a specific scene by name
    public void SpecificScene(string name)
    {
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(name);
        changeMusic();
    }

    //Go to a specific scene by name
    public static void staticSpecificScene(string name)
    {
        if (audio1 != null) { audio1.Play(); }
        float fadeTime = BeginFade(1);
        System.Threading.Thread.Sleep(Mathf.CeilToInt(fadeTime));
        SceneManager.LoadScene(name);
        changeMusic();
    }

    //Change Music when you get to the gameplay
    private static void changeMusic()
    {
        if (SceneManager.GetActiveScene().name == "SelectRocketScreen")
        {
            music1.Stop();
            music2.Play();
        }
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            music2.Stop();
            music1.Play();
        }
    }

    public static void launch_sounds()
    {
        if (audio2.isPlaying == false)
        {
            audio2.Play();
        }
        else
        {
            audio2.Stop();
        }
    }
}
