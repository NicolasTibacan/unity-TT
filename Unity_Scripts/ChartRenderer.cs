using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Renderizador simple de gráficos 2D para Unity
/// Alternativa ligera a Chart.js para Unity 6.2
/// </summary>
[RequireComponent(typeof(RawImage))]
public class ChartRenderer : MonoBehaviour
{
    [Header("Configuración del Gráfico")]
    public string chartTitle = "Gráfico";
    public Color backgroundColor = new Color(1f, 1f, 1f, 0.9f);
    public int width = 700;
    public int height = 200;

    [Header("Series de Datos")]
    public Color simulationColor = new Color(0.17f, 0.55f, 0.75f);
    public Color analyticColor = new Color(1f, 0.5f, 0.06f);
    public float lineWidth = 2f;

    [Header("Ejes")]
    public string xAxisLabel = "Tiempo (s)";
    public string yAxisLabel = "Valor";
    public Color axisColor = Color.black;
    public Color gridColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

    // Datos
    private List<Vector2> simulationData = new List<Vector2>();
    private List<Vector2> analyticData = new List<Vector2>();

    // Textura
    private Texture2D chartTexture;
    private RawImage rawImage;

    // Límites del gráfico
    private float minX = 0f, maxX = 10f;
    private float minY = 0f, maxY = 100f;
    private bool autoScale = true;

    // Máximo de puntos
    private const int MAX_POINTS = 3000;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        InitializeTexture();
    }

    /// <summary>
    /// Inicializa la textura del gráfico
    /// </summary>
    private void InitializeTexture()
    {
        chartTexture = new Texture2D(width, height);
        chartTexture.filterMode = FilterMode.Bilinear;
        
        if (rawImage != null)
        {
            rawImage.texture = chartTexture;
        }

        ClearTexture();
    }

    /// <summary>
    /// Limpia la textura
    /// </summary>
    private void ClearTexture()
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        chartTexture.SetPixels(pixels);
        chartTexture.Apply();
    }

    /// <summary>
    /// Añade un punto de datos
    /// </summary>
    public void AddDataPoint(float x, float y, bool isAnalytic)
    {
        Vector2 point = new Vector2(x, y);

        if (isAnalytic)
        {
            analyticData.Add(point);
            if (analyticData.Count > MAX_POINTS)
            {
                analyticData.RemoveAt(0);
            }
        }
        else
        {
            simulationData.Add(point);
            if (simulationData.Count > MAX_POINTS)
            {
                simulationData.RemoveAt(0);
            }
        }

        UpdateChart();
    }

    /// <summary>
    /// Limpia todos los datos
    /// </summary>
    public void ClearData()
    {
        simulationData.Clear();
        analyticData.Clear();
        ClearTexture();
    }

    /// <summary>
    /// Actualiza el gráfico completo
    /// </summary>
    private void UpdateChart()
    {
        if (autoScale)
        {
            CalculateBounds();
        }

        ClearTexture();
        DrawGrid();
        DrawAxes();
        DrawData(simulationData, simulationColor, false);
        DrawData(analyticData, analyticColor, true);
        
        chartTexture.Apply();
    }

    /// <summary>
    /// Calcula los límites del gráfico automáticamente
    /// </summary>
    private void CalculateBounds()
    {
        if (simulationData.Count == 0 && analyticData.Count == 0)
        {
            minX = 0f; maxX = 10f;
            minY = 0f; maxY = 100f;
            return;
        }

        minX = float.MaxValue;
        maxX = float.MinValue;
        minY = float.MaxValue;
        maxY = float.MinValue;

        foreach (var point in simulationData)
        {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        foreach (var point in analyticData)
        {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        // Añadir margen
        float rangeX = maxX - minX;
        float rangeY = maxY - minY;
        minX -= rangeX * 0.05f;
        maxX += rangeX * 0.05f;
        minY -= rangeY * 0.1f;
        maxY += rangeY * 0.1f;

        // Asegurar rango mínimo
        if (maxX - minX < 0.1f) maxX = minX + 10f;
        if (maxY - minY < 0.1f) maxY = minY + 10f;
    }

    /// <summary>
    /// Dibuja la cuadrícula
    /// </summary>
    private void DrawGrid()
    {
        int gridLinesX = 5;
        int gridLinesY = 5;

        for (int i = 0; i <= gridLinesX; i++)
        {
            int x = Mathf.RoundToInt((float)i / gridLinesX * width);
            DrawVerticalLine(x, gridColor);
        }

        for (int i = 0; i <= gridLinesY; i++)
        {
            int y = Mathf.RoundToInt((float)i / gridLinesY * height);
            DrawHorizontalLine(y, gridColor);
        }
    }

    /// <summary>
    /// Dibuja los ejes
    /// </summary>
    private void DrawAxes()
    {
        // Eje X (abajo)
        DrawHorizontalLine(0, axisColor);
        
        // Eje Y (izquierda)
        DrawVerticalLine(0, axisColor);
    }

    /// <summary>
    /// Dibuja una serie de datos
    /// </summary>
    private void DrawData(List<Vector2> data, Color color, bool dashed)
    {
        if (data.Count < 2) return;

        for (int i = 0; i < data.Count - 1; i++)
        {
            Vector2 p1 = DataToScreen(data[i]);
            Vector2 p2 = DataToScreen(data[i + 1]);

            DrawLine(p1, p2, color, dashed);
        }
    }

    /// <summary>
    /// Convierte coordenadas de datos a coordenadas de pantalla
    /// </summary>
    private Vector2 DataToScreen(Vector2 dataPoint)
    {
        float x = Mathf.Lerp(0, width, Mathf.InverseLerp(minX, maxX, dataPoint.x));
        float y = Mathf.Lerp(0, height, Mathf.InverseLerp(minY, maxY, dataPoint.y));
        return new Vector2(x, y);
    }

    /// <summary>
    /// Dibuja una línea entre dos puntos
    /// </summary>
    private void DrawLine(Vector2 from, Vector2 to, Color color, bool dashed)
    {
        int x0 = Mathf.RoundToInt(from.x);
        int y0 = Mathf.RoundToInt(from.y);
        int x1 = Mathf.RoundToInt(to.x);
        int y1 = Mathf.RoundToInt(to.y);

        // Algoritmo de Bresenham
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int dashCounter = 0;
        int dashLength = 6;

        while (true)
        {
            bool shouldDraw = !dashed || (dashCounter % (dashLength * 2) < dashLength);
            
            if (shouldDraw)
            {
                SetPixelSafe(x0, y0, color);
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }

            dashCounter++;
        }
    }

    /// <summary>
    /// Dibuja una línea horizontal
    /// </summary>
    private void DrawHorizontalLine(int y, Color color)
    {
        if (y < 0 || y >= height) return;
        
        for (int x = 0; x < width; x++)
        {
            SetPixelSafe(x, y, color);
        }
    }

    /// <summary>
    /// Dibuja una línea vertical
    /// </summary>
    private void DrawVerticalLine(int x, Color color)
    {
        if (x < 0 || x >= width) return;
        
        for (int y = 0; y < height; y++)
        {
            SetPixelSafe(x, y, color);
        }
    }

    /// <summary>
    /// Establece un pixel de forma segura
    /// </summary>
    private void SetPixelSafe(int x, int y, Color color)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            chartTexture.SetPixel(x, y, color);
        }
    }
}
