# 📘 Ejemplos de Uso y Casos Prácticos

## 🎯 Casos de Uso Educativos

### Ejemplo 1: Caída Libre Ideal (Sin Resistencia)

**Objetivo:** Verificar las ecuaciones de cinemática básica

**Configuración:**
```csharp
// En Unity Inspector o mediante código:
g = 9.81 m/s²
k = 0.0 kg/s     // Sin resistencia
m = 1.0 kg
h0 = 100 m
v0 = 0 m/s
```

**Predicción Teórica:**
```
t_caída = √(2h/g) = √(200/9.81) ≈ 4.52 s
v_impacto = √(2gh) = √(1962) ≈ 44.29 m/s
```

**Resultado Esperado:**
- La simulación debe coincidir exactamente con la teoría
- El gráfico de velocidad debe ser una línea recta
- El gráfico de altura debe ser parabólico

**Preguntas para Estudiantes:**
1. ¿Por qué la velocidad aumenta linealmente?
2. ¿Cómo cambia la energía potencial a cinética?
3. ¿Qué representa la pendiente del gráfico v(t)?

---

### Ejemplo 2: Efecto de la Masa (Con Resistencia)

**Objetivo:** Demostrar que la masa afecta la caída con resistencia

**Experimento:**
```csharp
// Configuración base:
g = 9.81 m/s²
k = 0.5 kg/s
h0 = 100 m
v0 = 0 m/s

// Variar masa:
Pelota 1: m = 0.2 kg → v_terminal = 3.92 m/s
Pelota 2: m = 1.0 kg → v_terminal = 19.62 m/s
Pelota 3: m = 5.0 kg → v_terminal = 98.1 m/s
```

**Observación:**
- Pelota ligera: Alcanza v_terminal rápido, cae lentamente
- Pelota pesada: Tarda en alcanzar v_terminal, cae más rápido

**Actividad:**
1. Registrar tiempo de caída para cada pelota
2. Graficar v(t) para las tres pelotas
3. Identificar cuándo alcanzan v_terminal
4. Explicar por qué la masa importa con resistencia

---

### Ejemplo 3: Comparación de Planetas

**Objetivo:** Simular caída en diferentes planetas

**Configuraciones:**

**Tierra:**
```csharp
g = 9.81 m/s²
k = 0.5 kg/s
m = 1.0 kg
```

**Luna:**
```csharp
g = 1.62 m/s²
k = 0.1 kg/s  // Atmósfera tenue
m = 1.0 kg
```

**Marte:**
```csharp
g = 3.71 m/s²
k = 0.15 kg/s  // Atmósfera delgada
m = 1.0 kg
```

**Júpiter:**
```csharp
g = 24.79 m/s²
k = 2.0 kg/s  // Atmósfera densa
m = 1.0 kg
```

**Comparación:**
| Planeta | g (m/s²) | t_caída (aprox) | v_terminal |
|---------|----------|-----------------|------------|
| Luna    | 1.62     | ~11 s           | 16.2 m/s   |
| Marte   | 3.71     | ~7 s            | 24.7 m/s   |
| Tierra  | 9.81     | ~5 s            | 19.6 m/s   |
| Júpiter | 24.79    | ~3 s            | 12.4 m/s   |

---

### Ejemplo 4: Velocidad Terminal

**Objetivo:** Observar el concepto de velocidad terminal

**Configuración:**
```csharp
g = 9.81 m/s²
k = 2.0 kg/s    // Resistencia alta
m = 1.0 kg
h0 = 200 m      // Altura mayor
v0 = 0 m/s
```

**Cálculo Teórico:**
```
v_terminal = mg/k = (1.0 × 9.81) / 2.0 = 4.905 m/s
```

**Observación:**
- La velocidad aumenta exponencialmente al inicio
- Se estabiliza alrededor de 4.9 m/s
- La altura disminuye linealmente después de alcanzar v_terminal

**Ejercicio:**
1. Identificar en el gráfico cuándo se alcanza v_terminal
2. Medir el tiempo para alcanzar 95% de v_terminal
3. Calcular la distancia recorrida hasta ese punto

---

### Ejemplo 5: Efecto del Arrastre

**Objetivo:** Comparar diferentes coeficientes de arrastre

**Configuraciones:**

**Aire normal:**
```csharp
k = 0.5 kg/s
```

**Aire denso (neblina):**
```csharp
k = 1.5 kg/s
```

**Líquido (agua):**
```csharp
k = 5.0 kg/s
```

**Resultados Esperados:**
- Mayor k → Menor v_terminal
- Mayor k → Mayor tiempo de caída
- Mayor k → Curva v(t) se aplana más rápido

---

## 💻 Ejemplos de Código

### Ejemplo 1: Crear Mundo Personalizado via Script

