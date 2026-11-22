using UnityEngine;

// Boulder aur Spike par lagta hai: player ko chhua to 1 life gayi.
public class Hazard : MonoBehaviour
{
    public bool dangerous = true;

    void OnTriggerEnter2D(Collider2D other) { Check(other); }
    void OnTriggerStay2D(Collider2D other) { Check(other); }

    void Check(Collider2D other)
    {
        if (!dangerous) return;
        if (other.GetComponent<PlayerController>() == null) return;
        if (GameManager.Instance != null) GameManager.Instance.TakeHit();
    }
}
