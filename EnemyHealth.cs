using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;
    
    [Header("Efectos")]
    [SerializeField] private bool mostrarDanioEnConsola = true;
    
    private Animator animator;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        
        if (mostrarDanioEnConsola)
        {
            Debug.Log(gameObject.name + " recibió " + cantidad + " de daño. Vida restante: " + vidaActual);
        }
        
        // Activar animación de daño si existe
        if (animator != null)
        {
            animator.SetTrigger("Danio");
        }
        
        // Verificar si murió
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    void Morir()
    {
        Debug.Log(gameObject.name + " ha sido derrotado!");
        
        // Activar animación de muerte si existe
        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }
        
        // Destruir el objeto después de un tiempo (para dar tiempo a la animación)
        Destroy(gameObject, 1f);
    }
    
    // Método público para obtener la vida actual
    public float ObtenerVidaActual()
    {
        return vidaActual;
    }
    
    // Método público para curar
    public void Curar(float cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
    }
}
