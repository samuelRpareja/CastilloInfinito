using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif 

public class MenuSystem : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaJuego = "game"; // Nombre de la escena del juego
    
    /// <summary>
    /// Método para cargar la escena del juego cuando se presiona el botón "Jugar"
    /// </summary>
    public void Jugar()
    {
        // Cargar la escena del juego
        SceneManager.LoadScene(nombreEscenaJuego);
    }
    
    /// <summary>
    /// Método para salir del juego cuando se presiona el botón "Salir"
    /// </summary>
    public void Salir()
    {
        // En el editor de Unity, detener la reproducción
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // En móviles, intentar cerrar la aplicación
        #if UNITY_ANDROID || UNITY_IOS
        // En Android, usar el método nativo para cerrar
        #if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                currentActivity.Call("finish");
            }
        }
        #else
        // En iOS, simplemente usar Application.Quit()
        Application.Quit();
        #endif
        #else
        // En otras plataformas (PC, etc.)
        Application.Quit();
        #endif
        #endif
    }
}