```csharp
using UnityEngine;

public class CustomWorldCreator : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        // Crear configuración de mundo "Lluvia"
        WorldConfig rainWorld = new WorldConfig
        {
            name = "Día Lluvioso",
            gravity = 9.81f,
            dragCoefficient = 1.2f, // Mayor resistencia por gotas
            backgroundColor = new Color(0.7f, 0.7f, 0.8f)
        };

        // Aplicar
        gameManager.ApplyCustomConfiguration(
            rainWorld.gravity,
            rainWorld.dragCoefficient,
            1.0f,  // masa
            100f,  // altura
            0f     // velocidad inicial
        );
    }
}
```

### Ejemplo 2: Registrar Datos para Análisis

```csharp
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour
{
    public PhysicsSimulation physics;
    public GameManager gameManager;

    private List<string> dataLog = new List<string>();

    void Start()
    {
        dataLog.Add("Tiempo,Altura,Velocidad,Energía_Cinética,Energía_Potencial");
    }

    void Update()
    {
        if (gameManager.currentState == GameState.Running)
        {
            float t = Time.time;
            float h = physics.height;
            float v = physics.velocity;
            float ke = physics.CalculateKineticEnergy();
            float pe = physics.CalculatePotentialEnergy();

            string line = $"{t:F3},{h:F3},{v:F3},{ke:F3},{pe:F3}";
            dataLog.Add(line);
        }
    }

    public void SaveToCSV()
    {
        string path = Application.persistentDataPath + "/simulation_data.csv";
        File.WriteAllLines(path, dataLog);
        Debug.Log($"Datos guardados en: {path}");
    }

    void OnApplicationQuit()
    {
        SaveToCSV();
    }
}
```

### Ejemplo 3: Comparador Automático Simulación vs Teoría

```csharp
using UnityEngine;

public class AccuracyChecker : MonoBehaviour
{
    public PhysicsSimulation physics;
    public AnalyticsCalculator analytics;
    
    [Header("Parámetros")]
    public float checkInterval = 0.5f;
    public float errorThreshold = 5f; // 5% de error aceptable

    private float lastCheckTime = 0f;

    void Update()
    {
        if (Time.time - lastCheckTime > checkInterval)
        {
            lastCheckTime = Time.time;
            CheckAccuracy();
        }
    }

    void CheckAccuracy()
    {
        // Obtener valores simulados
        float simHeight = physics.height;
        float simVelocity = physics.velocity;

        // Calcular valores teóricos
        var analytic = analytics.CalculateAtTime(
            Time.time,
            physics.initialHeight,
            physics.initialVelocity,
            9.81f,
            0.5f,
            1.0f
        );

        // Calcular errores
        float heightError = FreeFallUtilities.CalculatePercentError(simHeight, analytic.height);
        float velocityError = FreeFallUtilities.CalculatePercentError(simVelocity, analytic.velocity);

        // Reportar si excede umbral
        if (heightError > errorThreshold)
        {
            Debug.LogWarning($"Error altura: {heightError:F2}% (Sim: {simHeight:F2}, Teórica: {analytic.height:F2})");
        }

        if (velocityError > errorThreshold)
        {
            Debug.LogWarning($"Error velocidad: {velocityError:F2}% (Sim: {simVelocity:F2}, Teórica: {analytic.velocity:F2})");
        }
    }
}
```

### Ejemplo 4: Generador de Obstáculos Temáticos

```csharp
using UnityEngine;

public class ThemedObstacleGenerator : MonoBehaviour
{
    public ObstacleManager obstacleManager;

    public enum Theme
    {
        Forest,    // Plataformas como ramas
        City,      // Edificios
        Space,     // Asteroides
        Ocean      // Icebergs
    }

    public void GenerateThemedObstacles(Theme theme)
    {
        // Limpiar obstáculos actuales
        obstacleManager.ClearObstacles();

        // Configurar según tema
        switch (theme)
        {
            case Theme.Forest:
                GenerateForestObstacles();
                break;
            case Theme.City:
                GenerateCityObstacles();
                break;
            case Theme.Space:
                GenerateSpaceObstacles();
                break;
            case Theme.Ocean:
                GenerateOceanObstacles();
                break;
        }
    }

    void GenerateForestObstacles()
    {
        // Más plataformas (ramas), menos bloques
        obstacleManager.platformProbability = 0.9f;
        obstacleManager.GenerateObstacles();
    }

    void GenerateCityObstacles()
    {
        // Mix equilibrado
        obstacleManager.platformProbability = 0.6f;
        obstacleManager.GenerateObstacles();
    }

    void GenerateSpaceObstacles()
    {
        // Más bloques (asteroides peligrosos)
        obstacleManager.platformProbability = 0.3f;
        obstacleManager.GenerateObstacles();
    }

    void GenerateOceanObstacles()
    {
        // Plataformas flotantes
        obstacleManager.platformProbability = 0.8f;
        obstacleManager.obstacleCount = 12; // Más obstáculos
        obstacleManager.GenerateObstacles();
    }
}
```

