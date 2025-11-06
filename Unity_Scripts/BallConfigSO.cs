using UnityEngine;

/// <summary>
/// ScriptableObject para configurar pelotas
/// Permite crear assets de configuración reutilizables
/// Compatible con Unity 6.2
/// </summary>
[CreateAssetMenu(fileName = "NewBall", menuName = "FreeFall/Ball Config", order = 2)]
public class BallConfigSO : ScriptableObject
{
    [Header("Información")]
    public string ballName = "Nueva Pelota";
    
    [TextArea(2, 4)]
    public string description = "Descripción de la pelota";

    [Header("Propiedades Físicas")]
    [Tooltip("Masa en kilogramos")]
    [Range(0.01f, 100f)]
    public float mass = 1.0f;

    [Tooltip("Radio visual en unidades Unity")]
    [Range(0.1f, 2f)]
    public float radius = 0.6f;

    [Header("Visual")]
    public Color color = new Color(0.4f, 0.64f, 1f);
    public Material material;
    public GameObject prefab;

    [Header("Efectos")]
    public ParticleSystem trailEffect;
    public AudioClip impactSound;

    /// <summary>
    /// Convierte a BallConfig
    /// </summary>
    public BallConfig ToBallConfig()
    {
        return new BallConfig
        {
            mass = mass,
            radius = radius,
            color = color
        };
    }

    /// <summary>
    /// Calcula la densidad de la pelota
    /// </summary>
    public float CalculateDensity()
    {
        float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3f);
        return mass / volume;
    }
}
