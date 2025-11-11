using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    [SerializeField] private float danio = 15f;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float tiempoVida = 5f;
    
    private Rigidbody2D rb;
    private Vector2 direccion;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Destruir el proyectil después de un tiempo
        Destroy(gameObject, tiempoVida);
    }

    void FixedUpdate()
    {
        // Mover el proyectil
        if (rb != null && direccion != Vector2.zero)
        {
            rb.velocity = direccion * velocidad;
        }
    }

    public void ConfigurarDireccion(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;
        
        // Rotar el proyectil hacia la dirección
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    public float ObtenerDanio()
    {
        return danio;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destruir al chocar con algo (excepto enemigos)
        if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Enemigo"))
        {
            Destroy(gameObject);
        }
    }
}
