using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using NGO = Unity.Netcode;

[DefaultExecutionOrder(100)]
public class MenuMultiplayer : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string menuScene = "MenuPrincipal";
    [SerializeField] private string gameplayScene = "MultiGameplay";

    [Header("Red")]
    [SerializeField] private string ipAddress = "192.168.152.64";
    [SerializeField] private int port = 7777;
    [Tooltip("Tiempo máximo para que el cliente conecte, en segundos")]
    [SerializeField] private float clientConnectTimeout = 8f;

    [Header("UI (opcional)")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuMultiplayer;
    [SerializeField] private GameObject mensajeErrorPrefab;
    
    [Header("Mensajes de Error en Pantalla")]
    [SerializeField] private GameObject panelError;
    [SerializeField] private UnityEngine.UI.Text textoError;
    [SerializeField] private UnityEngine.UI.Button botonCerrarError;
    [SerializeField] private Color colorTextoError = Color.red;
    
    [Header("Texto de Estado de Conexión")]
    [SerializeField] private UnityEngine.UI.Text textoEstadoConexion;
    [SerializeField] private TMPro.TextMeshProUGUI textoEstadoConexionTMP;

    private NGO.NetworkManager nm;
    private UnityTransport transport;

    private bool pendingClientConnect; // evita dobles intentos
    private string lastDisconnectReason; // si tu versión lo soporta

    // IPs típicas de hotspot
    private readonly string[] ANDROID_HOTSPOT_IPs = {"10.162.19.254", "10.26.69.41", "192.168.1.1", "192.168.152.217", "10.0.0.1"};
    private const string WINDOWS_HOTSPOT_IP = "192.168.137.1";

    private void Start()
    {
        nm = NGO.NetworkManager.Singleton ?? FindObjectOfType<NGO.NetworkManager>();
        if (nm == null)
        {
            Debug.LogError("❌ No hay NetworkManager en la escena.");
            return;
        }

        // Asegura Scene Management habilitado
        if (nm.NetworkConfig != null)
            nm.NetworkConfig.EnableSceneManagement = true;

        transport = nm.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("❌ Falta UnityTransport en el NetworkManager.");
        }

        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        // Configurar botón de cerrar error
        if (botonCerrarError != null)
        {
            botonCerrarError.onClick.AddListener(CerrarMensajeError);
        }
        
        // Suscribirse a los logs para detectar errores de socket
        Application.logMessageReceived += OnLogMessageReceived;
        
        // Verificar configuración del texto de estado
        VerificarConfiguracionTexto();
        
        // Cambiar el texto inicial de weasConectarse
        CambiarTextoInicial();

        // IMPORTANTE: Desactiva "Connection Approval" en el NetworkManager (Inspector)
        // para que no se requiera callback de aprobación.
    }

    private void OnDestroy()
    {
        if (nm != null)
        {
            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        
        // Desuscribirse de los logs
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    // ===================== Botones / Helpers de Hotspot =====================

    public void UsarIPHotspotAndroid()
    {
        ipAddress = ANDROID_HOTSPOT_IPs[0]; // Usar la primera IP por defecto
        Debug.Log($"📶 IP (Hotspot Android) fijada: {ipAddress}");
    }

    public void UsarIPHotspotWindows()
    {
        ipAddress = WINDOWS_HOTSPOT_IP;
        Debug.Log($"📶 IP (Hotspot Windows) fijada: {ipAddress}");
    }

    public string MostrarIPLocalDelDispositivo()
    {
        var ip = NetHelpers.GetLocalIPv4();
        Debug.Log($"🔎 IP local detectada: {ip ?? "desconocida"}");
        return ip;
    }
    
    /// <summary>
    /// Obtiene la IP real del dispositivo para el servidor
    /// </summary>
    private string ObtenerIPReal()
    {
        try
        {
            // Intentar obtener la IP local
            string ipLocal = NetHelpers.GetLocalIPv4();
            if (!string.IsNullOrEmpty(ipLocal) && !ipLocal.StartsWith("127."))
            {
                Debug.Log($"🌐 IP real detectada: {ipLocal}");
                return ipLocal;
            }
            
            // Si no se puede obtener la IP local, usar la configurada
            Debug.LogWarning($"⚠️ No se pudo obtener IP real, usando: {ipAddress}");
            return ipAddress;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error al obtener IP real: {e.Message}");
            return ipAddress;
        }
    }

    public void ConfigurarIP(string nuevaIP)
    {
        ipAddress = nuevaIP;
        Debug.Log($"IP configurada: {ipAddress}");
    }

    public void ConfigurarPuerto(int nuevoPuerto)
    {
        port = Mathf.Clamp(nuevoPuerto, 1, 65535);
        Debug.Log($"Puerto configurado: {port}");
    }

    // ===================== Host / Cliente =====================

    public void CrearServidor()
    {
        if (!AsegurarNM()) return;
        if (!ConfigurarTransporte()) return;

        // Limpiar texto inicial
        ActualizarTextoEstado("");

        // Obtener la IP real del dispositivo
        string ipReal = ObtenerIPReal();
        if (!string.IsNullOrEmpty(ipReal))
        {
            ipAddress = ipReal;
            transport.SetConnectionData(ipAddress, (ushort)port);
        }

        PlayerPrefs.SetString("GameMode", "Host");
        PlayerPrefs.SetString("ServerIP", ipAddress);
        PlayerPrefs.SetInt("ServerPort", port);

        AgregarMensajeEstado("🏠 Iniciando servidor...");
        AgregarMensajeEstado($"📡 IP del servidor: {ipAddress}");
        AgregarMensajeEstado($"🔌 Puerto: {port}");
        
        if (!nm.StartHost())
        {
            MostrarMensajeError("No se pudo iniciar el Host.");
            AgregarMensajeEstado("❌ Error: No se pudo iniciar el servidor");
            return;
        }

        Debug.Log($"🏠 Host en {ipAddress}:{port}");
        AgregarMensajeEstado($"✅ Servidor iniciado correctamente");
        AgregarMensajeEstado($"🌐 Otros dispositivos pueden conectarse a:");
        AgregarMensajeEstado($"   IP: {ipAddress}");
        AgregarMensajeEstado($"   Puerto: {port}");

        if (nm.SceneManager != null)
        {
            AgregarMensajeEstado("🎮 Cambiando a escena MultiGameplay...");
            nm.SceneManager.LoadScene(gameplayScene, LoadSceneMode.Single);
            Debug.Log("🎮 Host cargando gameplay (sincronizado).");
        }
        else
        {
            AgregarMensajeEstado("🎮 Cambiando a escena MultiGameplay...");
            SceneManager.LoadScene(gameplayScene);
            Debug.LogWarning("⚠️ NetworkSceneManager no disponible. Carga local.");
        }
    }

    public void ConectarComoCliente()
    {
        if (pendingClientConnect) 
        {
            Debug.Log("⚠️ Ya hay una conexión en progreso");
            return;
        }
        
        if (!AsegurarNM()) return;
        if (!ConfigurarTransporte()) return;

        // Iniciar la búsqueda de IPs de Android Hotspot
        StartCoroutine(ProbarIPsAndroidHotspot());
    }
    
    /// <summary>
    /// Corrutina que prueba todas las IPs de Android Hotspot secuencialmente
    /// </summary>
    private IEnumerator ProbarIPsAndroidHotspot()
    {
        pendingClientConnect = true;
        bool conexionExitosa = false;
        
        // Limpiar texto inicial
        ActualizarTextoEstado("");
        
        // Crear lista de IPs incluyendo la IP actual del host si está disponible
        var ipsParaProbar = new List<string>(ANDROID_HOTSPOT_IPs);
        
        // Agregar la IP del host si está disponible
        string ipHost = PlayerPrefs.GetString("ServerIP", "");
        if (!string.IsNullOrEmpty(ipHost) && !ipsParaProbar.Contains(ipHost))
        {
            ipsParaProbar.Insert(0, ipHost); // Prioridad a la IP del host
            AgregarMensajeEstado($"🎯 IP del host detectada: {ipHost}");
        }
        
        Debug.Log($"🔍 Iniciando búsqueda de servidor en {ipsParaProbar.Count} IPs...");
        AgregarMensajeEstado($"🔍 Iniciando búsqueda de servidor en {ipsParaProbar.Count} IPs...");
        
        for (int i = 0; i < ipsParaProbar.Count; i++)
        {
            string ipActual = ipsParaProbar[i];
            Debug.Log($"🔗 Probando IP {i + 1}/{ipsParaProbar.Count}: {ipActual}:{port}");
            AgregarMensajeEstado($"🔗 Probando IP {i + 1}/{ipsParaProbar.Count}: {ipActual}:{port}");
            
            // Configurar la IP actual
            transport.SetConnectionData(ipActual, (ushort)port);
            ipAddress = ipActual;
            
            // Guardar configuración
            PlayerPrefs.SetString("GameMode", "Client");
            PlayerPrefs.SetString("ServerIP", ipActual);
            PlayerPrefs.SetInt("ServerPort", port);
            
            // Intentar conectar
            bool clienteIniciado = nm.StartClient();
            if (!clienteIniciado)
            {
                Debug.LogWarning($"❌ No se pudo iniciar cliente para {ipActual}");
                AgregarMensajeEstado($"❌ No se pudo iniciar cliente para {ipActual}");
                continue;
            }
            
            // Esperar un tiempo para que se establezca la conexión
            AgregarMensajeEstado($"⏳ Conectando a {ipActual}:{port}...");
            yield return new WaitForSeconds(2f);
            
            // Verificar si la conexión fue exitosa
            if (nm.IsClient && nm.IsConnectedClient)
            {
                Debug.Log($"✅ ¡Conexión exitosa! Servidor encontrado en {ipActual}:{port}");
                AgregarMensajeEstado($"✅ ¡Conexión exitosa! Servidor encontrado en {ipActual}:{port}");
                conexionExitosa = true;
                break;
            }
            else
            {
                // Verificar si es un error de socket específico
                string mensajeError = lastDisconnectReason ?? "Conexión fallida";
                if (EsErrorDeSocket(mensajeError))
                {
                    Debug.LogWarning($"🚫 Error de socket en {ipActual}:{port} - {mensajeError}");
                    AgregarMensajeEstado($"🚫 Error de socket en {ipActual}:{port}");
                    AgregarMensajeEstado($"   Motivo: {mensajeError}");
                }
                else
                {
                    Debug.LogWarning($"❌ No se pudo conectar a {ipActual}:{port}");
                    AgregarMensajeEstado($"❌ No se pudo conectar a {ipActual}:{port}");
                }
                
                // Desconectar antes de probar la siguiente IP
                if (nm.IsClient)
                {
                    nm.Shutdown();
                    yield return new WaitForSeconds(0.5f); // Esperar un momento antes de la siguiente IP
                }
            }
        }
        
        pendingClientConnect = false;
        
        if (conexionExitosa)
        {
            Debug.Log("🎮 Cambiando a escena MultiGameplay como Cliente");
            AgregarMensajeEstado("🎮 Cambiando a escena MultiGameplay...");
            SceneManager.LoadScene(gameplayScene);
        }
        else
        {
            Debug.LogError("❌ No se pudo conectar a ningún servidor en las IPs de Android Hotspot");
            AgregarMensajeEstado("❌ No se encontró ningún servidor disponible");
            AgregarMensajeEstado($"IPs probadas: {string.Join(", ", ANDROID_HOTSPOT_IPs)}");
            AgregarMensajeEstado("Verifica que el servidor esté ejecutándose");
            string mensajeDetallado = $"No se encontró ningún servidor disponible.\n\nIPs probadas:\n{string.Join("\n", ANDROID_HOTSPOT_IPs)}\n\nAsegúrate de que:\n• El servidor esté ejecutándose\n• Ambos dispositivos estén en la misma red\n• El puerto {port} esté disponible";
            MostrarMensajeError(mensajeDetallado);
        }
    }

    public void UnirseAPartida() => ConectarComoCliente();

    public void Desconectar()
    {
        if (nm != null && nm.IsListening)
        {
            nm.Shutdown();
        }
        StopAllCoroutines();
        pendingClientConnect = false;

        if (!string.IsNullOrEmpty(menuScene))
            SceneManager.LoadScene(menuScene);

        Debug.Log("🔌 Desconectado y vuelta al menú.");
    }

    public void MostrarMenuPrincipal()
    {
        if (menuPrincipal != null && menuMultiplayer != null)
        {
            menuMultiplayer.SetActive(false);
            menuPrincipal.SetActive(true);
        }
    }

    // ===================== Callbacks =====================

    private void OnClientConnected(ulong clientId)
    {
        if (nm == null) return;

        if (nm.IsHost && clientId == nm.LocalClientId)
        {
            Debug.Log("✅ Host conectado (local).");
            AgregarMensajeEstado("✅ Host conectado (local).");
        }
        else if (nm.IsClient && clientId == nm.LocalClientId)
        {
            Debug.Log("✅ Cliente conectado. Esperando sincronización de escena…");
            AgregarMensajeEstado("✅ Cliente conectado. Esperando sincronización de escena…");
            pendingClientConnect = false;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (nm == null) return;

        if (nm.IsClient && clientId == nm.LocalClientId && !nm.IsHost)
        {
            pendingClientConnect = false;

            // Algunas versiones traen DisconnectReason, otras no. Esto es seguro.
            var reasonField = GetDisconnectReasonSafe();
            if (!string.IsNullOrEmpty(reasonField))
                lastDisconnectReason = reasonField;

            string reason = string.IsNullOrEmpty(lastDisconnectReason)
                ? "Desconectado del servidor."
                : lastDisconnectReason;

            Debug.LogWarning($"⚠️ Cliente desconectado. Motivo: {reason}");
            AgregarMensajeEstado($"⚠️ Cliente desconectado. Motivo: {reason}");
            MostrarMensajeError(reason);
        }
    }

    // ===================== Utilidades =====================

    private bool AsegurarNM()
    {
        if (nm == null)
            nm = NGO.NetworkManager.Singleton ?? FindObjectOfType<NGO.NetworkManager>();

        if (nm == null)
        {
            Debug.LogError("❌ No hay NetworkManager en la escena.");
            return false;
        }

        if (transport == null)
            transport = nm.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("❌ Falta UnityTransport en el NetworkManager.");
            return false;
        }

        return true;
    }

    private bool ConfigurarTransporte()
    {
        try
        {
            transport.SetConnectionData(ipAddress, (ushort)port);
            Debug.Log($"🌐 Transporte configurado: {ipAddress}:{port}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error configurando transporte: {e.Message}");
            MostrarMensajeError($"Error configurando red: {e.Message}");
            return false;
        }
    }

    private IEnumerator EsperarConexionCliente(float timeout)
    {
        float t = 0f;

        while (t < timeout)
        {
            if (nm == null) yield break;

            if (nm.IsConnectedClient)
            {
                Debug.Log("✅ Cliente conectado. El Host sincronizará la escena.");
                yield break;
            }

            if (!nm.IsClient || !nm.IsListening)
            {
                var reason = GetDisconnectReasonSafe();
                if (string.IsNullOrEmpty(reason))
                    reason = "No se pudo conectar al servidor.";
                MostrarMensajeError(reason);
                yield break;
            }

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.LogWarning("⏱️ Timeout de conexión del cliente.");
        nm.Shutdown();
        MostrarMensajeError("Tiempo de conexión agotado.");
    }

    public void MostrarMensajeError(string mensaje)
    {
        Debug.LogError($"❌ {mensaje}");
        
        // Mostrar mensaje en pantalla si hay UI configurada
        if (panelError != null && textoError != null)
        {
            textoError.text = mensaje;
            textoError.color = colorTextoError; // Establecer color rojo
            panelError.SetActive(true);
            Debug.Log("📱 Mensaje de error mostrado en pantalla (texto en rojo)");
        }
        else if (mensajeErrorPrefab != null)
        {
            Instantiate(mensajeErrorPrefab);
            Debug.Log("📱 Mensaje de error mostrado con prefab");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay UI configurada para mostrar mensajes de error");
        }
    }
    
    /// <summary>
    /// Cierra el mensaje de error en pantalla
    /// </summary>
    public void CerrarMensajeError()
    {
        if (panelError != null)
        {
            panelError.SetActive(false);
            Debug.Log("❌ Mensaje de error cerrado");
        }
    }
    
    /// <summary>
    /// Configura el color del texto de error
    /// </summary>
    public void ConfigurarColorTextoError(Color nuevoColor)
    {
        colorTextoError = nuevoColor;
        Debug.Log($"🎨 Color de texto de error configurado a: {nuevoColor}");
    }
    
    /// <summary>
    /// Verifica si el texto de estado está configurado correctamente
    /// </summary>
    public void VerificarConfiguracionTexto()
    {
        if (textoEstadoConexion == null)
        {
            Debug.LogError("❌ textoEstadoConexion NO está asignado en el Inspector");
            Debug.LogError("   Asigna el componente Text de 'weasConectarse' al campo 'Texto Estado Conexion'");
            Debug.LogError("   Buscando automáticamente el componente Text...");
            
            // Intentar encontrar el texto automáticamente
            textoEstadoConexion = FindObjectOfType<UnityEngine.UI.Text>();
            if (textoEstadoConexion != null)
            {
                Debug.Log("✅ Texto encontrado automáticamente: " + textoEstadoConexion.name);
            }
            else
            {
                Debug.LogError("❌ No se pudo encontrar ningún componente Text en la escena");
            }
        }
        else
        {
            Debug.Log("✅ textoEstadoConexion está configurado correctamente");
            Debug.Log($"   GameObject: {textoEstadoConexion.name}");
            Debug.Log($"   Texto actual: '{textoEstadoConexion.text}'");
            Debug.Log($"   Componente activo: {textoEstadoConexion.gameObject.activeInHierarchy}");
        }
    }
    
    /// <summary>
    /// Método público para probar la actualización del texto
    /// </summary>
    public void ProbarActualizacionTexto()
    {
        AgregarMensajeEstado("🧪 Prueba de actualización de texto");
        AgregarMensajeEstado("   Si ves este mensaje, el texto se está actualizando correctamente");
    }
    
    /// <summary>
    /// Cambia el texto inicial de weasConectarse
    /// </summary>
    private void CambiarTextoInicial()
    {
        // Intentar con TextMeshPro primero
        if (textoEstadoConexionTMP != null)
        {
            textoEstadoConexionTMP.text = "🔌 Listo para conectar";
            Debug.Log("📝 Texto inicial cambiado (TMP) a: '🔌 Listo para conectar'");
            return;
        }
        
        // Intentar con Text normal
        if (textoEstadoConexion != null)
        {
            textoEstadoConexion.text = "🔌 Listo para conectar";
            textoEstadoConexion.SetAllDirty();
            Debug.Log("📝 Texto inicial cambiado (Text) a: '🔌 Listo para conectar'");
        }
        else
        {
            Debug.LogWarning("⚠️ No se puede cambiar el texto inicial - ningún componente de texto asignado");
        }
    }
    
    /// <summary>
    /// Actualiza el texto de estado de conexión
    /// </summary>
    private void ActualizarTextoEstado(string mensaje)
    {
        // MÉTODO AGRESIVO: Buscar TODOS los componentes de texto en la escena
        var todosLosTextos = FindObjectsOfType<UnityEngine.UI.Text>();
        var todosLosTMP = FindObjectsOfType<TMPro.TextMeshProUGUI>();
        
        // Buscar específicamente el que contiene "weasConectarse" o "Para conectarse"
        foreach (var tmp in todosLosTMP)
        {
            if (tmp.name.ToLower().Contains("weasconectarse") || 
                tmp.text.Contains("Para conectarse") || 
                tmp.text.Contains("conectar"))
            {
                tmp.text = mensaje;
                return;
            }
        }
        
        foreach (var texto in todosLosTextos)
        {
            if (texto.name.ToLower().Contains("weasconectarse") || 
                texto.text.Contains("Para conectarse") || 
                texto.text.Contains("conectar"))
            {
                texto.text = mensaje;
                texto.SetAllDirty();
                return;
            }
        }
        
        // Si no encuentra nada, usar el primer texto disponible
        if (todosLosTMP.Length > 0)
        {
            todosLosTMP[0].text = mensaje;
            return;
        }
        
        if (todosLosTextos.Length > 0)
        {
            todosLosTextos[0].text = mensaje;
            todosLosTextos[0].SetAllDirty();
        }
    }
    
    /// <summary>
    /// Agrega un mensaje al texto de estado (mantiene historial)
    /// </summary>
    private void AgregarMensajeEstado(string mensaje)
    {
        // MÉTODO AGRESIVO: Buscar TODOS los componentes de texto en la escena
        var todosLosTextos = FindObjectsOfType<UnityEngine.UI.Text>();
        var todosLosTMP = FindObjectsOfType<TMPro.TextMeshProUGUI>();
        
        // Buscar específicamente el que contiene "weasConectarse" o "Para conectarse"
        UnityEngine.UI.Text textoEncontrado = null;
        TMPro.TextMeshProUGUI tmpEncontrado = null;
        
        foreach (var texto in todosLosTextos)
        {
            if (texto.name.ToLower().Contains("weasconectarse") || 
                texto.text.Contains("Para conectarse") || 
                texto.text.Contains("conectar"))
            {
                textoEncontrado = texto;
                break;
            }
        }
        
        foreach (var tmp in todosLosTMP)
        {
            if (tmp.name.ToLower().Contains("weasconectarse") || 
                tmp.text.Contains("Para conectarse") || 
                tmp.text.Contains("conectar"))
            {
                tmpEncontrado = tmp;
                break;
            }
        }
        
        // Actualizar el texto encontrado
        if (tmpEncontrado != null)
        {
            string mensajeActual = tmpEncontrado.text;
            if (string.IsNullOrEmpty(mensajeActual))
            {
                tmpEncontrado.text = mensaje;
            }
            else
            {
                tmpEncontrado.text = mensajeActual + "\n" + mensaje;
            }
            return;
        }
        
        if (textoEncontrado != null)
        {
            string mensajeActual = textoEncontrado.text;
            if (string.IsNullOrEmpty(mensajeActual))
            {
                textoEncontrado.text = mensaje;
            }
            else
            {
                textoEncontrado.text = mensajeActual + "\n" + mensaje;
            }
            textoEncontrado.SetAllDirty();
            return;
        }
        
        // Si no encuentra nada, usar el primer texto disponible
        if (todosLosTMP.Length > 0)
        {
            var tmp = todosLosTMP[0];
            string mensajeActual = tmp.text;
            if (string.IsNullOrEmpty(mensajeActual))
            {
                tmp.text = mensaje;
            }
            else
            {
                tmp.text = mensajeActual + "\n" + mensaje;
            }
            return;
        }
        
        if (todosLosTextos.Length > 0)
        {
            var texto = todosLosTextos[0];
            string mensajeActual = texto.text;
            if (string.IsNullOrEmpty(mensajeActual))
            {
                texto.text = mensaje;
            }
            else
            {
                texto.text = mensajeActual + "\n" + mensaje;
            }
            texto.SetAllDirty();
        }
    }
    
    /// <summary>
    /// Detecta errores de socket específicos y los maneja
    /// </summary>
    private bool EsErrorDeSocket(string mensaje)
    {
        // Verificar si el mensaje es null o vacío
        if (string.IsNullOrEmpty(mensaje))
        {
            return false;
        }
        
        return mensaje.Contains("All socket receive requests were marked as failed") ||
               mensaje.Contains("socket itself has failed") ||
               mensaje.Contains("SocketException") ||
               mensaje.Contains("Connection refused") ||
               mensaje.Contains("Connection timeout") ||
               mensaje.Contains("Network unreachable");
    }
    
    /// <summary>
    /// Detecta errores de socket en los logs de Unity
    /// </summary>
    private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        // Solo procesar errores y excepciones
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (EsErrorDeSocket(logString))
            {
                Debug.LogWarning($"🚨 ERROR DE SOCKET DETECTADO: {logString}");
                AgregarMensajeEstado($"🚨 Error de socket detectado");
                AgregarMensajeEstado($"   {logString}");
                
                // Actualizar la razón de desconexión
                lastDisconnectReason = logString;
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && nm != null && (nm.IsHost || nm.IsClient))
        {
            Debug.Log("✅ Conexión activa (focus).");
        }
    }

    // Versions-safe: intenta leer DisconnectReason si existe en tu versión
    private string GetDisconnectReasonSafe()
    {
        try
        {
            // En NGO 1.5+ existe nm.DisconnectReason
            return nm != null ? nm.DisconnectReason : null;
        }
        catch
        {
            // En versiones sin esa propiedad simplemente no la usamos
            return null;
        }
    }

    // ------------------- Helper de red -------------------
    private static class NetHelpers
    {
        public static string GetLocalIPv4()
        {
            try
            {
                return Dns
                    .GetHostAddresses(Dns.GetHostName())
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                    .ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
