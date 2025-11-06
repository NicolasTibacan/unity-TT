# 🚀 Guía Rápida de Implementación

## ⚡ Setup Rápido (15 minutos)

### 1. Crear Proyecto Unity
```
- Unity Hub 6.2
- Proyecto 3D (URP recomendado)
- Nombre: FreeFallPhysics
```

### 2. Importar Scripts
```
Copiar todos los .cs a Assets/Scripts/
```

### 3. Configuración Mínima de Escena

#### Jerarquía de GameObjects:
```
Escena
├─ GameManager (Empty)
│  └─ Scripts: GameManager, PhysicsSimulation, ObstacleManager, AnalyticsCalculator
│
├─ Player (Sphere)
│  ├─ PlayerController script
│  └─ BallVisual (Sphere hijo)
│
├─ Main Camera
│  └─ CameraController script
│
├─ Ground (Plane)
│
├─ UICanvas (Canvas)
│  ├─ UIManager (Empty hijo)
│  └─ [Elementos UI]
│
└─ VisualEffects (Empty - opcional)
   └─ VisualEffectsManager script
```

## 📋 Checklist de Configuración

### GameManager
- [x] Asignar PhysicsSimulation (mismo objeto)
- [x] Asignar PlayerController (Player)
- [x] Asignar ObstacleManager (mismo objeto)
- [x] Asignar UIManager (UICanvas/UIManager)
- [x] Asignar AnalyticsCalculator (mismo objeto)

### PhysicsSimulation
- [x] Initial Height: 100
- [x] Initial Velocity: 0
- [x] Live Update: ✓

### PlayerController
- [x] Move Speed: 10
- [x] Min X: -15
- [x] Max X: 15
- [x] Ball Visual: (BallVisual GameObject)
- [x] Physics Simulation: (GameManager)

### CameraController
- [x] Target: Player
- [x] Smooth Speed: 0.125
- [x] Vertical Offset: 10
- [x] Camera Distance: -20
- [x] Orthographic Size: 15

### ObstacleManager
- [x] Platform Prefab: (crear prefab)
- [x] Block Prefab: (crear prefab)
- [x] Min X: -12
- [x] Max X: 12
- [x] Obstacle Count: 8
- [x] Platform Probability: 0.7

## 🎨 Prefabs Necesarios

### Platform Prefab
```
GameObject: Cube
Escala: (3, 0.2, 1)
Color: RGB(68, 68, 68)
BoxCollider: ✓
```

### Block Prefab
```
GameObject: Cube
Escala: (3, 1, 1)
Color: RGB(139, 0, 0)
BoxCollider: ✓
```

## 🖼️ UI Elementos Básicos

### Texto de Estado (TextMeshPro):
- Height
- Velocity
- Time
- Score
- Message

### Controles:
- Dropdown World
- Dropdown Ball
- Button Start
- Button Reset
- Button Apply

### Inputs (TMP Input Field):
- Gravity (g)
- Drag (k)
- Mass (m)
- Initial Height (h0)
- Initial Velocity (v0)

### Toggles:
- Show Theory
- Show Laplace
- Live Update

## ⌨️ Input Configuration

Verificar en Edit → Project Settings → Input Manager:

```
Horizontal Axis:
- Negative: left, a
- Positive: right, d
- Alt Negative: arrow left
- Alt Positive: arrow right
- Gravity: 3
- Sensitivity: 3
- Type: Key or Mouse Button
```

## 🎮 Orden de Ejecución

Unity ejecuta los scripts en este orden:
1. Awake()
2. Start()
3. Update() / FixedUpdate()
4. LateUpdate()

Asegurar que:
- GameManager inicializa primero
- PlayerController lee de PhysicsSimulation en Update()
- CameraController actualiza en LateUpdate()

## 🔧 Configuración de Físicas por Defecto

```csharp
// En GameManager.worlds[]
worlds[0]: Plano ideal      (g=9.81, k=0.0)
worlds[1]: Viento leve      (g=9.81, k=0.5)
worlds[2]: Resistencia alta (g=9.81, k=2.0)
worlds[3]: Gravedad baja    (g=6.0,  k=0.6)
worlds[4]: Gravedad alta    (g=15.0, k=0.3)

// En GameManager.balls[]
balls[0]: Ligera (m=0.2, r=0.4)
balls[1]: Media  (m=1.0, r=0.6)  [DEFAULT]
balls[2]: Pesada (m=5.0, r=0.8)
```

