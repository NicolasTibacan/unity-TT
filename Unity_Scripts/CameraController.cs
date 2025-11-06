using UnityEngine;

/// <summary>
/// Controlador de cámara para el juego de caída libre
/// Sigue al jugador en el eje Y con suavizado
/// Compatible con Unity 6.2
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del jugador a seguir")]
    public Transform target;

    [Header("Configuración de Seguimiento")]
    [Tooltip("Velocidad de suavizado de la cámara")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("Offset vertical respecto al jugador")]
    public float verticalOffset = 10f;

    [Tooltip("Distancia en Z de la cámara")]
    public float cameraDistance = -20f;

    [Header("Límites")]
    [Tooltip("Altura mínima de la cámara")]
    public float minHeight = 0f;

    [Tooltip("Altura máxima de la cámara")]
    public float maxHeight = 120f;

    [Tooltip("Si debe seguir al jugador horizontalmente")]
    public bool followHorizontal = false;

    [Header("Zoom")]
    [Tooltip("Tamaño ortográfico de la cámara")]
    public float orthographicSize = 15f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        
        if (cam == null)
        {
            Debug.LogError("CameraController requiere un componente Camera");
            return;
        }

        // Configurar cámara ortográfica
        cam.orthographic = true;
        cam.orthographicSize = orthographicSize;
    }

    void Start()
    {
        // Si no se asignó target, buscar el jugador
        if (target == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró target para la cámara");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcular posición deseada
        Vector3 desiredPosition = transform.position;

        // Seguir verticalmente al target
        desiredPosition.y = target.position.y + verticalOffset;

        // Seguir horizontalmente si está habilitado
        if (followHorizontal)
        {
            desiredPosition.x = target.position.x;
        }

        // Aplicar límites
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minHeight, maxHeight);

        // Mantener distancia en Z
        desiredPosition.z = cameraDistance;

        // Suavizar movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicar posición
        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Establece un nuevo target
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Centra la cámara instantáneamente en el target
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 position = transform.position;
        position.y = target.position.y + verticalOffset;
        
        if (followHorizontal)
        {
            position.x = target.position.x;
        }

        position.z = cameraDistance;
        transform.position = position;
    }

    /// <summary>
    /// Ajusta el zoom de la cámara
    /// </summary>
    public void SetZoom(float newSize)
    {
        if (cam != null && cam.orthographic)
        {
            orthographicSize = Mathf.Max(5f, newSize);
            cam.orthographicSize = orthographicSize;
        }
    }
}
