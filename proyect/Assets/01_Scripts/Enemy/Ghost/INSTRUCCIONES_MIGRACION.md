# Instrucciones para Migrar el Sistema de Fantasmas

## Problema Identificado
Los fantasmas se quedaban atrapados en las esquinas debido a múltiples scripts compitiendo por controlar su posición y altura.

## Solución Implementada
Se ha creado un sistema unificado que reemplaza múltiples scripts conflictivos con un solo script `GhostBehavior`.

## Archivos Modificados

### 1. Nuevo Script Unificado
- **`GhostBehavior.cs`** - Reemplaza a:
  - `GhostController.cs`
  - `GhostHeightLock.cs`
  - `GhostRoomBounds.cs`
  - `GhostRoomBoundsSetup.cs`

### 2. Scripts Actualizados
- **`GhostChaseState.cs`** - Actualizado para usar `GhostBehavior`
- **`RoomBehaveor.cs`** - Actualizado para usar `GhostBehavior`
- **`PrefabSetupHelper.cs`** - Mejorado para crear spawn points más seguros

### 3. Script de Migración
- **`GhostPrefabMigration.cs`** - Para actualizar prefabs existentes

## Pasos para Aplicar la Migración

### Paso 1: Actualizar el Prefab del Fantasma
1. Abre el prefab `Ghost_Normal.prefab` en Unity
2. Selecciona el GameObject principal del fantasma
3. Agrega el componente `GhostPrefabMigration`
4. Haz clic derecho en el componente y selecciona "Migrar Prefab de Fantasma"
5. Guarda el prefab

### Paso 2: Verificar Configuración
1. Asegúrate de que el fantasma tenga el componente `GhostBehavior`
2. Verifica que no tenga los scripts antiguos:
   - `GhostController`
   - `GhostHeightLock`
   - `GhostRoomBounds`
   - `GhostRoomBoundsSetup`

### Paso 3: Probar en Escena
1. Coloca el fantasma en una habitación
2. Verifica que se mueva correctamente sin quedarse atrapado
3. Revisa los logs de debug para confirmar que los límites se configuran correctamente

## Configuración del GhostBehavior

### Parámetros Importantes
- **`useRoomBounds`** - Activa/desactiva límites de habitación
- **`roomMargin`** - Margen desde los bordes de la habitación
- **`useInitialHeight`** - Usa la altura inicial del fantasma
- **`heightTolerance`** - Tolerancia para corrección de altura

### Debug
- Activa `showDebugBounds` para ver los límites de la habitación en la escena
- Los logs mostrarán información sobre la configuración de límites

## Beneficios de la Nueva Implementación

1. **Sin Conflictos** - Un solo script controla todo el comportamiento
2. **Mejor Rendimiento** - Menos scripts ejecutándose en Update()
3. **Más Fácil de Mantener** - Lógica centralizada
4. **Spawn Points Mejorados** - Los fantasmas spawnean más lejos de los bordes
5. **Debug Mejorado** - Visualización clara de límites y comportamiento

## Solución de Problemas

### Si el fantasma sigue atrapado:
1. Verifica que `useRoomBounds` esté activado
2. Ajusta `roomMargin` para dar más espacio
3. Revisa los logs para ver los límites calculados

### Si el fantasma no respeta la altura:
1. Verifica que `useInitialHeight` esté activado
2. Ajusta `heightTolerance` si es necesario

### Si los spawn points están mal:
1. Regenera los room prefabs usando `PrefabSetupHelper`
2. Verifica que la escala de la habitación sea correcta
