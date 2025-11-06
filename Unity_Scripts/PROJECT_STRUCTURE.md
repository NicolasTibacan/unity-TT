# 📦 Estructura Completa del Proyecto Unity

## 📂 Jerarquía de Carpetas

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── PhysicsSimulation.cs
│   │   └── AnalyticsCalculator.cs
│   │
│   ├── Player/
│   │   └── PlayerController.cs
│   │
│   ├── Obstacles/
│   │   └── ObstacleManager.cs
│   │
│   ├── UI/
│   │   ├── UIManager.cs
│   │   └── ChartRenderer.cs
│   │
│   ├── Camera/
│   │   └── CameraController.cs
│   │
│   ├── Effects/
│   │   └── VisualEffectsManager.cs
│   │
│   ├── ScriptableObjects/
│   │   ├── WorldConfigSO.cs
│   │   └── BallConfigSO.cs
│   │
│   └── Utilities/
│       └── FreeFallUtilities.cs
│
├── Prefabs/
│   ├── Platform.prefab
│   ├── Block.prefab
│   ├── Player.prefab (opcional)
│   └── Obstacles/
│
├── Materials/
│   ├── Player/
│   │   ├── BallLight.mat
│   │   ├── BallMedium.mat
│   │   └── BallHeavy.mat
│   │
│   ├── Obstacles/
│   │   ├── Platform.mat
│   │   └── Block.mat
│   │
│   └── Environment/
│       ├── Ground.mat
│       └── Background.mat
│
├── Textures/
│   └── UI/
│
├── Audio/
│   ├── SFX/
│   │   ├── Impact.wav
│   │   ├── Platform.wav
│   │   └── Block.wav
│   │
│   └── Music/
│       └── Background.mp3
│
├── Scenes/
│   ├── MainScene.unity
│   └── TestScene.unity
│
└── Resources/
    └── Configs/
        ├── WorldConfigs/
        │   ├── IdealWorld.asset
        │   ├── LightWind.asset
        │   ├── HighResistance.asset
        │   ├── LowGravity.asset
        │   └── HighGravity.asset
        │
        └── BallConfigs/
            ├── LightBall.asset
            ├── MediumBall.asset
            └── HeavyBall.asset
