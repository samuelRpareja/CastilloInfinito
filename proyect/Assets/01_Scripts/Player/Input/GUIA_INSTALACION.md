# 🎮 Guía de Instalación - Joystick Virtual para Android

## 📋 **Pasos para Configurar el Sistema Completo**

### **Paso 1: Preparar el Canvas**

#### **Opción A - Automática (Recomendada):**
1. Crea un GameObject vacío en la escena
2. Agrega el componente `CanvasSetup`
3. El Canvas se creará automáticamente

#### **Opción B - Manual:**
1. En la jerarquía: Click derecho → **UI → Canvas**
2. Selecciona el Canvas y configura:
   - **Render Mode:** Screen Space - Overlay
   - **UI Scale Mode:** Scale With Screen Size
   - **Reference Resolution:** 1920 x 1080

### **Paso 2: Configurar el Jugador**

#### **Método 1 - Con tu Prefab (Recomendado):**
1. Crea un GameObject vacío en la escena
2. Agrega el componente `PlayerSetup`
3. En el Inspector:
   - **Player Prefab:** Arrastra tu prefab del jugador
   - **Spawn Point:** (Opcional) Arrastra un Transform para la posición de spawn
   - **Target Canvas:** Arrastra el Canvas creado
4. El sistema configurará todo automáticamente

#### **Método 2 - Manual:**
1. Arrastra tu prefab del jugador a la escena
2. Verifica que tenga el componente `PlayerController`
3. Agrega el componente `AndroidJoystickSetup` al prefab
4. Configura las referencias en el Inspector

### **Paso 3: Verificar la Configuración**

#### **Componentes Necesarios en tu Prefab:**
- ✅ `PlayerController`
- ✅ `SimpleMovementController`
- ✅ `AnimatorDriver`
- ✅ `SimpleAttacker`
- ✅ `Health`

#### **Componentes que se Agregarán Automáticamente:**
- ✅ `AdaptivePlayerInput`
- ✅ `PlayerInputManager`
- ✅ `HybridInputProvider`
- ✅ `JoystickInputProvider`

### **Paso 4: Configuración del Joystick**

El joystick se creará automáticamente con estas configuraciones por defecto:
- **Posición:** Esquina inferior izquierda (-200, -200)
- **Tamaño:** 150x150 píxeles
- **Colores:** Fondo semi-transparente, handle más opaco
- **Solo visible en Android**

## 🎯 **Configuración Rápida (1 Minuto)**

### **Para Configuración Súper Rápida:**

1. **Crea un GameObject vacío** en tu escena
2. **Agrega `PlayerSetup`** como componente
3. **Arrastra tu prefab del jugador** al campo "Player Prefab"
4. **¡Listo!** Todo se configurará automáticamente

### **Si ya tienes un Canvas:**
1. **Crea un GameObject vacío**
2. **Agrega `AndroidJoystickSetup`**
3. **Arrastra tu prefab del jugador** al campo "Player Controller"
4. **Arrastra tu Canvas** al campo "Target Canvas"

## 🔧 **Personalización**

### **Cambiar Posición del Joystick:**
```csharp
// En el Inspector de AndroidJoystickSetup
Posicion Joystick: X = -300, Y = -300  // Esquina inferior izquierda
```

### **Cambiar Tamaño del Joystick:**
```csharp
// En el Inspector de AndroidJoystickSetup
Tamano Joystick: 200  // Más grande
```

### **Cambiar Colores:**
- Modifica los colores en `JoystickUISetup`
- O crea tu propio sprite para el joystick

## 🧪 **Testing**

### **En el Editor:**
- Presiona **J** para alternar entre joystick y teclado
- El joystick se simula con el mouse

### **En Android:**
- El joystick aparece automáticamente
- Toca y arrastra para mover
- Toca fuera del joystick para atacar

## ❗ **Solución de Problemas**

### **El joystick no aparece:**
1. Verifica que hay un Canvas en la escena
2. Verifica que estás ejecutando en Android
3. Verifica que `JoystickUISetup` está configurado

### **El jugador no se mueve:**
1. Verifica que tu prefab tiene `PlayerController`
2. Verifica que `AdaptivePlayerInput` está agregado
3. Verifica que el joystick está activo

### **Error de compilación:**
1. Verifica que todos los scripts están en las carpetas correctas
2. Verifica que no hay errores de sintaxis
3. Revisa la consola de Unity para errores específicos

## 📱 **Configuración para Diferentes Resoluciones**

### **Para Pantallas Pequeñas:**
```csharp
Reference Resolution: 1080 x 1920
Joystick Size: 120
Joystick Position: (-150, -150)
```

### **Para Pantallas Grandes:**
```csharp
Reference Resolution: 1440 x 2560
Joystick Size: 180
Joystick Position: (-250, -250)
```

## 🎨 **Personalización Avanzada**

### **Crear tu Propio Sprite del Joystick:**
1. Crea un sprite circular en tu editor de imágenes
2. Importa a Unity
3. Modifica `JoystickUISetup.cs` para usar tu sprite

### **Cambiar el Comportamiento del Input:**
- Modifica `VirtualJoystick.cs` para ajustar sensibilidad
- Modifica `AdaptivePlayerInput.cs` para cambiar la lógica de input

## ✅ **Checklist Final**

- [ ] Canvas creado y configurado
- [ ] Prefab del jugador asignado
- [ ] PlayerController configurado
- [ ] Sistema de joystick activado
- [ ] Testing en Android realizado
- [ ] Posición y tamaño del joystick ajustados
- [ ] Colores personalizados (opcional)

¡Con estos pasos tendrás el joystick funcionando perfectamente en Android! 🎮
