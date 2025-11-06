using UnityEngine;

/// <summary>
/// Sistema de efectos visuales para el juego de caída libre
/// Rastros, partículas, indicadores visuales
/// Compatible con Unity 6.2
/// </summary>
public class VisualEffectsManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerController player;
    public PhysicsSimulation physics;

    [Header("Rastro (Trail)")]
    public bool enableTrail = true;
    public TrailRenderer trailRenderer;
    public Gradient trailColorGradient;
    public float trailTime = 2f;
    public float trailWidth = 0.2f;

    [Header("Partículas de Velocidad")]
    public ParticleSystem velocityParticles;
    public float particleEmissionMultiplier = 1f;

    [Header("Indicador de Velocidad")]
    public GameObject velocityArrow;
    public float arrowScaleMultiplier = 0.1f;
    public Color slowColor = Color.green;
    public Color fastColor = Color.red;
    public float maxVelocityForColor = 50f;

    [Header("Efectos de Impacto")]
    public ParticleSystem impactEffect;
    public AudioSource audioSource;
    public AudioClip impactSound;

    [Header("Indicadores de Altura")]
    public GameObject heightMarkerPrefab;
    public int numberOfMarkers = 10;
    public Color markerColor = new Color(1f, 1f, 1f, 0.3f);

    private GameObject[] heightMarkers;
    private float lastVelocity = 0f;

    void Start()
    {
        SetupTrail();
        SetupHeightMarkers();
        SetupAudioSource();
    }

    void Update()
    {
        if (player != null && physics != null)
        {
            UpdateTrail();
            UpdateVelocityParticles();
            UpdateVelocityArrow();
        }
    }

    /// <summary>
    /// Configura el trail renderer
    /// </summary>
    private void SetupTrail()
    {
        if (trailRenderer == null && player != null)
        {
            trailRenderer = player.gameObject.AddComponent<TrailRenderer>();
        }

        if (trailRenderer != null)
        {
            trailRenderer.time = trailTime;
            trailRenderer.startWidth = trailWidth;
            trailRenderer.endWidth = trailWidth * 0.1f;
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            
            if (trailColorGradient == null)
            {
                trailColorGradient = new Gradient();
                trailColorGradient.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(Color.cyan, 0f),
                        new GradientColorKey(Color.blue, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
            }
            
            trailRenderer.colorGradient = trailColorGradient;
            trailRenderer.enabled = enableTrail;
        }
    }

    /// <summary>
    /// Actualiza el trail según velocidad
    /// </summary>
    private void UpdateTrail()
    {
        if (trailRenderer == null) return;

        trailRenderer.enabled = enableTrail && physics.velocity > 0.1f;

        // Cambiar color del trail según velocidad
        float velocityRatio = Mathf.Clamp01(physics.velocity / maxVelocityForColor);
        Color trailColor = Color.Lerp(slowColor, fastColor, velocityRatio);
        
        Gradient newGradient = new Gradient();
        newGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(trailColor, 0f),
                new GradientColorKey(trailColor * 0.5f, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trailRenderer.colorGradient = newGradient;
    }

    /// <summary>
    /// Actualiza las partículas de velocidad
    /// </summary>
    private void UpdateVelocityParticles()
    {
        if (velocityParticles == null) return;

        var emission = velocityParticles.emission;
        float targetRate = physics.velocity * particleEmissionMultiplier;
        emission.rateOverTime = targetRate;

        // Activar/desactivar según velocidad
        if (physics.velocity < 0.5f && velocityParticles.isPlaying)
        {
            velocityParticles.Stop();
        }
        else if (physics.velocity >= 0.5f && !velocityParticles.isPlaying)
        {
            velocityParticles.Play();
        }
    }

    /// <summary>
    /// Actualiza la flecha indicadora de velocidad
    /// </summary>
    private void UpdateVelocityArrow()
    {
        if (velocityArrow == null) return;

        // Posicionar al lado del jugador
        velocityArrow.transform.position = player.transform.position + Vector3.right * 1.5f;

        // Escalar según velocidad
        float scale = physics.velocity * arrowScaleMultiplier;
        velocityArrow.transform.localScale = new Vector3(0.3f, scale, 0.3f);

        // Colorear según velocidad
        float velocityRatio = Mathf.Clamp01(physics.velocity / maxVelocityForColor);
        Color arrowColor = Color.Lerp(slowColor, fastColor, velocityRatio);
        
        Renderer renderer = velocityArrow.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = arrowColor;
        }

        // Mostrar solo cuando hay movimiento
        velocityArrow.SetActive(physics.velocity > 0.1f);
    }

    /// <summary>
    /// Configura los marcadores de altura
    /// </summary>
    private void SetupHeightMarkers()
    {
        if (heightMarkerPrefab == null) return;

        heightMarkers = new GameObject[numberOfMarkers];

        for (int i = 0; i < numberOfMarkers; i++)
        {
            float height = (i + 1) * (100f / (numberOfMarkers + 1));
            
            GameObject marker = Instantiate(heightMarkerPrefab, transform);
            marker.name = $"HeightMarker_{height}m";
            marker.transform.position = new Vector3(0, height, 0);
            
            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = markerColor;
            }

            heightMarkers[i] = marker;
        }
    }

    /// <summary>
    /// Configura el audio source
    /// </summary>
    private void SetupAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    /// <summary>
    /// Reproduce el efecto de impacto
    /// </summary>
    public void PlayImpactEffect(Vector3 position)
    {
        // Partículas
        if (impactEffect != null)
        {
            impactEffect.transform.position = position;
            impactEffect.Play();
        }

        // Sonido
        if (audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        Debug.Log($"Efecto de impacto en {position}");
    }

    /// <summary>
    /// Activa/desactiva el trail
    /// </summary>
    public void SetTrailEnabled(bool enabled)
    {
        enableTrail = enabled;
        if (trailRenderer != null)
        {
            trailRenderer.enabled = enabled;
        }
    }

    /// <summary>
    /// Limpia el trail actual
    /// </summary>
    public void ClearTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
    }

    /// <summary>
    /// Crea un marcador temporal en una posición
    /// </summary>
    public void CreateTemporaryMarker(Vector3 position, Color color, float lifetime = 2f)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.3f;
        
        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            renderer.material = mat;
        }

        Destroy(marker, lifetime);
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos()
    {
        // Dibujar líneas de altura cada 10m
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        
        for (int i = 0; i <= 10; i++)
        {
            float height = i * 10f;
            Vector3 start = new Vector3(-20f, height, 0);
            Vector3 end = new Vector3(20f, height, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
