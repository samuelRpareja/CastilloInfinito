# Sistema de Game Over y Reinicio de Escena

## Descripción
Este sistema permite que la escena se reinicie automáticamente cuando el player muere.

## Scripts Incluidos

### 1. SimpleGameOver.cs
**Script principal** - Maneja el reinicio de escena cuando el player muere.

**Cómo usar:**
1. Agregar este script a cualquier GameObject en la escena
2. Configurar el `restartDelay` (tiempo en segundos antes de reiniciar)
3. El script se conecta automáticamente al PlayerProxy

### 2. AutoGameOverSetup.cs
**Script de configuración automática** - Configura el sistema automáticamente.

**Cómo usar:**
1. Agregar este script a cualquier GameObject en la escena
2. Configurar el `restartDelay` deseado
3. El script creará automáticamente un GameOverManager

### 3. GameOverManager.cs
**Script avanzado** - Versión más completa con más opciones.

## Configuración Rápida

### Opción 1: Configuración Manual
1. Crear un GameObject vacío llamado "GameOverManager"
2. Agregar el script `SimpleGameOver` al GameObject
3. Configurar el `restartDelay` (recomendado: 3 segundos)

### Opción 2: Configuración Automática
1. Crear un GameObject vacío llamado "GameOverSetup"
2. Agregar el script `AutoGameOverSetup` al GameObject
3. Configurar el `restartDelay` deseado
4. El sistema se configurará automáticamente

## Testing

### Probar Muerte del Player
- Usar el botón "Test Player Death" en el Inspector del `AutoGameOverSetup`
- O usar el botón "Simulate Player Death" en el Inspector del `GameOverManager`

### Verificar Funcionamiento
1. El player debe tener un componente `PlayerProxy`
2. Cuando el player muera, debe aparecer el mensaje "PLAYER MURIÓ"
3. Después del delay configurado, la escena debe reiniciarse

## Notas Importantes
- El sistema se conecta automáticamente al `PlayerProxy` existente
- El reinicio preserva todos los objetos y configuraciones de la escena
- El delay permite que el jugador vea el resultado antes del reinicio
