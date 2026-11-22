using UnityEngine;

public enum CollectibleType { Gem, Heart }

// Sun Gem aur Heart of Eternity ko chhoone par collect karta hai.
public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    bool taken;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (taken) return;
        if (other.GetComponent<PlayerController>() == null) return;
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.state != GameState.Playing) return;

        taken = true;
        if (type == CollectibleType.Gem) gm.CollectGem();
        else gm.CollectHeart();
        Destroy(gameObject);
    }
}
