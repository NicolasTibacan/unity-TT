# 🎮 Juego de Caída Libre - Scripts para Unity 6.2

## 📋 Descripción
Este proyecto contiene los scripts de C# convertidos desde JavaScript para crear un juego educativo de física sobre caída libre con resistencia del aire en Unity 6.2.

## 📁 Estructura de Scripts

### Scripts Principales

1. **GameManager.cs** - Gestión principal del juego
   - Control del estado del juego
   - Coordinación entre componentes
   - Configuración de mundos y pelotas
   - Sistema de puntuación

2. **PhysicsSimulation.cs** - Simulación de física
   - Implementa el modelo: `a = g - (k/m) * v`
   - Actualización de altura y velocidad
   - Registro de historial de datos

3. **PlayerController.cs** - Control del jugador
   - Movimiento horizontal con teclado (← →)
   - Límites del área de juego
   - Visual de la pelota

4. **ObstacleManager.cs** - Gestión de obstáculos
   - Generación procedural de obstáculos
   - Tipos: Plataformas (seguras) y Bloques (peligrosos)
   - Sistema de colisiones

5. **AnalyticsCalculator.cs** - Cálculos analíticos
   - Solución mediante transformada de Laplace
   - Cálculo de velocidad terminal
   - Tiempo de caída teórico

6. **UIManager.cs** - Interfaz de usuario
   - Actualización de textos en tiempo real
   - Gráficos de altura y velocidad
   - Controles y configuración

7. **ChartRenderer.cs** - Renderizador de gráficos
   - Alternativa a Chart.js para Unity
   - Gráficos 2D en tiempo real
   - Series de datos múltiples

8. **CameraController.cs** - Control de cámara
   - Seguimiento suave del jugador
   - Cámara ortográfica
   - Zoom ajustable

## 🚀 Instrucciones de Configuración en Unity

### Paso 1: Crear el Proyecto
1. Abrir Unity Hub 6.2
2. Crear nuevo proyecto 3D (URP o Built-in Render Pipeline)
3. Nombrar el proyecto: "FreeFallPhysics"

### Paso 2: Importar Scripts
1. Crear carpeta `Assets/Scripts`
2. Copiar todos los archivos .cs a esta carpeta
3. Esperar a que Unity compile

### Paso 3: Instalar TextMeshPro
1. Window → TextMeshPro → Import TMP Essential Resources
2. Confirmar la importación

### Paso 4: Configurar la Escena

#### A. Crear GameManager
1. GameObject → Create Empty
2. Renombrar a "GameManager"
3. Agregar script `GameManager.cs`
4. Agregar scripts `PhysicsSimulation.cs`, `AnalyticsCalculator.cs`

#### B. Crear Jugador
1. GameObject → 3D Object → Sphere
2. Renombrar a "Player"
3. Agregar script `PlayerController.cs`
4. Escalar: (0.6, 0.6, 0.6)
5. Posición: (0, 100, 0)
6. En el Rigidbody: marcar "Is Kinematic"

#### C. Crear Visual de la Pelota
1. Crear GameObject → 3D Object → Sphere como hijo de Player
2. Renombrar a "BallVisual"
3. Asignar en PlayerController → Ball Visual

#### D. Crear Cámara
1. Seleccionar Main Camera
2. Agregar script `CameraController.cs`
3. Posición: (0, 60, -20)
4. Rotation: (0, 0, 0)
5. Asignar Player en el campo Target

#### E. Crear Suelo
1. GameObject → 3D Object → Plane
2. Renombrar a "Ground"
3. Posición: (0, -0.5, 0)
4. Escalar: (5, 1, 5)
5. Agregar material verde

#### F. Crear ObstacleManager
1. En GameManager, agregar componente `ObstacleManager.cs`
2. Crear prefabs para obstáculos:

**Prefab Plataforma:**
- GameObject → 3D Object → Cube
- Nombre: "Platform"
- Escalar: (3, 0.2, 1)
- Material: Gris oscuro (RGB: 68, 68, 68)
- Guardar en Assets/Prefabs/

**Prefab Bloque:**
- GameObject → 3D Object → Cube
- Nombre: "Block"
- Escalar: (3, 1, 1)
- Material: Rojo oscuro (RGB: 139, 0, 0)
- Guardar en Assets/Prefabs/

#### G. Crear UI Canvas
1. GameObject → UI → Canvas
2. Canvas Scaler → Scale With Screen Size
3. Reference Resolution: 1920x1080

**Crear elementos de UI:**

**Panel Izquierdo (Info):**
```
Canvas
└─ PanelInfo (Panel)
   ├─ TextHeight (TextMeshPro)
   ├─ TextVelocity (TextMeshPro)
   ├─ TextTime (TextMeshPro)
   ├─ TextScore (TextMeshPro)
   └─ TextMessage (TextMeshPro)
```

**Panel Superior (Controles):**
```
Canvas
└─ PanelControls (Panel)
   ├─ DropdownWorld (TMP Dropdown)
   ├─ DropdownBall (TMP Dropdown)
   ├─ ButtonStart (Button)
   ├─ ButtonReset (Button)
   ├─ InputG (TMP Input Field)
   ├─ InputK (TMP Input Field)
   ├─ InputM (TMP Input Field)
   ├─ InputH0 (TMP Input Field)
   ├─ InputV0 (TMP Input Field)
   ├─ ToggleTheory (Toggle)
   ├─ ToggleLaplace (Toggle)
   ├─ ToggleLiveUpdate (Toggle)
   └─ ButtonApply (Button)
```

