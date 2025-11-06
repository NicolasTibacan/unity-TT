using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Gestiona toda la interfaz de usuario del juego
/// Compatible con Unity 6.2 (usa TextMeshPro)
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Referencias de Texto - Estado")]
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;

    [Header("Referencias de Texto - Análisis")]
    public TextMeshProUGUI fallTimeSimText;
    public TextMeshProUGUI fallTimeAnaText;
    public TextMeshProUGUI impactVelSimText;
    public TextMeshProUGUI impactVelAnaText;
    public TextMeshProUGUI terminalVelText;

    [Header("Dropdowns y Controles")]
    public TMP_Dropdown worldDropdown;
    public TMP_Dropdown ballDropdown;
    public Button startButton;
    public Button resetButton;
    public Button applyButton;
    public Toggle theoryToggle;
    public Toggle laplaceToggle;
    public Toggle liveUpdateToggle;

    [Header("Input Fields")]
    public TMP_InputField gravityInput;
    public TMP_InputField dragInput;
    public TMP_InputField massInput;
    public TMP_InputField heightInput;
    public TMP_InputField velocityInput;

    [Header("Paneles")]
    public GameObject theoryPanel;
    public GameObject analysisPanel;

    [Header("Gráficos (Opcional)")]
    public ChartRenderer heightChart;
    public ChartRenderer velocityChart;

    // Estado
    public bool showLaplace = false;
    
    // Referencia al GameManager
    private GameManager gameManager;

    void Awake()
    {
        // Encontrar el GameManager
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No se encontró GameManager en la escena");
        }

        // Configurar listeners
        SetupEventListeners();

        // Configurar valores por defecto
        SetDefaultValues();
    }

    /// <summary>
    /// Configura los listeners de eventos
    /// </summary>
    private void SetupEventListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(() => gameManager?.StartSimulation());

        if (resetButton != null)
            resetButton.onClick.AddListener(() => gameManager?.ResetSimulation());

        if (applyButton != null)
            applyButton.onClick.AddListener(ApplyCustomConfiguration);

        if (theoryToggle != null)
            theoryToggle.onValueChanged.AddListener(ToggleTheoryPanel);

        if (laplaceToggle != null)
            laplaceToggle.onValueChanged.AddListener((value) => showLaplace = value);

        if (worldDropdown != null)
            worldDropdown.onValueChanged.AddListener((index) => gameManager?.SetWorld(index));

        if (ballDropdown != null)
            ballDropdown.onValueChanged.AddListener((index) => gameManager?.SetBall(index));
    }

    /// <summary>
    /// Establece valores por defecto en los inputs
    /// </summary>
    private void SetDefaultValues()
    {
        if (gravityInput != null) gravityInput.text = "9.81";
        if (dragInput != null) dragInput.text = "0.5";
        if (massInput != null) massInput.text = "1.0";
        if (heightInput != null) heightInput.text = "100";
        if (velocityInput != null) velocityInput.text = "0";

        if (liveUpdateToggle != null) liveUpdateToggle.isOn = true;
        if (theoryPanel != null) theoryPanel.SetActive(false);
    }

    /// <summary>
    /// Actualiza la altura en la UI
    /// </summary>
    public void UpdateHeight(float height)
    {
        if (heightText != null)
            heightText.text = $"{height:F2}";
    }

    /// <summary>
    /// Actualiza la velocidad en la UI
    /// </summary>
    public void UpdateVelocity(float velocity)
    {
        if (velocityText != null)
            velocityText.text = $"{velocity:F2}";
    }

    /// <summary>
    /// Actualiza el tiempo en la UI
    /// </summary>
    public void UpdateTime(float time)
    {
        if (timeText != null)
            timeText.text = $"{time:F2}";
    }

    /// <summary>
    /// Actualiza el score en la UI
    /// </summary>
    public void UpdateScore(float score)
    {
        if (scoreText != null)
            scoreText.text = $"{score:F2}";
    }

    /// <summary>
    /// Establece el mensaje de estado
    /// </summary>
    public void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    /// <summary>
    /// Actualiza el análisis estadístico
    /// </summary>
    public void UpdateAnalysis(float simTime, float anaTime, float simImpactVel, float anaImpactVel, float terminalVel)
    {
        if (fallTimeSimText != null)
            fallTimeSimText.text = simTime > 0 ? $"{simTime:F3}" : "-";

        if (fallTimeAnaText != null)
            fallTimeAnaText.text = !float.IsNaN(anaTime) ? $"{anaTime:F3}" : "—";

        if (impactVelSimText != null)
            impactVelSimText.text = simTime > 0 ? $"{simImpactVel:F3}" : "-";

        if (impactVelAnaText != null)
            impactVelAnaText.text = !float.IsNaN(anaImpactVel) ? $"{anaImpactVel:F3}" : "—";

        if (terminalVelText != null)
            terminalVelText.text = float.IsInfinity(terminalVel) ? "∞" : $"{terminalVel:F3}";
    }

    /// <summary>
    /// Actualiza las listas de mundos y pelotas
    /// </summary>
    public void UpdateWorldList(WorldConfig[] worlds)
    {
        if (worldDropdown != null)
        {
            worldDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var world in worlds)
            {
                options.Add(world.name);
            }
            worldDropdown.AddOptions(options);
        }
    }

    public void UpdateBallList(BallConfig[] balls)
    {
        if (ballDropdown != null)
        {
            ballDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < balls.Length; i++)
            {
                string label = i == 0 ? "Ligera" : i == 1 ? "Media" : "Pesada";
                options.Add($"{label} ({balls[i].mass} kg)");
            }
            ballDropdown.AddOptions(options);
            ballDropdown.value = 1; // Seleccionar "Media" por defecto
        }
    }

    /// <summary>
    /// Actualiza los inputs de física
    /// </summary>
    public void UpdatePhysicsInputs(float? gravity = null, float? drag = null, float? mass = null)
    {
        if (gravity.HasValue && gravityInput != null)
            gravityInput.text = gravity.Value.ToString("F2");

        if (drag.HasValue && dragInput != null)
            dragInput.text = drag.Value.ToString("F2");

        if (mass.HasValue && massInput != null)
            massInput.text = mass.Value.ToString("F2");
    }

    /// <summary>
    /// Aplica configuración personalizada
    /// </summary>
    private void ApplyCustomConfiguration()
    {
        if (gameManager == null) return;

        float g = ParseInputFloat(gravityInput, 9.81f);
        float k = ParseInputFloat(dragInput, 0.5f);
        float m = ParseInputFloat(massInput, 1.0f);
        float h0 = ParseInputFloat(heightInput, 100f);
        float v0 = ParseInputFloat(velocityInput, 0f);

        gameManager.ApplyCustomConfiguration(g, k, m, h0, v0);
    }

    /// <summary>
    /// Parsea un input field a float
    /// </summary>
    private float ParseInputFloat(TMP_InputField input, float defaultValue)
    {
        if (input == null) return defaultValue;
        
        if (float.TryParse(input.text, out float result))
            return result;
        
        return defaultValue;
    }

    /// <summary>
    /// Toggle del panel de teoría
    /// </summary>
    private void ToggleTheoryPanel(bool show)
    {
        if (theoryPanel != null)
            theoryPanel.SetActive(show);
    }

    /// <summary>
    /// Añade un punto al gráfico (simulación)
    /// </summary>
    public void AddChartPoint(float time, float height, float velocity)
    {
        if (heightChart != null)
            heightChart.AddDataPoint(time, height, false);

        if (velocityChart != null)
            velocityChart.AddDataPoint(time, velocity, false);
    }

    /// <summary>
    /// Añade un punto analítico al gráfico
    /// </summary>
    public void AddAnalyticPoint(float time, float height, float velocity)
    {
        if (showLaplace)
        {
            if (heightChart != null)
                heightChart.AddDataPoint(time, height, true);

            if (velocityChart != null)
                velocityChart.AddDataPoint(time, velocity, true);
        }
    }

    /// <summary>
    /// Limpia los gráficos
    /// </summary>
    public void ClearCharts()
    {
        if (heightChart != null)
            heightChart.ClearData();

        if (velocityChart != null)
            velocityChart.ClearData();
    }
}