```

## 🎯 Descripción de Scripts

### 📊 Core (Núcleo del Sistema)

#### **GameManager.cs**
- **Propósito:** Gestión principal del flujo del juego
- **Responsabilidades:**
  - Control de estados (Ready, Running, Finished)
  - Coordinación entre componentes
  - Gestión de configuraciones de mundos y pelotas
  - Sistema de puntuación
  - Loop principal del juego
- **Dependencias:** Todos los demás sistemas
- **Singleton:** No (pero único en la escena)

#### **PhysicsSimulation.cs**
- **Propósito:** Motor de simulación física
- **Modelo:** `a = g - (k/m) * v`
- **Responsabilidades:**
  - Cálculo de aceleración, velocidad y posición
  - Integración numérica (Euler)
  - Registro de historial de datos
  - Actualización en vivo de parámetros
- **Dependencias:** WorldConfig, BallConfig

#### **AnalyticsCalculator.cs**
- **Propósito:** Soluciones analíticas y comparación
- **Método:** Transformada de Laplace
- **Responsabilidades:**
  - Calcular trayectoria teórica
  - Encontrar tiempo de caída analítico
  - Calcular velocidad terminal
  - Análisis de energía
- **Dependencias:** Ninguna (puro matemático)

### 🎮 Player

#### **PlayerController.cs**
- **Propósito:** Control del jugador
- **Input:** Horizontal Axis (← → o A D)
- **Responsabilidades:**
  - Movimiento horizontal
  - Aplicación de límites
  - Actualización de posición vertical desde física
  - Gestión de visual de pelota
  - Sistema de "push" por colisiones
- **Dependencias:** PhysicsSimulation

### 🚧 Obstacles

#### **ObstacleManager.cs**
- **Propósito:** Generación y gestión de obstáculos
- **Responsabilidades:**
  - Generación procedural de obstáculos
  - Instanciación desde prefabs
  - Sistema de colisiones
  - Limpieza de obstáculos antiguos
- **Tipos de Obstáculos:**
  - **Platform:** Termina el juego (objetivo)
  - **Block:** Reduce velocidad y empuja

### 📺 UI

#### **UIManager.cs**
- **Propósito:** Gestión de interfaz de usuario
- **Framework:** TextMeshPro
- **Responsabilidades:**
  - Actualización de textos en tiempo real
  - Gestión de controles (botones, dropdowns, inputs)
  - Coordinación con ChartRenderer
  - Paneles de teoría y análisis
- **Dependencias:** GameManager, TextMeshPro

#### **ChartRenderer.cs**
- **Propósito:** Renderizado de gráficos 2D
- **Método:** Textura procedural + Bresenham
- **Responsabilidades:**
  - Dibujar gráficos de líneas
  - Gestionar series de datos (sim + analítica)
  - Auto-escalado de ejes
  - Grid y etiquetas
- **Alternativa a:** Chart.js del original

### 📷 Camera

#### **CameraController.cs**
- **Propósito:** Control de cámara suave
- **Tipo:** Ortográfica con seguimiento
- **Responsabilidades:**
  - Seguir al jugador verticalmente
  - Suavizado con Lerp
  - Gestión de límites
  - Control de zoom
- **Modo:** LateUpdate para evitar jitter

### ✨ Effects

#### **VisualEffectsManager.cs**
- **Propósito:** Efectos visuales y feedback
- **Responsabilidades:**
  - Trail renderer dinámico
  - Sistema de partículas de velocidad
  - Indicador visual de velocidad (flecha)
  - Marcadores de altura
  - Efectos de impacto
  - Audio
- **Opcional:** Puede deshabilitarse para mejor performance

### 🗂️ ScriptableObjects

#### **WorldConfigSO.cs**
- **Propósito:** Configuración reutilizable de mundos
- **Ventajas:** 
  - Crear assets en Project
  - Modificar sin código
  - Compartir entre escenas
- **Cómo crear:** `Right Click → Create → FreeFall → World Config`

#### **BallConfigSO.cs**
- **Propósito:** Configuración reutilizable de pelotas
- **Incluye:** Física, visual, efectos, audio
- **Cómo crear:** `Right Click → Create → FreeFall → Ball Config`

### 🛠️ Utilities

#### **FreeFallUtilities.cs**
- **Propósito:** Funciones auxiliares estáticas
- **Incluye:**
  - Conversiones de unidades
  - Cálculos físicos rápidos
  - Formateo de valores
  - Helpers de color
  - Debug visual
  - Extensiones de Vector3
- **Uso:** `FreeFallUtilities.FormatTime(time)`

## 🔗 Diagrama de Dependencias

```
GameManager
    ├─→ PhysicsSimulation
    ├─→ PlayerController
    │       └─→ PhysicsSimulation
    ├─→ ObstacleManager
    ├─→ UIManager
    │       ├─→ ChartRenderer
    │       └─→ TextMeshPro
    └─→ AnalyticsCalculator

CameraController
    └─→ PlayerController (Transform)

VisualEffectsManager
    ├─→ PlayerController
    └─→ PhysicsSimulation
```

## 🔄 Flujo de Ejecución

### Inicialización (Start):
```
1. GameManager.Start()
   ├─ InitializeGame()
   ├─ PhysicsSimulation.Initialize()
   ├─ ObstacleManager.GenerateObstacles()
   └─ UIManager.SetupUI()

2. PlayerController.Start()
   └─ SetupVisuals()

3. CameraController.Start()
   └─ FindTarget()
```

### Loop de Juego (Update):
```
1. GameManager.Update()
   └─ if (Running):
       ├─ PhysicsSimulation.UpdatePhysics(dt)
       ├─ PlayerController.UpdateMovement(dt)
       ├─ CheckCollisions()
       ├─ UpdateCharts()
       └─ UpdateUI()

2. VisualEffectsManager.Update()
   ├─ UpdateTrail()
   ├─ UpdateVelocityParticles()
   └─ UpdateVelocityArrow()

