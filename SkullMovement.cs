using UnityEngine;

public class SkullMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float fuerzaSalto = 10f;
    
    [Header("Detección de Suelo")]
    [SerializeField] private Transform verificadorSuelo;
    [SerializeField] private float radioVerificacion = 0.2f;
    [SerializeField] private LayerMask capaSuelo;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool enSuelo;
    private float movimientoHorizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Crear verificador de suelo automáticamente si no existe
        if (verificadorSuelo == null)
        {
            GameObject verificador = new GameObject("VerificadorSuelo");
            verificador.transform.parent = transform;
            verificador.transform.localPosition = new Vector3(0, -0.5f, 0);
            verificadorSuelo = verificador.transform;
        }
    }

    void Update()
    {
        // Capturar input horizontal (A y D)
        movimientoHorizontal = Input.GetAxis("Horizontal");
        
        // Verificar si está en el suelo
        enSuelo = Physics2D.OverlapCircle(verificadorSuelo.position, radioVerificacion, capaSuelo);
        
        // Saltar con W
        if (Input.GetKeyDown(KeyCode.W) && enSuelo)
        {
            Saltar();
        }
        
        // Actualizar animaciones
        ActualizarAnimaciones();
        
        // Voltear sprite según dirección
        if (movimientoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimientoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        // Mover el personaje horizontalmente
        if (rb != null)
        {
            rb.velocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.velocity.y);
        }
    }
    
    void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        
        // Activar animación de salto si existe
        if (animator != null)
        {
            animator.SetTrigger("Salto");
        }
    }
    
    void ActualizarAnimaciones()
    {
        if (animator != null)
        {
            // Parámetros de animación
            animator.SetFloat("Velocidad", Mathf.Abs(movimientoHorizontal));
            animator.SetBool("EnSuelo", enSuelo);
            animator.SetFloat("VelocidadY", rb.velocity.y);
        }
    }
    
    // Visualizar el área de detección de suelo en el editor
    void OnDrawGizmosSelected()
    {
        if (verificadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(verificadorSuelo.position, radioVerificacion);
        }
    }
}