## 📊 Testing Checklist

### Funcionalidad Básica:
- [ ] El juego inicia sin errores
- [ ] La pelota cae al presionar "Iniciar"
- [ ] El jugador se mueve con ← →
- [ ] La altura disminuye con el tiempo
- [ ] La velocidad aumenta con el tiempo

### Física:
- [ ] Sin arrastre: caída libre pura
- [ ] Con arrastre: velocidad se estabiliza
- [ ] Diferentes masas afectan la caída
- [ ] La gravedad mayor = caída más rápida

### Obstáculos:
- [ ] Se generan 8 obstáculos
- [ ] Plataformas detienen el juego
- [ ] Bloques reducen velocidad
- [ ] Colisiones se detectan correctamente

### UI:
- [ ] Textos actualizan en tiempo real
- [ ] Dropdowns funcionan
- [ ] Botones responden
- [ ] Inputs aceptan valores

### Análisis:
- [ ] Tiempo de caída simulado correcto
- [ ] Tiempo de caída analítico calculado
- [ ] Velocidad terminal mostrada
- [ ] Gráficos se dibujan (si implementados)

## 🐛 Errores Comunes

### "NullReferenceException"
**Causa:** Referencias no asignadas
**Solución:** Verificar todas las asignaciones en Inspector

### "Physics not updating"
**Causa:** GameState no es "Running"
**Solución:** Presionar botón Start

### "Player not moving"
**Causa:** Input no configurado
**Solución:** Verificar Input Manager

### "UI not visible"
**Causa:** Canvas no configurado
**Solución:** Canvas → Render Mode: Screen Space - Overlay

### "TMP not found"
**Causa:** TextMeshPro no importado
**Solución:** Window → TextMeshPro → Import TMP Essential

## 📈 Valores de Prueba Interesantes

### Caída Libre Pura:
```
g = 9.81
k = 0
m = cualquiera
h0 = 100
v0 = 0
Resultado: t ≈ 4.52s, v ≈ 44.3 m/s
```

### Con Resistencia Normal:
```
g = 9.81
k = 0.5
m = 1.0
h0 = 100
v0 = 0
Resultado: Terminal velocity ≈ 19.62 m/s
```

### Gravedad Lunar:
```
g = 1.62
k = 0.1
m = 1.0
h0 = 100
v0 = 0
Resultado: Caída muy lenta
```

## 🚀 Optimizaciones Opcionales

### Performance:
- Usar Object Pooling para obstáculos
- Limitar frecuencia de actualización de gráficos
- LOD para objetos visuales distantes

### Features Adicionales:
- Sistema de puntuación complejo
- Leaderboard local
- Múltiples niveles
- Power-ups
- Efectos de sonido
- Música de fondo

## 📚 Recursos Adicionales

### Documentación Unity:
- [Input System](https://docs.unity3d.com/Manual/Input.html)
- [TextMeshPro](https://docs.unity3d.com/Manual/com.unity.textmeshpro.html)
- [ScriptableObjects](https://docs.unity3d.com/Manual/class-ScriptableObject.html)

### Física:
- [Caída Libre](https://es.wikipedia.org/wiki/Ca%C3%ADda_libre)
- [Resistencia Aerodinámica](https://es.wikipedia.org/wiki/Resistencia_aerodin%C3%A1mica)
- [Transformada de Laplace](https://es.wikipedia.org/wiki/Transformada_de_Laplace)

## 💡 Tips Pro

1. **Usar ScriptableObjects** para configuraciones reutilizables
2. **Debug.DrawRay** para visualizar vectores de física
3. **Gizmos** para debugging visual en Scene view
4. **[SerializeField] private** para exponer variables sin hacerlas públicas
5. **Profiler** (Window → Analysis → Profiler) para optimización

## 🎓 Extensiones Educativas

### Para Profesores:
- Exportar datos a CSV para análisis en Excel
- Comparar resultados con ecuaciones teóricas
- Graficar diferentes escenarios
- Calcular errores porcentuales

### Para Estudiantes:
- Experimentar con diferentes valores
- Predecir resultados antes de simular
- Documentar observaciones
- Crear hipótesis y validarlas

---

**¿Listo para empezar?** 🎮
1. Importa los scripts
2. Configura la escena básica
3. Presiona Play
4. ¡A experimentar con física!
