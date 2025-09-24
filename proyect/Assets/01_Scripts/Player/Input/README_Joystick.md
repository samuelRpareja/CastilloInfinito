# Sistema de Joystick Virtual para Android

Este sistema permite integrar un joystick virtual para controlar el movimiento del jugador en dispositivos Android.

## Archivos del Sistema

### Scripts Principales
- **`VirtualJoystick.cs`** - Script principal del joystick virtual
- **`AdaptivePlayerInput.cs`** - Input del jugador que se adapta a la plataforma
- **`PlayerInputManager.cs`** - Gestor principal del sistema de input
- **`AndroidJoystickSetup.cs`** - Configurador automático del sistema

### Scripts de Soporte
- **`JoystickInputProvider.cs`** - Provider de input que usa el joystick virtual
- **`HybridInputProvider.cs`** - Provider híbrido que combina teclado y joystick
- **`JoystickUISetup.cs`** - Configurador automático de la UI del joystick

## Instalación Rápida

### Método 1: Configuración Automática (Recomendado)

1. **Agregar el configurador automático:**
   - Crea un GameObject vacío en la escena
   - Agrega el componente `AndroidJoystickSetup`
   - El sistema se configurará automáticamente al iniciar

2. **Configuración manual (opcional):**
   - En el Inspector del `AndroidJoystickSetup`, puedes ajustar:
     - Posición del joystick
     - Tamaño del joystick
     - Canvas objetivo

### Método 2: Configuración Manual

1. **Configurar el PlayerController:**
   - Remover el componente `KeyboardPlayerInput` del PlayerController
   - Agregar el componente `AdaptivePlayerInput`
   - Agregar el componente `PlayerInputManager`

2. **Crear la UI del joystick:**
   - Crear un GameObject con el componente `JoystickUISetup`
   - Asignar el Canvas donde se creará el joystick
   - El joystick se creará automáticamente

## Características

### Funcionalidades del Joystick
- ✅ Detección de toques táctiles
- ✅ Animación suave del handle
- ✅ Zona muerta configurable
- ✅ Sensibilidad ajustable
- ✅ Solo se muestra en Android
- ✅ Detección de ataque por toque fuera del joystick

### Sistema de Input Adaptativo
- ✅ Detección automática de plataforma
- ✅ Cambio dinámico entre teclado y joystick
- ✅ Integración con el sistema de movimiento existente
- ✅ Compatibilidad con el sistema de ataque

## Configuración

### Parámetros del Joystick
```csharp
// En VirtualJoystick
public float joystickRange = 50f;        // Rango de movimiento
public float sensibilidad = 1f;          // Sensibilidad del input
public float deadZone = 0.1f;            // Zona muerta
public bool animarHandle = true;         // Animación del handle
public float animacionVelocidad = 10f;   // Velocidad de animación
```

### Parámetros de la UI
```csharp
// En JoystickUISetup
public Vector2 joystickPosition = new Vector2(-200, -200);  // Posición en pantalla
public float joystickSize = 150f;                           // Tamaño del joystick
public Color backgroundColor = new Color(1f, 1f, 1f, 0.3f); // Color del fondo
public Color handleColor = new Color(1f, 1f, 1f, 0.8f);     // Color del handle
```

## Uso en Código

### Obtener Input del Joystick
```csharp
VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();

// Obtener valores de movimiento
float horizontal = joystick.GetHorizontal();
float vertical = joystick.GetVertical();
Vector2 input = joystick.GetInput();

// Verificar si está siendo usado
bool isPressed = joystick.IsPressed();
float magnitude = joystick.GetMagnitude();
```

### Cambiar Método de Input
```csharp
PlayerInputManager inputManager = FindObjectOfType<PlayerInputManager>();

// Cambiar a joystick
inputManager.ForceJoystickInput();

// Cambiar a teclado
inputManager.ForceKeyboardInput();

// Alternar entre ambos (útil para testing)
inputManager.ToggleInputMethod();
```

## Testing

### En el Editor
- Presiona **J** para alternar entre joystick y teclado
- El joystick se simula con el mouse en el editor
- Usa el botón derecho del mouse para simular toques

### En Android
- El joystick aparece automáticamente
- Toca y arrastra para mover el jugador
- Toca fuera del joystick para atacar

## Solución de Problemas

### El joystick no aparece
1. Verificar que hay un Canvas en la escena
2. Verificar que el `JoystickUISetup` está configurado
3. Verificar que estás ejecutando en Android

### El jugador no se mueve
1. Verificar que el `PlayerController` tiene `AdaptivePlayerInput`
2. Verificar que el `PlayerInputManager` está configurado
3. Verificar que el joystick está activo

### Input no funciona correctamente
1. Verificar la configuración de `deadZone`
2. Verificar la `sensibilidad` del joystick
3. Verificar que el `joystickRange` es apropiado

## Personalización

### Cambiar Apariencia
- Modifica los colores en `JoystickUISetup`
- Cambia el sprite del joystick (actualmente usa círculos generados)
- Ajusta la posición y tamaño

### Cambiar Comportamiento
- Modifica la `sensibilidad` para input más/menos sensible
- Ajusta la `deadZone` para mayor/menor precisión
- Cambia la `animacionVelocidad` para animaciones más rápidas/lentas

## Integración con Otros Sistemas

El sistema está diseñado para ser modular y compatible con:
- Sistema de movimiento existente (`SimpleMovementController`)
- Sistema de ataque existente (`SimplePlayerAttack`)
- Sistema de animaciones existente (`AnimatorDriver`)
- Sistema de salud existente (`Health`)

No requiere modificaciones en estos sistemas para funcionar.
