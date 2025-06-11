using UnityEngine;

public class TextScrawl : MonoBehaviour
{
    public float moveSpeed = 21f;

    void FixedUpdate()
    {
        // Move on up!
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