**Panel Derecho (Análisis):**
```
Canvas
└─ PanelAnalysis (Panel)
   ├─ TextFallTimeSim (TextMeshPro)
   ├─ TextFallTimeAna (TextMeshPro)
   ├─ TextImpactVelSim (TextMeshPro)
   ├─ TextImpactVelAna (TextMeshPro)
   └─ TextTerminalVel (TextMeshPro)
```

**Panel Teoría:**
```
Canvas
└─ PanelTheory (Panel) [Inicialmente desactivado]
   └─ TextTheory (TextMeshPro)
```

**Gráficos:**
```
Canvas
└─ PanelCharts (Panel)
   ├─ ImageChartHeight (Raw Image)
   │  └─ Agregar ChartRenderer.cs
   └─ ImageChartVelocity (Raw Image)
      └─ Agregar ChartRenderer.cs
```

### Paso 5: Conectar Referencias en GameManager
En el Inspector del GameManager, asignar:
- Physics Simulation → (mismo objeto)
- Player Controller → Player
- Obstacle Manager → (mismo objeto)
- UI Manager → (crear GameObject vacío con UIManager.cs)
- Analytics Calculator → (mismo objeto)

### Paso 6: Configurar UIManager
En el Inspector del UIManager, asignar todos los textos, botones, dropdowns y paneles creados.

### Paso 7: Configurar ObstacleManager
- Platform Prefab → Prefab de plataforma
- Block Prefab → Prefab de bloque
- Min X: -12
- Max X: 12

## 🎯 Configuración de Input
Unity 6.2 usa el nuevo Input System. Configurar:

1. Edit → Project Settings → Input Manager
2. Asegurar que "Horizontal" esté configurado:
   - Negative Button: left, a
   - Positive Button: right, d
   - Alt Negative: ← (arrow left)
   - Alt Positive: → (arrow right)

## 🎨 Materiales Recomendados

### Jugador
- Color: Azul claro (RGB: 102, 163, 255)
- Shader: Standard

### Obstáculos
- Plataformas: Gris oscuro (RGB: 68, 68, 68)
- Bloques: Rojo oscuro (RGB: 139, 0, 0)

### Fondo
- Skybox: Color celeste degradado
- Lighting: Ambient Source = Color

## ⚙️ Configuración de Física

Los valores por defecto son:
- **g (gravedad)**: 9.81 m/s²
- **k (arrastre)**: 0.5 kg/s
- **m (masa)**: 1.0 kg
- **h0 (altura inicial)**: 100 m
- **v0 (velocidad inicial)**: 0 m/s

## 🎮 Controles del Juego

- **← →** (Flechas) o **A D**: Mover horizontalmente
- **Botón Iniciar**: Comenzar simulación
- **Botón Reiniciar**: Reiniciar a condiciones iniciales
- **Botón Aplicar**: Aplicar configuración personalizada

## 📊 Características Implementadas

✅ Simulación de física con resistencia del aire  
✅ Solución analítica mediante Laplace  
✅ Comparación simulación vs teoría  
✅ Múltiples mundos con diferentes físicas  
✅ Diferentes pelotas (masas)  
✅ Obstáculos procedurales  
✅ Sistema de colisiones  
✅ Gráficos en tiempo real  
✅ Análisis estadístico  
✅ Cálculo de velocidad terminal  
✅ Actualización en vivo de parámetros  

## 🐛 Troubleshooting

### Error: "TextMeshPro namespace not found"
- Solución: Importar TMP Essential Resources (Window → TextMeshPro)

### La pelota no se mueve
- Verificar que PhysicsSimulation esté en el GameManager
- Verificar que el GameState sea "Running"

### Los obstáculos no aparecen
- Verificar que los prefabs estén asignados en ObstacleManager
- Verificar que la función GenerateObstacles() se llame

### UI no actualiza
- Verificar que todas las referencias estén asignadas en UIManager
- Verificar que los scripts usen "using TMPro;"

## 📝 Notas Adicionales

- **Performance**: El sistema de gráficos es básico. Para mejor rendimiento, considera usar una librería de gráficos externa.
- **Extensibilidad**: Puedes añadir más mundos editando el array `worlds` en GameManager.
- **Física**: El modelo usa integración de Euler. Para mayor precisión, considera RK4.

## 🔗 Comparación con el Original

| Característica | JavaScript/Canvas | Unity C# |
|---------------|-------------------|----------|
| Renderizado | Canvas 2D | 3D (proyección ortográfica) |
| Física | Manual | Rigidbody (cinemático) |
| UI | HTML/CSS | TextMeshPro/UI Toolkit |
| Gráficos | Chart.js | ChartRenderer custom |
| Input | addEventListener | Input.GetAxisRaw |

## 👨‍💻 Autor
Convertido de JavaScript/HTML5 a C#/Unity 6.2

## 📄 Licencia
Proyecto educativo - Uso libre para aprendizaje
