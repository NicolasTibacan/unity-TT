using UnityEngine;

public class SkullAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float danioAtaque = 25f;
    [SerializeField] private float rangoAtaque = 1f;
    [SerializeField] private float tiempoEntreAtaques = 0.5f;
    
    [Header("Punto de Ataque")]
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private Vector2 tamanoAreaAtaque = new Vector2(1.5f, 1f);
    
    [Header("Capas")]
    [SerializeField] private LayerMask capasEnemigos;
    
    private Animator animator;
    private float tiempoProximoAtaque = 0f;
    private bool estaAtacando = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Crear punto de ataque automáticamente si no existe
        if (puntoAtaque == null)
        {
            GameObject punto = new GameObject("PuntoAtaque");
            punto.transform.parent = transform;
            punto.transform.localPosition = new Vector3(0.5f, 0, 0);
            puntoAtaque = punto.transform;
        }
    }

    void Update()
    {
        // Detectar input de ataque con tecla J
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= tiempoProximoAtaque && !estaAtacando)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        // Marcar que está atacando
        estaAtacando = true;
        
        // Activar animación de ataque
        if (animator != null)
        {
            animator.SetTrigger("Ataque");
        }
        
        // Establecer el tiempo para el próximo ataque
        tiempoProximoAtaque = Time.time + tiempoEntreAtaques;
        
        // Detectar enemigos en rango
        Collider2D[] enemigosGolpeados = Physics2D.OverlapBoxAll(
            puntoAtaque.position, 
            tamanoAreaAtaque, 
            0f, 
            capasEnemigos
        );
        
        // Aplicar daño a cada enemigo detectado
        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Golpeaste a: " + enemigo.name);
            
            // Intentar aplicar daño si el enemigo tiene un componente de vida
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDanio(danioAtaque);
            }
        }
        
        // Resetear el estado de ataque después de un tiempo
        Invoke("FinalizarAtaque", tiempoEntreAtaques * 0.8f);
    }
    
    void FinalizarAtaque()
    {
        estaAtacando = false;
    }
    
    // Método público para verificar si está atacando (útil para otros scripts)
    public bool EstaAtacando()
    {
        return estaAtacando;
    }
    
    // Visualizar el área de ataque en el editor
    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(puntoAtaque.position, tamanoAreaAtaque);
        }
    }
}
