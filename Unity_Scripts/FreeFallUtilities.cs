using UnityEngine;

/// <summary>
/// Utilidades y helpers para el juego de caída libre
/// Compatible con Unity 6.2
/// </summary>
public static class FreeFallUtilities
{
    /// <summary>
    /// Convierte metros a unidades Unity (1:1 por defecto)
    /// </summary>
    public static float MetersToUnits(float meters)
    {
        return meters;
    }

    /// <summary>
    /// Convierte unidades Unity a metros (1:1 por defecto)
    /// </summary>
    public static float UnitsToMeters(float units)
    {
        return units;
    }

    /// <summary>
    /// Formatea un valor float con unidades
    /// </summary>
    public static string FormatValue(float value, string unit, int decimals = 2)
    {
        return $"{value.ToString($"F{decimals}")} {unit}";
    }

    /// <summary>
    /// Calcula el tiempo de caída sin resistencia (caso ideal)
    /// </summary>
    public static float CalculateFreeFlightTime(float height, float gravity)
    {
        if (gravity <= 0f || height <= 0f) return 0f;
        return Mathf.Sqrt(2f * height / gravity);
    }

    /// <summary>
    /// Calcula la velocidad de impacto sin resistencia
    /// </summary>
    public static float CalculateImpactVelocity(float height, float gravity)
    {
        if (gravity <= 0f || height <= 0f) return 0f;
        return Mathf.Sqrt(2f * gravity * height);
    }

    /// <summary>
    /// Calcula la energía cinética
    /// </summary>
    public static float CalculateKineticEnergy(float mass, float velocity)
    {
        return 0.5f * mass * velocity * velocity;
    }

    /// <summary>
    /// Calcula la energía potencial gravitatoria
    /// </summary>
    public static float CalculatePotentialEnergy(float mass, float gravity, float height)
    {
        return mass * gravity * height;
    }

    /// <summary>
    /// Interpola suavemente entre dos colores
    /// </summary>
    public static Color LerpColor(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, Mathf.SmoothStep(0f, 1f, t));
    }

    /// <summary>
    /// Genera un color aleatorio vibrante
    /// </summary>
    public static Color RandomVibrantColor()
    {
        return Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
    }

    /// <summary>
    /// Clamp con wraparound (para posiciones horizontales cíclicas)
    /// </summary>
    public static float WrapClamp(float value, float min, float max)
    {
        float range = max - min;
        if (value < min)
            return max - (min - value) % range;
        if (value > max)
            return min + (value - max) % range;
        return value;
    }

    /// <summary>
    /// Genera un gradiente de color basado en velocidad
    /// </summary>
    public static Color VelocityToColor(float velocity, float maxVelocity)
    {
        float t = Mathf.Clamp01(velocity / maxVelocity);
        
        // Verde (lento) -> Amarillo (medio) -> Rojo (rápido)
        if (t < 0.5f)
        {
            return Color.Lerp(Color.green, Color.yellow, t * 2f);
        }
        else
        {
            return Color.Lerp(Color.yellow, Color.red, (t - 0.5f) * 2f);
        }
    }

    /// <summary>
    /// Dibuja una flecha de debug en el editor
    /// </summary>
    public static void DrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
    {
        Debug.DrawRay(origin, direction, color);

        if (direction != Vector3.zero)
        {
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
            
            Debug.DrawRay(origin + direction, right * arrowHeadLength, color);
            Debug.DrawRay(origin + direction, left * arrowHeadLength, color);
        }
    }

    /// <summary>
    /// Calcula el porcentaje de error
    /// </summary>
    public static float CalculatePercentError(float measured, float actual)
    {
        if (Mathf.Abs(actual) < 0.0001f)
            return Mathf.Abs(measured - actual);
        
        return Mathf.Abs((measured - actual) / actual) * 100f;
    }

    /// <summary>
    /// Formatea el tiempo en formato MM:SS.mmm
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000f) % 1000f);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    /// <summary>
    /// Valida que un valor esté en un rango razonable
    /// </summary>
    public static bool IsValidPhysicsValue(float value, float min = -1000f, float max = 1000f)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && value >= min && value <= max;
    }

    /// <summary>
    /// Safe division que retorna 0 si el divisor es 0
    /// </summary>
    public static float SafeDivide(float numerator, float denominator, float defaultValue = 0f)
    {
        if (Mathf.Abs(denominator) < 0.0001f)
            return defaultValue;
        return numerator / denominator;
    }

    /// <summary>
    /// Interpola exponencialmente (para suavizado de cámara, etc.)
    /// </summary>
    public static float ExpDecay(float a, float b, float decay, float deltaTime)
    {
        return b + (a - b) * Mathf.Exp(-decay * deltaTime);
    }
}

/// <summary>
/// Extensiones útiles para Vector3
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Establece solo el componente Y
    /// </summary>
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    /// <summary>
    /// Establece solo el componente X
    /// </summary>
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    /// <summary>
    /// Establece solo el componente Z
    /// </summary>
    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    /// <summary>
    /// Aplana el vector al plano XZ (Y = 0)
    /// </summary>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}