### Ejemplo 5: Sistema de Medallas/Logros

```csharp
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    public GameManager gameManager;

    public enum Medal
    {
        None,
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public Medal EvaluatePerformance(float fallTime, float targetTime)
    {
        float error = Mathf.Abs(fallTime - targetTime);
        float errorPercent = (error / targetTime) * 100f;

        if (errorPercent < 1f)
            return Medal.Platinum;  // <1% error
        else if (errorPercent < 5f)
            return Medal.Gold;      // <5% error
        else if (errorPercent < 10f)
            return Medal.Silver;    // <10% error
        else if (errorPercent < 20f)
            return Medal.Bronze;    // <20% error
        else
            return Medal.None;
    }

    void OnSimulationEnd()
    {
        // Calcular tiempo teórico
        float theoreticalTime = gameManager.analyticsCalculator.FindFallTime(
            100f, 0f, 9.81f, 0.5f, 1.0f
        );

        // Obtener tiempo simulado
        float simulatedTime = gameManager.finalScore ?? 0f;

        // Evaluar
        Medal medal = EvaluatePerformance(simulatedTime, theoreticalTime);

        // Mostrar resultado
        Debug.Log($"¡Medalla: {medal}! Tiempo: {simulatedTime:F2}s (Objetivo: {theoreticalTime:F2}s)");
    }
}
```

---

## 🧪 Experimentos Propuestos

### Experimento 1: "La Pluma y el Martillo"
**Recrear el experimento lunar de Apollo 15**

```csharp
// Luna (sin atmósfera)
g = 1.62f
k = 0.0f  // Sin resistencia

// Pluma
m_pluma = 0.001f

// Martillo
m_martillo = 0.5f

// Resultado: Ambos caen al mismo tiempo
```

### Experimento 2: "Carrera de Caída"
**¿Qué pelota llega primero con resistencia?**

```csharp
// Tierra con aire
g = 9.81f
k = 0.5f

// Pelota de ping-pong
m1 = 0.0027f
r1 = 0.02f

// Pelota de bowling
m2 = 7.0f
r2 = 0.11f

// Pregunta: ¿Cuál llega primero?
```

### Experimento 3: "Optimización de Paracaídas"
**Encontrar el k óptimo para aterrizar a 5 m/s**

```csharp
// Dado:
m = 70f  // persona
h0 = 1000f
v_objetivo = 5f  // velocidad segura

// Encontrar: k tal que v_terminal = 5 m/s
// k = mg / v_t = (70 × 9.81) / 5 = 137.34 kg/s
```

---

## 📊 Plantillas de Reportes

### Reporte de Laboratorio Virtual

```markdown
# Reporte de Experimento: Caída Libre con Resistencia

## 1. Objetivo
[Describir el objetivo del experimento]

## 2. Marco Teórico
- Ecuación de movimiento: a = g - (k/m)v
- Velocidad terminal: v_t = mg/k
- [Otras ecuaciones relevantes]

## 3. Hipótesis
[Predicción antes de la simulación]

## 4. Metodología
**Parámetros utilizados:**
- g = ___ m/s²
- k = ___ kg/s
- m = ___ kg
- h0 = ___ m
- v0 = ___ m/s

## 5. Resultados
**Datos recopilados:**
| Tiempo (s) | Altura (m) | Velocidad (m/s) |
|------------|------------|-----------------|
| 0.0        | 100.0      | 0.0             |
| ...        | ...        | ...             |

**Gráficos:**
[Adjuntar capturas de pantalla]

## 6. Análisis
**Comparación Simulación vs Teoría:**
- Tiempo de caída (sim): ___ s
- Tiempo de caída (teórico): ___ s
- Error: ____%

## 7. Conclusiones
[Interpretación de resultados]

## 8. Preguntas
1. ¿Los resultados validan la hipótesis?
2. ¿Cuáles son las fuentes de error?
3. ¿Cómo mejorar el experimento?
```

---

## 🎓 Evaluaciones Sugeridas

### Quiz 1: Conceptos Básicos
1. ¿Qué es la velocidad terminal?
2. ¿Por qué la masa afecta la caída con resistencia?
3. ¿Qué representa el coeficiente k?

### Quiz 2: Cálculos
1. Calcular v_terminal para m=2kg, g=9.81m/s², k=1kg/s
2. Si v_terminal=20m/s y m=1kg, ¿cuál es k?
3. Estimar tiempo de caída desde 100m en caída libre

### Proyecto Final: Mini Investigación
**Opciones:**
1. Comparar caída en diferentes planetas
2. Diseñar un paracaídas virtual
3. Analizar la física de deportes extremos
4. Optimizar tiempo de caída con obstáculos

---

**¡Estos ejemplos están listos para usar en clase! 🎓**
