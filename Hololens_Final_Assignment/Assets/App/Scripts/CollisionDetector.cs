using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Hit by: {other.gameObject.name} {Time.time}" );
    }
}
