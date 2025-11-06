using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameManager principal para el juego de caída libre educativo
/// Compatible con Unity 6.2
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public PhysicsSimulation physicsSimulation;
    public PlayerController playerController;
    public ObstacleManager obstacleManager;
    public UIManager uiManager;
    public AnalyticsCalculator analyticsCalculator;

    [Header("Configuración de Mundos")]
    public WorldConfig[] worlds = new WorldConfig[]
    {
        new WorldConfig { name = "Plano ideal", gravity = 9.81f, dragCoefficient = 0.0f, backgroundColor = new Color(0.81f, 0.94f, 1f) },
        new WorldConfig { name = "Viento leve", gravity = 9.81f, dragCoefficient = 0.5f, backgroundColor = new Color(0.91f, 0.94f, 1f) },
        new WorldConfig { name = "Resistencia alta", gravity = 9.81f, dragCoefficient = 2.0f, backgroundColor = new Color(1f, 0.94f, 0.94f) },
        new WorldConfig { name = "Gravedad baja", gravity = 6.0f, dragCoefficient = 0.6f, backgroundColor = new Color(0.94f, 1f, 0.94f) },
        new WorldConfig { name = "Gravedad alta", gravity = 15.0f, dragCoefficient = 0.3f, backgroundColor = new Color(1f, 0.97f, 0.88f) }
    };

    [Header("Configuración de Pelotas")]
    public BallConfig[] balls = new BallConfig[]
    {
        new BallConfig { mass = 0.2f, radius = 0.4f, color = new Color(1f, 0.4f, 0.4f) },
        new BallConfig { mass = 1.0f, radius = 0.6f, color = new Color(0.4f, 0.64f, 1f) },
        new BallConfig { mass = 5.0f, radius = 0.8f, color = new Color(1f, 0.82f, 0.4f) }
    };

    [Header("Estado del Juego")]
    public GameState currentState = GameState.Ready;
    public int currentWorldIndex = 0;
    public int currentBallIndex = 1;

    // Variables de tiempo
    private float gameTime = 0f;
    private float lastChartUpdate = 0f;
    private const float CHART_UPDATE_INTERVAL = 0.08f;

    // Configuración actual
    private WorldConfig currentWorld;
    private BallConfig currentBall;

    // Score
    public float? finalScore = null;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (currentState == GameState.Running)
        {
            GameLoop();
        }
    }

    /// <summary>
    /// Inicializa el juego con configuración por defecto
    /// </summary>
    public void InitializeGame()
    {
        currentWorld = worlds[currentWorldIndex];
        currentBall = balls[currentBallIndex];

        // Configurar componentes
        physicsSimulation.Initialize(currentWorld, currentBall);
        obstacleManager.GenerateObstacles();
        
        if (uiManager != null)
        {
            uiManager.SetMessage("Presiona 'Iniciar' para empezar");
            uiManager.UpdateWorldList(worlds);
            uiManager.UpdateBallList(balls);
        }

        currentState = GameState.Ready;
        gameTime = 0f;
        finalScore = null;

        UpdateUI();
    }

    /// <summary>
    /// Inicia la simulación
    /// </summary>
    public void StartSimulation()
    {
        if (currentState == GameState.Running) return;

        currentState = GameState.Running;
        gameTime = 0f;
        finalScore = null;

        physicsSimulation.ResetSimulation();
        obstacleManager.GenerateObstacles();

        if (uiManager != null)
        {
            uiManager.SetMessage("Cayendo... usa ← → para esquivar obstáculos");
        }

        Debug.Log("Simulación iniciada");
    }

    /// <summary>
    /// Reinicia el juego
    /// </summary>
    public void ResetSimulation()
    {
        currentState = GameState.Ready;
        gameTime = 0f;
        finalScore = null;

        physicsSimulation.ResetSimulation();
        obstacleManager.GenerateObstacles();

        if (uiManager != null)
        {
            uiManager.SetMessage("Listo. Presiona Iniciar.");
            uiManager.ClearCharts();
        }

        UpdateUI();
        Debug.Log("Simulación reiniciada");
    }

    /// <summary>
    /// Loop principal del juego
    /// </summary>
    private void GameLoop()
    {
        float dt = Time.deltaTime;
        gameTime += dt;

        // Actualizar física
        physicsSimulation.UpdatePhysics(dt);

        // Actualizar control del jugador
        playerController.UpdateMovement(dt);

        // Verificar colisiones
        CheckCollisions();

        // Verificar si tocó el suelo
        if (physicsSimulation.height <= 0f)
        {
            physicsSimulation.height = 0f;
            StopSimulation("Tocaste el suelo");
            return;
        }

        // Actualizar gráficos si es necesario
        if (gameTime - lastChartUpdate > CHART_UPDATE_INTERVAL)
        {
            lastChartUpdate = gameTime;
            UpdateCharts();
        }

        UpdateUI();
    }

    /// <summary>
    /// Verifica colisiones con obstáculos
    /// </summary>
    private void CheckCollisions()
    {
        Vector3 playerPos = playerController.transform.position;
        
        foreach (Obstacle obstacle in obstacleManager.GetObstacles())
        {
            if (obstacle.CheckCollision(playerPos, currentBall.radius))
            {
                if (obstacle.type == ObstacleType.Platform)
                {
                    StopSimulation("Tocaste plataforma");
                    return;
                }
                else if (obstacle.type == ObstacleType.Block)
                {
                    // Reducir velocidad y empujar lateralmente
                    physicsSimulation.velocity = Mathf.Max(0, physicsSimulation.velocity - 4f);
                    float pushDirection = Random.value > 0.5f ? 1f : -1f;
                    playerController.Push(pushDirection * 1.5f);
                }
            }
        }
    }

    /// <summary>
    /// Detiene la simulación
    /// </summary>
    private void StopSimulation(string reason)
    {
        currentState = GameState.Finished;
        finalScore = gameTime;

        if (uiManager != null)
        {
            uiManager.SetMessage($"Final: {reason}. Tiempo: {gameTime:F2} s");
            uiManager.UpdateScore(finalScore.Value);
        }

        UpdateUI();
        UpdateAnalysis();
        
        Debug.Log($"Simulación detenida: {reason}");
    }

    /// <summary>
    /// Actualiza la interfaz de usuario
    /// </summary>
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHeight(physicsSimulation.height);
            uiManager.UpdateVelocity(physicsSimulation.velocity);
            uiManager.UpdateTime(gameTime);
        }
    }

    /// <summary>
    /// Actualiza los gráficos
    /// </summary>
    private void UpdateCharts()
    {
        if (uiManager != null && analyticsCalculator != null)
        {
            // Agregar punto de simulación
            uiManager.AddChartPoint(gameTime, physicsSimulation.height, physicsSimulation.velocity);

            // Si Laplace está activado, agregar solución analítica
            if (uiManager.showLaplace)
            {
                var analytic = analyticsCalculator.CalculateAtTime(
                    gameTime,
                    physicsSimulation.initialHeight,
                    physicsSimulation.initialVelocity,
                    currentWorld.gravity,
                    currentWorld.dragCoefficient,
                    currentBall.mass
                );
                
                uiManager.AddAnalyticPoint(gameTime, analytic.height, analytic.velocity);
            }
        }
    }

    /// <summary>
    /// Actualiza el análisis estadístico
    /// </summary>
    private void UpdateAnalysis()
    {
        if (uiManager != null && analyticsCalculator != null)
        {
            // Tiempo de caída simulado
            float simTime = finalScore ?? 0f;

            // Tiempo de caída analítico
            float analyticTime = analyticsCalculator.FindFallTime(
                physicsSimulation.initialHeight,
                physicsSimulation.initialVelocity,
                currentWorld.gravity,
                currentWorld.dragCoefficient,
                currentBall.mass
            );

            // Velocidad de impacto simulada
            float simImpactVel = physicsSimulation.velocity;

            // Velocidad de impacto analítica
            var analyticImpact = analyticsCalculator.CalculateAtTime(
                analyticTime,
                physicsSimulation.initialHeight,
                physicsSimulation.initialVelocity,
                currentWorld.gravity,
                currentWorld.dragCoefficient,
                currentBall.mass
            );

            // Velocidad terminal
            float terminalVel = analyticsCalculator.CalculateTerminalVelocity(
                currentWorld.gravity,
                currentWorld.dragCoefficient,
                currentBall.mass
            );

            uiManager.UpdateAnalysis(simTime, analyticTime, simImpactVel, analyticImpact.velocity, terminalVel);
        }
    }

    /// <summary>
    /// Cambia el mundo actual
    /// </summary>
    public void SetWorld(int worldIndex)
    {
        if (worldIndex >= 0 && worldIndex < worlds.Length)
        {
            currentWorldIndex = worldIndex;
            currentWorld = worlds[worldIndex];
            
            if (uiManager != null)
            {
                uiManager.UpdatePhysicsInputs(currentWorld.gravity, currentWorld.dragCoefficient);
            }
        }
    }

    /// <summary>
    /// Cambia la pelota actual
    /// </summary>
    public void SetBall(int ballIndex)
    {
        if (ballIndex >= 0 && ballIndex < balls.Length)
        {
            currentBallIndex = ballIndex;
            currentBall = balls[ballIndex];
            
            if (uiManager != null)
            {
                uiManager.UpdatePhysicsInputs(mass: currentBall.mass);
            }

            if (playerController != null)
            {
                playerController.UpdateBallVisuals(currentBall);
            }
        }
    }

    /// <summary>
    /// Aplica configuración personalizada
    /// </summary>
    public void ApplyCustomConfiguration(float gravity, float dragCoeff, float mass, float height, float velocity)
    {
        currentWorld = new WorldConfig
        {
            name = "Custom",
            gravity = gravity,
            dragCoefficient = dragCoeff,
            backgroundColor = currentWorld.backgroundColor
        };

        currentBall.mass = mass;

        physicsSimulation.ApplyConfiguration(currentWorld, currentBall, height, velocity);
        
        ResetSimulation();
    }
}

/// <summary>
/// Estado del juego
/// </summary>
public enum GameState
{
    Ready,
    Running,
    Finished
}

/// <summary>
/// Configuración de un mundo
/// </summary>
[System.Serializable]
public struct WorldConfig
{
    public string name;
    public float gravity;
    public float dragCoefficient;
    public Color backgroundColor;
}

/// <summary>
/// Configuración de una pelota
/// </summary>
[System.Serializable]
public struct BallConfig
{
    public float mass;
    public float radius;
    public Color color;
}
