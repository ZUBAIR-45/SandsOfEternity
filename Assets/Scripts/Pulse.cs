using UnityEngine;

// Gems / heart ki glow halki halki chamakti rehti hai.
public class Pulse : MonoBehaviour
{
    public float speed = 2f;
    public float minScale = 0.85f;
    public float maxScale = 1.15f;
    public float minAlpha = 0.35f;
    public float maxAlpha = 0.8f;

    SpriteRenderer sr;
    Vector3 baseScale;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        float k = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        transform.localScale = baseScale * Mathf.Lerp(minScale, maxScale, k);
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, k);
            sr.color = c;
        }
    }
}
