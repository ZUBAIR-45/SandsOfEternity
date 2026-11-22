using System.Collections;
using UnityEngine;

// 3 gems milte hi golden door khulta hai aur heart room se andhera hat jata hai.
public class DoorController : MonoBehaviour
{
    public GameObject shadowCover;
    bool opening;

    void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnDoorOpened += Open;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnDoorOpened -= Open;
    }

    void Open()
    {
        if (opening) return;
        opening = true;
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (shadowCover != null) shadowCover.SetActive(false);

        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.6f;
            transform.localScale = Vector3.Lerp(start, new Vector3(start.x, 0f, start.z), t);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
