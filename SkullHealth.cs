using UnityEngine;
using UnityEngine.UI;

public class SkullHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;
    
    [Header("UI (Opcional)")]
    [SerializeField] private Slider barraVida;
    [SerializeField] private Text textoVida;
    
    [Header("Efectos")]
    [SerializeField] private bool mostrarDanioEnConsola = true;
    [SerializeField] private float tiempoInvulnerabilidad = 1f;
    private float tiempoUltimoDanio;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool estaMuerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ActualizarUI();
    }

    void Update()
    {
        // Efecto de parpadeo durante invulnerabilidad
        if (Time.time - tiempoUltimoDanio < tiempoInvulnerabilidad)
        {
            float alpha = Mathf.PingPong(Time.time * 10, 1);
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
        else if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    public void RecibirDanio(float cantidad)
    {
        if (estaMuerto) return;
        
        // Verificar invulnerabilidad
        if (Time.time - tiempoUltimoDanio < tiempoInvulnerabilidad)
        {
            return;
        }
        
        vidaActual -= cantidad;
        tiempoUltimoDanio = Time.time;
        
        if (mostrarDanioEnConsola)
        {
            Debug.Log("Skull recibió " + cantidad + " de daño. Vida restante: " + vidaActual);
        }
        
        // Activar animación de daño si existe
        if (animator != null)
        {
            animator.SetTrigger("Danio");
        }
        
        // Actualizar UI
        ActualizarUI();
        
        // Verificar si murió
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    public void Curar(float cantidad)
    {
        if (estaMuerto) return;
        
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
        
        Debug.Log("Skull curado: +" + cantidad + ". Vida actual: " + vidaActual);
        ActualizarUI();
    }
    
    void Morir()
    {
        if (estaMuerto) return;
        
        estaMuerto = true;
        Debug.Log("Skull ha muerto!");
        
        // Activar animación de muerte si existe
        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }
        
        // Desactivar controles
        SkullMovement movimiento = GetComponent<SkullMovement>();
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }
        
        SkullAttack ataque = GetComponent<SkullAttack>();
        if (ataque != null)
        {
            ataque.enabled = false;
        }
        
        // Aquí puedes agregar lógica de Game Over
        Invoke("GameOver", 2f);
    }
    
    void GameOver()
    {
        Debug.Log("GAME OVER");
        // Aquí puedes cargar una escena de Game Over o reiniciar el nivel
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ActualizarUI()
    {
        // Actualizar barra de vida si existe
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
        }
        
        // Actualizar texto de vida si existe
        if (textoVida != null)
        {
            textoVida.text = vidaActual.ToString("F0") + " / " + vidaMaxima.ToString("F0");
        }
    }
    
    // Métodos públicos para obtener información
    public float ObtenerVidaActual()
    {
        return vidaActual;
    }
    
    public float ObtenerVidaMaxima()
    {
        return vidaMaxima;
    }
    
    public bool EstaMuerto()
    {
        return estaMuerto;
    }
    
    public float ObtenerPorcentajeVida()
    {
        return (vidaActual / vidaMaxima) * 100f;
    }
}