3. CameraController.LateUpdate()
   └─ SmoothFollow()
```

## 📊 Flujo de Datos

### Física → Visual:
```
PhysicsSimulation (height, velocity)
    ├─→ PlayerController (transform.position.y)
    ├─→ UIManager (texto altura/velocidad)
    ├─→ ChartRenderer (puntos de datos)
    └─→ VisualEffectsManager (trail, partículas)
```

### Usuario → Sistema:
```
UIManager (Input Fields, Buttons)
    └─→ GameManager
        └─→ PhysicsSimulation (nuevos parámetros)
```

### Colisiones → Respuesta:
```
PlayerController (posición)
    └─→ GameManager.CheckCollisions()
        └─→ ObstacleManager.GetObstacles()
            └─→ Obstacle.CheckCollision()
                ├─→ GameManager.StopSimulation() [Platform]
                └─→ PhysicsSimulation (reducir velocidad) [Block]
```

## 🎨 Assets Necesarios

### Prefabs (2 obligatorios):
- ✅ Platform.prefab
- ✅ Block.prefab
- 🔲 Player.prefab (opcional)

### Materials (mínimo 4):
- ✅ BallMedium.mat (azul)
- ✅ Platform.mat (gris)
- ✅ Block.mat (rojo)
- ✅ Ground.mat (verde)

### Audio (opcional):
- 🔲 Impact.wav
- 🔲 Platform.wav
- 🔲 Block.wav

## 📋 Componentes Requeridos en Escena

### GameObject: GameManager
```
- Transform
- GameManager (script)
- PhysicsSimulation (script)
- ObstacleManager (script)
- AnalyticsCalculator (script)
- [VisualEffectsManager] (opcional)
```

### GameObject: Player
```
- Transform (0, 100, 0)
- Rigidbody (isKinematic=true)
- SphereCollider
- PlayerController (script)
  └─ Child: BallVisual (Sphere)
      - MeshRenderer
```

### GameObject: Main Camera
```
- Transform (0, 60, -20)
- Camera (Orthographic, size=15)
- CameraController (script)
- [AudioListener]
```

### GameObject: UICanvas
```
- Canvas
- CanvasScaler
- GraphicRaycaster
  └─ Child: UIManager
      - UIManager (script)
      └─ Children: UI Elements (TMP, Buttons, etc.)
```

## 🔧 Configuraciones Críticas

### Project Settings:
- **Input Manager:** Horizontal configurado
- **Quality:** Medium o superior
- **Player:** Color Space = Linear (URP)

### Scene Settings:
- **Lighting:** Ambient Color azul claro
- **Camera:** Clear Flags = Solid Color
- **Background:** Color celeste

### Physics Settings:
- **Gravity:** Y = 0 (usamos nuestra propia gravedad)
- **Default Material:** Sin fricción

## 🚀 Orden de Creación Recomendado

1. ✅ Importar todos los scripts
2. ✅ Crear GameManager con todos sus componentes
3. ✅ Crear Player con controller y visual
4. ✅ Crear cámara con controller
5. ✅ Crear prefabs de obstáculos
6. ✅ Asignar prefabs en ObstacleManager
7. ✅ Crear UI Canvas básico
8. ✅ Crear elementos UI esenciales
9. ✅ Crear UIManager y asignar referencias
10. ✅ Conectar todas las referencias en GameManager
11. ✅ Probar funcionalidad básica
12. ✅ Añadir efectos visuales (opcional)
13. ✅ Pulir UI y visual
14. ✅ Testing completo

## 📚 Documentación de Referencia

Cada script incluye:
- ✅ XML Documentation (///)
- ✅ [Tooltip] en campos públicos
- ✅ [Header] para organización
- ✅ Comentarios explicativos
- ✅ Región de código (opcional)

---

**Total Scripts:** 11  
**Líneas de Código:** ~3,000+  
**Complejidad:** Media  
**Tiempo Estimado Setup:** 30-60 min  
**Nivel:** Intermedio-Avanzado  
