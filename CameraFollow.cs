using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    [SerializeField] private Transform objetivo;
    
    [Header("Zona de Cámara")]
    [SerializeField] private float margenIzquierdo = 3f; // Distancia desde el borde izquierdo donde la cámara empieza a seguir
    [SerializeField] private float margenDerecho = 3f;   // Distancia desde el borde derecho donde la cámara empieza a seguir
    [SerializeField] private float suavizado = 0.1f;
    
    [Header("Límites de la Zona")]
    [SerializeField] private float limiteIzquierdoZona = -10f;
    [SerializeField] private float limiteDerechoZona = 10f;
    [SerializeField] private float limiteInferiorZona = -5f;
    [SerializeField] private float limiteSuperiorZona = 5f;
    
    [Header("Configuración")]
    [SerializeField] private float distanciaZ = -10f; // Distancia de la cámara en Z
    [SerializeField] private bool seguirEnY = false;  // Por defecto no sigue en Y para plataformas
    [SerializeField] private float alturaFija = 0f;   // Altura fija de la cámara
    
    private Camera cam;
    private float alturaCamera;
    private float anchoCamera;

    void Start()
    {
        // Buscar automáticamente el objeto skull si no está asignado
        if (objetivo == null)
        {
            GameObject skull = GameObject.FindGameObjectWithTag("Player");
            if (skull != null)
            {
                objetivo = skull.transform;
                Debug.Log("CameraFollow: Objetivo 'Player' encontrado automáticamente.");
            }
            else
            {
                Debug.LogWarning("CameraFollow: No se encontró un objetivo. Asigna el objeto skull manualmente o dale el tag 'Player'.");
            }
        }
        
        // Obtener la cámara y calcular dimensiones
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            alturaCamera = cam.orthographicSize;
            anchoCamera = alturaCamera * cam.aspect;
        }
        
        // Fijar la rotación de la cámara
        transform.rotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        if (objetivo == null) return;
        
        // Asegurar que la cámara no rote
        transform.rotation = Quaternion.identity;
        
        // Posición deseada siguiendo al personaje SIEMPRE
        Vector3 posicionDeseada = objetivo.position;
        
        // Mantener altura fija o seguir en Y
        if (!seguirEnY)
        {
            posicionDeseada.y = alturaFija;
        }
        
        // Mantener la distancia Z fija
        posicionDeseada.z = distanciaZ;
        
        // Aplicar movimiento suave
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado);
    }
    
    // Método para cambiar los límites de la zona dinámicamente
    public void CambiarZona(float izquierda, float derecha, float inferior, float superior)
    {
        limiteIzquierdoZona = izquierda;
        limiteDerechoZona = derecha;
        limiteInferiorZona = inferior;
        limiteSuperiorZona = superior;
    }
    
    // Método para ajustar los márgenes
    public void AjustarMargenes(float izquierdo, float derecho)
    {
        margenIzquierdo = izquierdo;
        margenDerecho = derecho;
    }
    
    // Visualizar los límites de la zona en el editor
    void OnDrawGizmosSelected()
    {
        // Dibujar límites de la zona
        Gizmos.color = Color.green;
        
        // Línea izquierda
        Gizmos.DrawLine(new Vector3(limiteIzquierdoZona, limiteInferiorZona, 0), 
                       new Vector3(limiteIzquierdoZona, limiteSuperiorZona, 0));
        
        // Línea derecha
        Gizmos.DrawLine(new Vector3(limiteDerechoZona, limiteInferiorZona, 0), 
                       new Vector3(limiteDerechoZona, limiteSuperiorZona, 0));
        
        // Línea inferior
        Gizmos.DrawLine(new Vector3(limiteIzquierdoZona, limiteInferiorZona, 0), 
                       new Vector3(limiteDerechoZona, limiteInferiorZona, 0));
        
        // Línea superior
        Gizmos.DrawLine(new Vector3(limiteIzquierdoZona, limiteSuperiorZona, 0), 
                       new Vector3(limiteDerechoZona, limiteSuperiorZona, 0));
        
        // Dibujar márgenes
        if (cam != null)
        {
            Gizmos.color = Color.yellow;
            float altura = cam.orthographicSize;
            float ancho = altura * cam.aspect;
            Vector3 pos = transform.position;
            
            // Línea margen izquierdo
            float bordeIzq = pos.x - ancho + margenIzquierdo;
            Gizmos.DrawLine(new Vector3(bordeIzq, pos.y - altura, 0), 
                           new Vector3(bordeIzq, pos.y + altura, 0));
            
            // Línea margen derecho
            float bordeDer = pos.x + ancho - margenDerecho;
            Gizmos.DrawLine(new Vector3(bordeDer, pos.y - altura, 0), 
                           new Vector3(bordeDer, pos.y + altura, 0));
        }
    }
}
