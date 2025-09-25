using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MenuSystemEnd : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaJuego = "game"; // Nombre de la escena del juego
    public string nombreEscenaMenu = "inicio"; // Nombre de la escena del menú principal
    
    /// <summary>
    /// Método para reiniciar el juego cuando se presiona el botón "Retry"
    /// </summary>
    public void Retry()
    {
        // Cargar la escena del juego nuevamente
        SceneManager.LoadScene(nombreEscenaJuego);
    }
    
    /// <summary>
    /// Método para volver al menú principal cuando se presiona el botón "Main Menu"
    /// </summary>
    public void MainMenu()
    {
        // Cargar la escena del menú principal
        SceneManager.LoadScene(nombreEscenaMenu);
    }
    
    /// <summary>
    /// Método para salir de la aplicación cuando se presiona el botón "Salir"
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
