using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simulación de física para caída libre con resistencia del aire
/// Implementa el modelo: a = g - (k/m) * v
/// Compatible con Unity 6.2
/// </summary>
public class PhysicsSimulation : MonoBehaviour
{
    [Header("Parámetros Actuales")]
    public float height = 100f;
    public float velocity = 0f;
    public float initialHeight = 100f;
    public float initialVelocity = 0f;

    [Header("Configuración de Física")]
    public bool liveUpdate = true;

    // Referencias de configuración
    private WorldConfig currentWorld;
    private BallConfig currentBall;

    // Series temporales para análisis
    private List<float> timeHistory = new List<float>();
    private List<float> heightHistory = new List<float>();
    private List<float> velocityHistory = new List<float>();

    private const int MAX_HISTORY_SIZE = 3000;

    /// <summary>
    /// Inicializa la simulación con configuración de mundo y pelota
    /// </summary>
    public void Initialize(WorldConfig world, BallConfig ball)
    {
        currentWorld = world;
        currentBall = ball;
        
        ResetSimulation();
    }

    /// <summary>
    /// Reinicia la simulación a condiciones iniciales
    /// </summary>
    public void ResetSimulation()
    {
        height = initialHeight;
        velocity = initialVelocity;
        
        timeHistory.Clear();
        heightHistory.Clear();
        velocityHistory.Clear();
    }

    /// <summary>
    /// Actualiza la física de caída libre con resistencia
    /// Modelo: a = g - (k/m) * v
    /// </summary>
    public void UpdatePhysics(float deltaTime)
    {
        float g = currentWorld.gravity;
        float k = currentWorld.dragCoefficient;
        float m = currentBall.mass;

        // Calcular aceleración con resistencia del aire
        float acceleration = g - (k / m) * velocity;

        // Integración de Euler (simple pero efectiva para dt pequeños)
        velocity += acceleration * deltaTime;
        height -= velocity * deltaTime;

        // Asegurar que no baje de 0
        if (height < 0f)
        {
            height = 0f;
        }
    }

    /// <summary>
    /// Aplica nueva configuración de física
    /// </summary>
    public void ApplyConfiguration(WorldConfig world, BallConfig ball, float newHeight, float newVelocity)
    {
        currentWorld = world;
        currentBall = ball;
        initialHeight = newHeight;
        initialVelocity = newVelocity;
        
        ResetSimulation();
    }

    /// <summary>
    /// Registra el estado actual en el historial
    /// </summary>
    public void RecordState(float currentTime)
    {
        timeHistory.Add(currentTime);
        heightHistory.Add(height);
        velocityHistory.Add(velocity);

        // Limitar tamaño del historial
        if (timeHistory.Count > MAX_HISTORY_SIZE)
        {
            int excess = timeHistory.Count - MAX_HISTORY_SIZE;
            timeHistory.RemoveRange(0, excess);
            heightHistory.RemoveRange(0, excess);
            velocityHistory.RemoveRange(0, excess);
        }
    }

    /// <summary>
    /// Obtiene el historial de datos
    /// </summary>
    public (List<float> time, List<float> height, List<float> velocity) GetHistory()
    {
        return (timeHistory, heightHistory, velocityHistory);
    }

    /// <summary>
    /// Actualiza la configuración del mundo (para live update)
    /// </summary>
    public void UpdateWorldConfig(float gravity, float dragCoefficient)
    {
        if (liveUpdate)
        {
            currentWorld.gravity = gravity;
            currentWorld.dragCoefficient = dragCoefficient;
        }
    }

    /// <summary>
    /// Actualiza la configuración de la pelota (para live update)
    /// </summary>
    public void UpdateBallConfig(float mass)
    {
        if (liveUpdate)
        {
            currentBall.mass = mass;
        }
    }

    /// <summary>
    /// Calcula la energía cinética actual
    /// </summary>
    public float CalculateKineticEnergy()
    {
        return 0.5f * currentBall.mass * velocity * velocity;
    }

    /// <summary>
    /// Calcula la energía potencial actual
    /// </summary>
    public float CalculatePotentialEnergy()
    {
        return currentBall.mass * currentWorld.gravity * height;
    }

    /// <summary>
    /// Calcula la energía mecánica total
    /// </summary>
    public float CalculateTotalEnergy()
    {
        return CalculateKineticEnergy() + CalculatePotentialEnergy();
    }

    /// <summary>
    /// Obtiene información de depuración
    /// </summary>
    public string GetDebugInfo()
    {
        return $"Altura: {height:F2} m\n" +
               $"Velocidad: {velocity:F2} m/s\n" +
               $"Gravedad: {currentWorld.gravity:F2} m/s²\n" +
               $"Arrastre: {currentWorld.dragCoefficient:F2} kg/s\n" +
               $"Masa: {currentBall.mass:F2} kg\n" +
               $"Energía Cinética: {CalculateKineticEnergy():F2} J\n" +
               $"Energía Potencial: {CalculatePotentialEnergy():F2} J\n" +
               $"Energía Total: {CalculateTotalEnergy():F2} J";
    }
}
