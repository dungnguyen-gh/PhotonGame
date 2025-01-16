using UnityEngine;

public class DestroyFeed : MonoBehaviour
{
    public float DestroyTime = 4f;

    private void OnEnable()
    {
        Destroy(gameObject, DestroyTime);
    }
}
