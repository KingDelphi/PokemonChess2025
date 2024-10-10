using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneChanger : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; // Se llama cuando el video termina.
    }

    void Update()
    {
        // Detecta si el usuario ha hecho clic con el ratón o ha presionado una tecla.
        if (Input.anyKeyDown)
        {
            ChangeScene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        ChangeScene(); // Cambia a la siguiente escena cuando el video termina.
    }

    void ChangeScene()
    {
        // Cambia a la siguiente escena en la lista de escenas.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
