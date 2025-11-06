using UnityEngine;

/// <summary>
/// Calculadora de soluciones analíticas para caída libre con resistencia
/// Usa transformada de Laplace para resolver: a = g - (k/m)*v
/// Compatible con Unity 6.2
/// </summary>
public class AnalyticsCalculator : MonoBehaviour
{
    /// <summary>
    /// Calcula posición y velocidad en un tiempo dado usando solución analítica
    /// </summary>
    public AnalyticResult CalculateAtTime(float t, float h0, float v0, float g, float k, float m)
    {
        AnalyticResult result = new AnalyticResult();

        // Caso sin resistencia (k = 0)
        if (k <= 0.0001f)
        {
            result.velocity = v0 + g * t;
            result.height = h0 - v0 * t - 0.5f * g * t * t;
        }
        else
        {
            // Con resistencia: solución usando transformada de Laplace
            float lambda = k / m;
            float vTerminal = (m * g) / k; // Velocidad terminal (positiva hacia abajo)
            
            float expTerm = Mathf.Exp(-lambda * t);
            
            // v(t) = v_terminal + (v0 - v_terminal) * exp(-lambda * t)
            result.velocity = vTerminal + (v0 - vTerminal) * expTerm;
            
            // y(t) = h0 - v_terminal * t - (v0 - v_terminal) * (1 - exp(-lambda * t)) / lambda
            result.height = h0 - vTerminal * t - (v0 - vTerminal) * (1f - expTerm) / lambda;
        }

        return result;
    }

    /// <summary>
    /// Encuentra el tiempo de caída usando bisección
    /// </summary>
    public float FindFallTime(float h0, float v0, float g, float k, float m, float tmax = 10000f, float tolerance = 0.000001f)
    {
        // Función para evaluar altura en tiempo t
        System.Func<float, float> heightAtTime = (t) => 
        {
            return CalculateAtTime(t, h0, v0, g, k, m).height;
        };

        float a = 0f;
        float b = Mathf.Max(1f, h0 / (g > 0 ? g : 9.81f) * 2f, 10f);

        // Si ya está en el suelo
        if (heightAtTime(a) <= 0f)
            return 0f;

        // Expandir b hasta encontrar cruce con y=0
        int expansionAttempts = 0;
        while (b < tmax && heightAtTime(b) > 0f && expansionAttempts < 50)
        {
            b *= 2f;
            expansionAttempts++;
        }

        // Si nunca cae
        if (heightAtTime(b) > 0f)
            return float.NaN;

        // Bisección
        for (int i = 0; i < 100; i++)
        {
            float mid = 0.5f * (a + b);
            float heightMid = heightAtTime(mid);

            if (Mathf.Abs(heightMid) < tolerance)
                return mid;

            if (heightAtTime(a) * heightMid <= 0f)
                b = mid;
            else
                a = mid;

            // Criterio de convergencia adicional
            if (Mathf.Abs(b - a) < tolerance)
                return 0.5f * (a + b);
        }

        return 0.5f * (a + b);
    }

    /// <summary>
    /// Calcula la velocidad terminal
    /// </summary>
    public float CalculateTerminalVelocity(float g, float k, float m)
    {
        if (k <= 0.0001f)
            return float.PositiveInfinity;

        return (m * g) / k;
    }

    /// <summary>
    /// Calcula el error relativo entre simulación y analítica
    /// </summary>
    public float CalculateRelativeError(float simulated, float analytic)
    {
        if (Mathf.Abs(analytic) < 0.0001f)
            return Mathf.Abs(simulated - analytic);

        return Mathf.Abs((simulated - analytic) / analytic) * 100f;
    }

    /// <summary>
    /// Valida la conservación de energía (aproximada con resistencia)
    /// </summary>
    public EnergyAnalysis AnalyzeEnergy(float height, float velocity, float h0, float v0, float g, float m)
    {
        EnergyAnalysis analysis = new EnergyAnalysis();

        // Energía inicial
        analysis.initialKinetic = 0.5f * m * v0 * v0;
        analysis.initialPotential = m * g * h0;
        analysis.initialTotal = analysis.initialKinetic + analysis.initialPotential;

        // Energía actual
        analysis.currentKinetic = 0.5f * m * velocity * velocity;
        analysis.currentPotential = m * g * height;
        analysis.currentTotal = analysis.currentKinetic + analysis.currentPotential;

        // Energía disipada
        analysis.dissipated = analysis.initialTotal - analysis.currentTotal;

        return analysis;
    }

    /// <summary>
    /// Genera predicciones para múltiples puntos temporales
    /// </summary>
    public AnalyticResult[] GenerateTrajectory(float h0, float v0, float g, float k, float m, float tMax, int points)
    {
        AnalyticResult[] trajectory = new AnalyticResult[points];
        float dt = tMax / (points - 1);

        for (int i = 0; i < points; i++)
        {
            float t = i * dt;
            trajectory[i] = CalculateAtTime(t, h0, v0, g, k, m);
            trajectory[i].time = t;
        }

        return trajectory;
    }
}

/// <summary>
/// Resultado de cálculo analítico
/// </summary>
[System.Serializable]
public struct AnalyticResult
{
    public float time;
    public float height;
    public float velocity;

    public override string ToString()
    {
        return $"t={time:F3}s, h={height:F3}m, v={velocity:F3}m/s";
    }
}

/// <summary>
/// Análisis de energía
/// </summary>
[System.Serializable]
public struct EnergyAnalysis
{
    public float initialKinetic;
    public float initialPotential;
    public float initialTotal;
    public float currentKinetic;
    public float currentPotential;
    public float currentTotal;
    public float dissipated;

    public float GetEfficiency()
    {
        if (initialTotal <= 0.0001f)
            return 0f;
        return (currentTotal / initialTotal) * 100f;
    }

    public override string ToString()
    {
        return $"Energía Total: {currentTotal:F2}J (Inicial: {initialTotal:F2}J)\n" +
               $"Cinética: {currentKinetic:F2}J, Potencial: {currentPotential:F2}J\n" +
               $"Disipada: {dissipated:F2}J ({(100f - GetEfficiency()):F1}%)";
    }
}
