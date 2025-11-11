using UnityEngine;

public class ItemCuracion : MonoBehaviour
{
    [Header("Configuración del Item")]
    [SerializeField] private float cantidadCuracion = 25f;
    [SerializeField] private bool rotarItem = true;
    [SerializeField] private float velocidadRotacion = 50f;
    
    void Update()
    {
        // Rotar el item para hacerlo más visible
        if (rotarItem)
        {
            transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);
        }
    }
    
    public float ObtenerCantidadCuracion()
    {
        return cantidadCuracion;
    }
}
