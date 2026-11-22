using UnityEngine;

// Spikes thodi der upar (khatarnak), thodi der neeche (safe). Neeche jaane se pehle halka laal warning.
[RequireComponent(typeof(Hazard))]
public class SpikeTrap : MonoBehaviour
{
    public float upTime = 1.5f;
    public float downTime = 2f;
    public float startOffset = 0f;
    public SpriteRenderer sprite;

    Hazard hazard;

    void Awake()
    {
        hazard = GetComponent<Hazard>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float cycle = upTime + downTime;
        float t = (Time.time + startOffset) % cycle;
        bool up = t < upTime;
        hazard.dangerous = up;

        if (sprite == null) return;
        if (up)
        {
            sprite.color = Color.white;
        }
        else
        {
            bool warning = (cycle - t) < 0.5f;
            sprite.color = warning ? new Color(1f, 0.45f, 0.45f, 0.85f) : new Color(1f, 1f, 1f, 0.3f);
        }
    }
}
