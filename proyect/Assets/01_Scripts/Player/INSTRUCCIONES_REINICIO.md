# Sistema de Reinicio de Escena - Instrucciones

## 🎮 Funcionamiento
Cuando el jugador muere (vida llega a 0), la escena se reinicia automáticamente después de unos segundos.

## 📋 Scripts Creados

### 1. SimpleGameManager.cs (Recomendado)
- **Ubicación**: `Assets/01_Scripts/Player/SimpleGameManager.cs`
- **Función**: Reinicia la escena cuando el jugador muere
- **Características**:
  - Delay configurable (3 segundos por defecto)
  - Countdown en consola
  - Reinicio automático

### 2. GameManager.cs (Avanzado)
- **Ubicación**: `Assets/01_Scripts/Player/GameManager.cs`
- **Función**: Sistema completo de gestión de juego
- **Características**:
  - UI de muerte opcional
  - Ralentización del tiempo
  - Control manual de reinicio

## ⚙️ Configuración Requerida

### Opción 1: SimpleGameManager (Fácil)
1. **Crear un GameObject vacío** en la escena
2. **Nombrarlo "GameManager"**
3. **Agregar el componente SimpleGameManager**
4. **Configurar el delay** (3 segundos por defecto)

### Opción 2: GameManager (Avanzado)
1. **Crear un GameObject vacío** en la escena
2. **Nombrarlo "GameManager"**
3. **Agregar el componente GameManager**
4. **Configurar parámetros**:
   - `restartDelay`: 2 segundos
   - `deathUI`: (opcional) UI de muerte

## 🎯 Flujo de Muerte y Reinicio

1. **Jugador recibe daño** → Vida baja
2. **Vida llega a 0** → Health.Die() se ejecuta
3. **Se dispara evento OnDeath** → GameManager lo detecta
4. **Se muestra mensaje** → "GAME OVER! Reiniciando en X segundos..."
5. **Countdown en consola** → Muestra tiempo restante
6. **Reinicio automático** → SceneManager.LoadScene()

## 🔧 Parámetros Configurables

### SimpleGameManager
- `restartDelay`: Tiempo antes de reiniciar (3 segundos)

### GameManager
- `restartDelay`: Tiempo antes de reiniciar (2 segundos)
- `deathUI`: GameObject de UI de muerte (opcional)

## 🐛 Debug y Logs
- `✅ GameManager conectado al jugador` - Conexión exitosa
- `💀 ¡GAME OVER! Reiniciando en X segundos...` - Muerte detectada
- `💀 Reiniciando en X segundos...` - Countdown
- `🔄 Reiniciando escena...` - Reinicio ejecutado

## ✅ Verificación
1. **GameManager en la escena** ✓
2. **Jugador tiene tag "Player"** ✓
3. **Jugador tiene componente Health** ✓
4. **Fantasma tiene componente GhostDamage** ✓

## 🎮 Experiencia de Juego
- **Muerte**: Jugador deja de moverse
- **Countdown**: 3 segundos de espera
- **Reinicio**: Escena vuelve al inicio
- **Continuidad**: Puedes jugar de nuevo inmediatamente

¡El sistema está listo para usar!
