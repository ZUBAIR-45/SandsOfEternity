using UnityEngine;

// WASD / Arrow keys se explorer ko chalata hai (top-down).
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.state != GameState.Playing)
        {
            input = Vector2.zero;
            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);

        if (sr != null)
        {
            // orange dot hamesha chalne ki taraf dikhe
            if (input.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
                sr.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            // hit lagne ke baad blink
            if (gm != null && gm.Invulnerable) sr.enabled = Mathf.FloorToInt(Time.time * 12f) % 2 == 0;
            else sr.enabled = true;
        }
    }

    void FixedUpdate()
    {
        Vector2 v = input * moveSpeed;
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }
}
