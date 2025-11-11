using UnityEngine;

public class SkullCollision : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private float danioContactoEnemigo = 10f;
    [SerializeField] private float fuerzaRetroceso = 5f;
    
    [Header("Capas")]
    [SerializeField] private LayerMask capaEnemigos;
    [SerializeField] private LayerMask capaColeccionables;
    
    private SkullHealth skullHealth;
    private Rigidbody2D rb;
    private bool puedeRecibirDanio = true;

    void Start()
    {
        skullHealth = GetComponent<SkullHealth>();
        rb = GetComponent<Rigidbody2D>();
        
        if (skullHealth == null)
        {
            Debug.LogError("SkullCollision: No se encontró el componente SkullHealth!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si colisionó con un enemigo
        if (EsEnemigo(collision.gameObject))
        {
            RecibirDanioDeEnemigo(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Daño continuo mientras está en contacto con el enemigo
        if (EsEnemigo(collision.gameObject) && puedeRecibirDanio)
        {
            RecibirDanioDeEnemigo(collision);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si es un coleccionable
        if (EsColeccionable(collision.gameObject))
        {
            RecogerObjeto(collision.gameObject);
        }
        
        // Verificar si es un proyectil enemigo
        if (collision.CompareTag("ProyectilEnemigo"))
        {
            RecibirDanioDeProyectil(collision.gameObject);
        }
    }

    void RecibirDanioDeEnemigo(Collision2D collision)
    {
        if (skullHealth != null && puedeRecibirDanio)
        {
            // Aplicar daño
            skullHealth.RecibirDanio(danioContactoEnemigo);
            
            // Aplicar retroceso
            AplicarRetroceso(collision.contacts[0].point);
            
            // Cooldown para evitar múltiples daños instantáneos
            puedeRecibirDanio = false;
            Invoke("ReactivarDanio", 0.5f);
        }
    }

    void RecibirDanioDeProyectil(GameObject proyectil)
    {
        if (skullHealth != null)
        {
            // Obtener el daño del proyectil si tiene un componente específico
            ProyectilEnemigo scriptProyectil = proyectil.GetComponent<ProyectilEnemigo>();
            float danio = scriptProyectil != null ? scriptProyectil.ObtenerDanio() : 15f;
            
            skullHealth.RecibirDanio(danio);
            
            // Destruir el proyectil
            Destroy(proyectil);
        }
    }

    void RecogerObjeto(GameObject objeto)
    {
        // Verificar el tipo de objeto
        if (objeto.CompareTag("Vida") || objeto.CompareTag("Pocion"))
        {
            // Curar al jugador
            ItemCuracion itemCuracion = objeto.GetComponent<ItemCuracion>();
            if (itemCuracion != null && skullHealth != null)
            {
                skullHealth.Curar(itemCuracion.ObtenerCantidadCuracion());
                Debug.Log("Recogiste una poción de vida!");
            }
            
            Destroy(objeto);
        }
        else if (objeto.CompareTag("Moneda"))
        {
            // Aquí puedes agregar lógica de monedas
            Debug.Log("Recogiste una moneda!");
            Destroy(objeto);
        }
        else if (objeto.CompareTag("PowerUp"))
        {
            // Aquí puedes agregar lógica de power-ups
            Debug.Log("Recogiste un power-up!");
            Destroy(objeto);
        }
    }

    void AplicarRetroceso(Vector2 puntoContacto)
    {
        if (rb != null)
        {
            // Calcular dirección del retroceso (alejarse del punto de contacto)
            Vector2 direccionRetroceso = (transform.position - (Vector3)puntoContacto).normalized;
            
            // Aplicar fuerza de retroceso
            rb.velocity = new Vector2(direccionRetroceso.x * fuerzaRetroceso, rb.velocity.y);
        }
    }

    void ReactivarDanio()
    {
        puedeRecibirDanio = true;
    }

    bool EsEnemigo(GameObject objeto)
    {
        // Verificar por tag
        if (objeto.CompareTag("Enemy") || objeto.CompareTag("Enemigo"))
        {
            return true;
        }
        
        // Verificar por layer
        int objetoLayer = objeto.layer;
        return ((1 << objetoLayer) & capaEnemigos) != 0;
    }

    bool EsColeccionable(GameObject objeto)
    {
        // Verificar por tag
        if (objeto.CompareTag("Vida") || objeto.CompareTag("Moneda") || 
            objeto.CompareTag("PowerUp") || objeto.CompareTag("Pocion"))
        {
            return true;
        }
        
        // Verificar por layer
        int objetoLayer = objeto.layer;
        return ((1 << objetoLayer) & capaColeccionables) != 0;
    }
}
