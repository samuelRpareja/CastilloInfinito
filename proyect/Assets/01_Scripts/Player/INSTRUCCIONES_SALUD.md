# Sistema de Salud - Instrucciones de Configuración

## 📋 Componentes Creados

### 1. Health.cs
- **Ubicación**: `Assets/01_Scripts/Player/Health.cs`
- **Función**: Maneja la vida del jugador, daño, curación y muerte
- **Características**:
  - Vida máxima configurable (10 por defecto)
  - Reducción de daño (0-1, 0 = sin reducción)
  - Eventos para cambios de vida y muerte
  - Métodos para curar y hacer daño

### 2. GhostDamage.cs
- **Ubicación**: `Assets/01_Scripts/Enemy/GhostDamage.cs`
- **Función**: Hace que el fantasma dañe al jugador al tocarlo
- **Características**:
  - Daño configurable (5 por defecto)
  - Cooldown entre ataques (1 segundo)
  - Detección por trigger y collision

### 3. HealthUI.cs
- **Ubicación**: `Assets/01_Scripts/Player/HealthUI.cs`
- **Función**: Muestra la vida del jugador en la UI
- **Características**:
  - Barra de vida con colores (verde/amarillo/rojo)
  - Texto con vida actual/máxima
  - Actualización automática

## ⚙️ Configuración Requerida

### 1. Configurar el Jugador
1. **Agregar el componente Health** al prefab del jugador
2. **Asignar el tag "Player"** al jugador si no lo tiene
3. **Configurar valores**:
   - `maxHP`: 10 (vida máxima)
   - `currentHP`: 10 (vida inicial)
   - `damageReduction`: 0 (sin reducción de daño)

### 2. Configurar el Fantasma
1. **Agregar el componente GhostDamage** al prefab del fantasma
2. **Configurar valores**:
   - `damageAmount`: 5 (daño por toque)
   - `damageCooldown`: 1 (segundos entre ataques)
3. **Asegurar que tenga Collider2D** (Trigger o Collision)

### 3. Configurar la UI (Opcional)
1. **Crear un Canvas** en la escena
2. **Agregar un Slider** para la barra de vida
3. **Agregar un Text** para mostrar números
4. **Agregar el componente HealthUI** a un GameObject
5. **Asignar referencias** en el inspector

## 🎮 Funcionamiento

### Sistema de Daño
- **El fantasma toca al jugador** → Se activa GhostDamage
- **Se verifica el cooldown** → Evita spam de daño
- **Se reduce la vida** → Health.TakeDamage()
- **Se actualiza la UI** → HealthUI se actualiza automáticamente
- **Vida llega a 0** → Jugador muere y se destruye

### Sistema de Muerte
- **Jugador muere** → Health.Die()
- **Se detiene el movimiento** → PlayerController no responde
- **Se destruye el objeto** → Destroy(gameObject)

## 🔧 Parámetros Configurables

### Health.cs
- `maxHP`: Vida máxima del jugador (10 por defecto)
- `currentHP`: Vida actual (se ajusta automáticamente)
- `damageReduction`: Reducción de daño (0 = sin reducción, 1 = inmune)

### GhostDamage.cs
- `damageAmount`: Daño que hace el fantasma (5 por defecto)
- `damageCooldown`: Tiempo entre ataques (segundos)

## 🐛 Debug
- Los logs aparecerán en la consola mostrando:
  - Daño recibido y vida restante
  - Curaciones
  - Muerte del jugador
  - Errores de configuración

## ✅ Verificación
1. **Jugador tiene componente Health** ✓
2. **Jugador tiene tag "Player"** ✓
3. **Fantasma tiene componente GhostDamage** ✓
4. **Fantasma tiene Collider2D** ✓
5. **UI configurada (opcional)** ✓

¡El sistema está listo para usar!
