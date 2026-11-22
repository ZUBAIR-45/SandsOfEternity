using UnityEngine;

// Entrance / Exit tile. Heart ke saath yahan pahunche to VICTORY.
public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;
        if (GameManager.Instance != null) GameManager.Instance.ReachExit();
    }
}
