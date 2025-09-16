using UnityEngine;

public class AndroidSetup : MonoBehaviour
{
    [Header("Configuración Android")]
    public bool optimizarParaAndroid = true;
    public int targetFrameRate = 60;
    public bool mantenerPantallaEncendida = true;
    
    void Start()
    {
        #if UNITY_ANDROID
        if (optimizarParaAndroid)
        {
            ConfigurarAndroid();
        }
        #endif
    }
    
    void ConfigurarAndroid()
    {
        // Configurar frame rate
        Application.targetFrameRate = targetFrameRate;
        
        // Mantener pantalla encendida
        if (mantenerPantallaEncendida)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        
        // Configurar calidad gráfica para móviles
        QualitySettings.SetQualityLevel(2); // Calidad media por defecto
        
        // Optimizaciones de memoria
        QualitySettings.masterTextureLimit = 1; // Reducir resolución de texturas
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        
        // Configurar V-Sync
        QualitySettings.vSyncCount = 1;
        
        Debug.Log("Configuración Android aplicada correctamente");
    }
}
