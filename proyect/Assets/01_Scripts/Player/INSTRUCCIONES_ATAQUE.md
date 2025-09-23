# Sistema de Ataque del Personaje - Instrucciones

## 🎯 Objetivo
Implementar un sistema de daño para que los golpes del personaje (Enter) afecten a los fantasmas.

## 📋 Scripts Creados

### 1. PlayerAttackDamage.cs
- **Función**: Sistema de daño por área (esfera)
- **Uso**: Alternativa simple que no requiere hitbox físico
- **Características**:
  - Detecta enemigos en rango y ángulo
  - Aplica daño a todos los enemigos válidos
  - Visualización de debug

### 2. PlayerHitbox.cs
- **Función**: Hitbox físico para ataques
- **Uso**: Sistema más preciso con collider
- **Características**:
  - Collider como trigger
  - Control de cooldown entre hits
  - Opción de golpear múltiples enemigos

### 3. PlayerAttackController.cs
- **Función**: Controla la sincronización del hitbox con la animación
- **Uso**: Conecta el sistema de ataque con el hitbox
- **Características**:
  - Activa/desactiva hitbox en momentos específicos
  - Configuración de delay y duración
  - Sincronización con SimpleAttacker

### 4. PlayerAttackSetup.cs
- **Función**: Configuración automática del sistema
- **Uso**: Facilita la implementación
- **Características**:
  - Crea hitbox automáticamente
  - Configura todos los componentes
  - Preview visual en el editor

## ⚙️ Implementación

### Opción 1: Sistema Simple (Recomendado para empezar)

1. **Agregar PlayerAttackDamage al personaje**:
   - Selecciona el GameObject del personaje
   - Agrega el componente `PlayerAttackDamage`
   - Configura los valores:
     - `Attack Damage`: 10 (daño por golpe)
     - `Attack Range`: 2 (rango de ataque)
     - `Attack Angle`: 120 (ángulo de ataque)
     - `Enemy Layer Mask`: Selecciona la capa de enemigos

2. **Configurar la capa de enemigos**:
   - Ve a `Edit > Project Settings > Tags and Layers`
   - Crea una capa llamada "Enemy"
   - Asigna esta capa a todos los fantasmas

### Opción 2: Sistema con Hitbox Físico (Más preciso)

1. **Configurar automáticamente**:
   - Selecciona el GameObject del personaje
   - Agrega el componente `PlayerAttackSetup`
   - Haz clic derecho en el componente
   - Selecciona "Configurar Sistema de Ataque"

2. **Configurar manualmente**:
   - Crear un GameObject hijo llamado "AttackHitbox"
   - Agregar un BoxCollider como Trigger
   - Agregar el componente `PlayerHitbox`
   - Agregar `PlayerAttackController` al personaje
   - Asignar referencias en el inspector

## 🎮 Configuración del Personaje

### Verificar que el personaje tenga:
1. **SimpleAttacker** - Para detectar cuando ataca
2. **AnimatorDriver** - Para las animaciones
3. **Health** - Para la salud del personaje
4. **Tag "Player"** - Para identificación

### Verificar que los fantasmas tengan:
1. **EnemyCommon** - Para recibir daño
2. **Capa "Enemy"** - Para detección
3. **Collider** - Para detección física

## 🔧 Configuración Avanzada

### PlayerAttackDamage (Sistema Simple)
```csharp
attackDamage = 10f;        // Daño por golpe
attackRange = 2f;          // Rango de detección
attackAngle = 120f;        // Ángulo de ataque
enemyLayerMask = Enemy;    // Capa de enemigos
```

### PlayerHitbox (Sistema Físico)
```csharp
damageAmount = 10f;                    // Daño por golpe
canHitMultipleEnemies = false;         // Un golpe por ataque
hitCooldown = 0.1f;                   // Tiempo entre hits
```

### PlayerAttackController
```csharp
attackDamage = 10f;        // Daño del ataque
hitboxDelay = 0.2f;        // Delay antes de activar hitbox
hitboxDuration = 0.5f;     // Duración del hitbox activo
```

## 🐛 Solución de Problemas

### El personaje no hace daño:
1. Verifica que tenga `SimpleAttacker`
2. Verifica que los fantasmas tengan `EnemyCommon`
3. Verifica la configuración de capas
4. Revisa los logs de debug

### El hitbox no se activa:
1. Verifica que `PlayerAttackController` esté configurado
2. Verifica que el hitbox esté desactivado inicialmente
3. Verifica las referencias en el inspector

### Los fantasmas no reciben daño:
1. Verifica que tengan el componente `EnemyCommon`
2. Verifica que no estén muertos
3. Verifica la configuración de capas
4. Revisa los logs de debug

## 📊 Debug

### Visualización en Escena:
- **PlayerAttackDamage**: Muestra esfera de rango y ángulo
- **PlayerHitbox**: Muestra el collider del hitbox
- **PlayerAttackSetup**: Muestra preview del hitbox

### Logs de Debug:
- Ataques iniciados/terminados
- Enemigos golpeados
- Daño aplicado
- Errores de configuración

## 🎯 Próximos Pasos

1. **Probar el sistema básico** con PlayerAttackDamage
2. **Ajustar valores** según el balance del juego
3. **Implementar efectos visuales** (partículas, sonidos)
4. **Agregar feedback** (cámara shake, efectos de pantalla)
5. **Optimizar rendimiento** si es necesario

## 💡 Consejos

- **Empieza simple**: Usa PlayerAttackDamage primero
- **Ajusta valores**: Prueba diferentes rangos y daños
- **Usa debug**: Activa las visualizaciones para entender el sistema
- **Testa con fantasmas**: Asegúrate de que el daño funcione correctamente
