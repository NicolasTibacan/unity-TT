using UnityEngine;

/// <summary>
/// ScriptableObject para configurar mundos
/// Permite crear assets de configuración reutilizables
/// Compatible con Unity 6.2
/// </summary>
[CreateAssetMenu(fileName = "NewWorld", menuName = "FreeFall/World Config", order = 1)]
public class WorldConfigSO : ScriptableObject
{
    [Header("Información")]
    public string worldName = "Nuevo Mundo";
    
    [TextArea(3, 5)]
    public string description = "Descripción del mundo";

    [Header("Parámetros Físicos")]
    [Tooltip("Aceleración de la gravedad en m/s²")]
    [Range(0f, 30f)]
    public float gravity = 9.81f;

    [Tooltip("Coeficiente de arrastre del aire en kg/s")]
    [Range(0f, 5f)]
    public float dragCoefficient = 0.5f;

    [Header("Visual")]
    public Color backgroundColor = new Color(0.81f, 0.94f, 1f);
    public Material skyboxMaterial;

    [Header("Ambiente")]
    public string environmentDescription = "Condiciones normales";
    
    /// <summary>
    /// Convierte a WorldConfig
    /// </summary>
    public WorldConfig ToWorldConfig()
    {
        return new WorldConfig
        {
            name = worldName,
            gravity = gravity,
            dragCoefficient = dragCoefficient,
            backgroundColor = backgroundColor
        };
    }
}
