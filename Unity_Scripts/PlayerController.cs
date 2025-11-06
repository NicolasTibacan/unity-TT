using UnityEngine;

/// <summary>
/// Controla el movimiento horizontal del jugador
/// Compatible con Unity 6.2
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento horizontal en unidades/segundo")]
    public float moveSpeed = 10f;
    
    [Tooltip("Límites horizontales del área de juego")]
    public float minX = -15f;
    public float maxX = 15f;

    [Header("Referencias")]
    public PhysicsSimulation physicsSimulation;
    
    [Header("Visual")]
    public GameObject ballVisual;
    private Renderer ballRenderer;
    private Transform ballTransform;

    // Input
    private float horizontalInput = 0f;

    // Rigidbody para física
    private Rigidbody rb;

    // Push externo (cuando choca con bloques)
    private float externalPush = 0f;
    private float pushDecayRate = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Configurar rigidbody para movimiento cinemático
        rb.isKinematic = true;
        rb.useGravity = false;

        // Configurar visual de la pelota
        if (ballVisual != null)
        {
            ballRenderer = ballVisual.GetComponent<Renderer>();
            ballTransform = ballVisual.transform;
        }
        else
        {
            Debug.LogWarning("No se asignó ballVisual en PlayerController");
        }
    }

    void Update()
    {
        // Capturar input
        CaptureInput();
    }

    /// <summary>
    /// Captura el input del jugador
    /// </summary>
    private void CaptureInput()
    {
        // Input horizontal (flechas o A/D)
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Actualiza el movimiento del jugador
    /// </summary>
    public void UpdateMovement(float deltaTime)
    {
        // Movimiento por input
        float movement = horizontalInput * moveSpeed * deltaTime;
        
        // Añadir push externo
        if (Mathf.Abs(externalPush) > 0.01f)
        {
            movement += externalPush * deltaTime;
            externalPush = Mathf.Lerp(externalPush, 0f, pushDecayRate * deltaTime);
        }

        // Calcular nueva posición
        Vector3 newPosition = transform.position;
        newPosition.x += movement;

        // Aplicar límites
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // Actualizar posición vertical según física
        if (physicsSimulation != null)
        {
            newPosition.y = physicsSimulation.height;
        }

        // Aplicar nueva posición
        transform.position = newPosition;
    }

    /// <summary>
    /// Aplica un empuje lateral (usado cuando choca con bloques)
    /// </summary>
    public void Push(float force)
    {
        externalPush += force;
    }

    /// <summary>
    /// Actualiza el visual de la pelota según configuración
    /// </summary>
    public void UpdateBallVisuals(BallConfig ballConfig)
    {
        if (ballTransform != null)
        {
            // Escalar el visual según el radio
            float scale = ballConfig.radius;
            ballTransform.localScale = Vector3.one * scale;
        }

        if (ballRenderer != null)
        {
            // Cambiar color del material
            ballRenderer.material.color = ballConfig.color;
        }
    }

    /// <summary>
    /// Resetea la posición del jugador
    /// </summary>
    public void ResetPosition(Vector3 startPosition)
    {
        transform.position = startPosition;
        externalPush = 0f;
    }

    /// <summary>
    /// Obtiene la posición actual
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos()
    {
        // Dibujar límites del área de juego
        Gizmos.color = Color.yellow;
        Vector3 leftBound = new Vector3(minX, transform.position.y, 0);
        Vector3 rightBound = new Vector3(maxX, transform.position.y, 0);
        Gizmos.DrawLine(leftBound, leftBound + Vector3.up * 2f);
        Gizmos.DrawLine(rightBound, rightBound + Vector3.up * 2f);
    }
}
