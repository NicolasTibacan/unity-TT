using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona la generación y actualización de obstáculos
/// Compatible con Unity 6.2
/// </summary>
public class ObstacleManager : MonoBehaviour
{
    [Header("Configuración de Obstáculos")]
    [Tooltip("Número de obstáculos a generar")]
    public int obstacleCount = 8;

    [Tooltip("Prefab para plataformas")]
    public GameObject platformPrefab;

    [Tooltip("Prefab para bloques peligrosos")]
    public GameObject blockPrefab;

    [Tooltip("Probabilidad de generar plataforma vs bloque (0-1)")]
    [Range(0f, 1f)]
    public float platformProbability = 0.7f;

    [Header("Límites del Área")]
    public float minX = -12f;
    public float maxX = 12f;

    [Header("Referencias")]
    public Transform obstacleContainer;

    // Lista de obstáculos activos
    private List<Obstacle> activeObstacles = new List<Obstacle>();

    void Awake()
    {
        // Crear contenedor si no existe
        if (obstacleContainer == null)
        {
            GameObject container = new GameObject("ObstacleContainer");
            obstacleContainer = container.transform;
            obstacleContainer.SetParent(transform);
        }

        // Verificar prefabs
        if (platformPrefab == null)
        {
            Debug.LogError("No se asignó platformPrefab en ObstacleManager");
        }
        if (blockPrefab == null)
        {
            Debug.LogError("No se asignó blockPrefab en ObstacleManager");
        }
    }

    /// <summary>
    /// Genera obstáculos aleatorios en el nivel
    /// </summary>
    public void GenerateObstacles()
    {
        // Limpiar obstáculos anteriores
        ClearObstacles();

        float heightStep = 100f / (obstacleCount + 1);

        for (int i = 1; i <= obstacleCount; i++)
        {
            // Altura con variación aleatoria
            float height = 100f - i * heightStep + Random.Range(-2f, 2f);

            // Posición X aleatoria dentro de los límites
            float xPos = Random.Range(minX + 3f, maxX - 3f);

            // Ancho aleatorio
            float width = Random.Range(3f, 7f);

            // Tipo de obstáculo
            ObstacleType type = Random.value < platformProbability 
                ? ObstacleType.Platform 
                : ObstacleType.Block;

            // Crear obstáculo
            CreateObstacle(height, xPos, width, type);
        }

        Debug.Log($"Generados {activeObstacles.Count} obstáculos");
    }

    /// <summary>
    /// Crea un obstáculo individual
    /// </summary>
    private void CreateObstacle(float height, float xPos, float width, ObstacleType type)
    {
        GameObject prefab = type == ObstacleType.Platform ? platformPrefab : blockPrefab;
        
        if (prefab == null) return;

        // Instanciar el obstáculo
        GameObject obstacleObj = Instantiate(prefab, obstacleContainer);
        obstacleObj.name = $"{type}_{activeObstacles.Count}";

        // Posicionar
        Vector3 position = new Vector3(xPos, height, 0);
        obstacleObj.transform.position = position;

        // Escalar según ancho
        Vector3 scale = obstacleObj.transform.localScale;
        scale.x = width;
        obstacleObj.transform.localScale = scale;

        // Obtener o añadir componente Obstacle
        Obstacle obstacle = obstacleObj.GetComponent<Obstacle>();
        if (obstacle == null)
        {
            obstacle = obstacleObj.AddComponent<Obstacle>();
        }

        // Configurar
        obstacle.Initialize(height, xPos, width, type);

        // Añadir a la lista
        activeObstacles.Add(obstacle);
    }

    /// <summary>
    /// Limpia todos los obstáculos
    /// </summary>
    public void ClearObstacles()
    {
        foreach (Obstacle obstacle in activeObstacles)
        {
            if (obstacle != null && obstacle.gameObject != null)
            {
                Destroy(obstacle.gameObject);
            }
        }

        activeObstacles.Clear();
    }

    /// <summary>
    /// Obtiene la lista de obstáculos activos
    /// </summary>
    public List<Obstacle> GetObstacles()
    {
        return activeObstacles;
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        // Dibujar límites del área
        Vector3 bottomLeft = new Vector3(minX, 0, 0);
        Vector3 bottomRight = new Vector3(maxX, 0, 0);
        Vector3 topLeft = new Vector3(minX, 100, 0);
        Vector3 topRight = new Vector3(maxX, 100, 0);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }
}

/// <summary>
/// Representa un obstáculo individual (plataforma o bloque)
/// </summary>
public class Obstacle : MonoBehaviour
{
    public float height;
    public float xPosition;
    public float width;
    public ObstacleType type;

    private BoxCollider boxCollider;
    private Renderer obstacleRenderer;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        obstacleRenderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Inicializa el obstáculo con sus parámetros
    /// </summary>
    public void Initialize(float h, float x, float w, ObstacleType t)
    {
        height = h;
        xPosition = x;
        width = w;
        type = t;

        // Configurar collider
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
            
            if (type == ObstacleType.Platform)
            {
                boxCollider.size = new Vector3(1f, 0.4f, 1f);
            }
            else
            {
                boxCollider.size = new Vector3(1f, 1.2f, 1f);
            }
        }

        // Configurar color
        if (obstacleRenderer != null)
        {
            Color color = type == ObstacleType.Platform 
                ? new Color(0.27f, 0.27f, 0.27f) // Gris oscuro
                : new Color(0.54f, 0f, 0f);      // Rojo oscuro

            obstacleRenderer.material.color = color;
        }
    }

    /// <summary>
    /// Verifica colisión con el jugador
    /// </summary>
    public bool CheckCollision(Vector3 playerPosition, float playerRadius)
    {
        // Verificar distancia vertical
        float verticalDistance = Mathf.Abs(playerPosition.y - height);
        
        // Verificar si está dentro del rango horizontal
        float minX = xPosition - width / 2f;
        float maxX = xPosition + width / 2f;
        bool withinX = playerPosition.x >= (minX - playerRadius) && 
                       playerPosition.x <= (maxX + playerRadius);

        // Umbral de colisión según tipo
        float collisionThreshold = type == ObstacleType.Platform ? 0.6f : 0.8f;

        return verticalDistance < collisionThreshold && withinX;
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos()
    {
        if (type == ObstacleType.Platform)
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
        else
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
        }

        Vector3 center = transform.position;
        Vector3 size = new Vector3(width, type == ObstacleType.Platform ? 0.4f : 1.2f, 1f);
        Gizmos.DrawCube(center, size);
    }
}

/// <summary>
/// Tipos de obstáculos
/// </summary>
public enum ObstacleType
{
    Platform,  // Plataforma segura (termina el juego)
    Block      // Bloque peligroso (reduce velocidad)
}
