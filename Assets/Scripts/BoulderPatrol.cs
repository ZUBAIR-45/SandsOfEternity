using UnityEngine;

// Boulder point A se point B tak aage-peechhe ghoomta rehta hai.
[RequireComponent(typeof(Rigidbody2D))]
public class BoulderPatrol : MonoBehaviour
{
    public Vector2 pointA;
    public Vector2 pointB;
    public float speed = 2.5f;
    public Transform visual;

    Rigidbody2D rb;
    Vector2 target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // Start mai target set karte hain taake LevelBuilder pehle pointA/pointB bhar sake
    void Start()
    {
        target = pointB;
    }

    void FixedUpdate()
    {
        Vector2 pos = rb.position;
        Vector2 next = Vector2.MoveTowards(pos, target, speed * Time.fixedDeltaTime);
        Vector2 delta = next - pos;
        rb.MovePosition(next);

        if (visual != null)
        {
            float roll = delta.magnitude / 0.45f * Mathf.Rad2Deg;
            visual.Rotate(0f, 0f, -roll * Mathf.Sign(delta.x + delta.y));
        }

        if (Vector2.Distance(next, target) < 0.01f)
            target = (Vector2.Distance(target, pointB) < 0.01f) ? pointA : pointB;
    }
}
