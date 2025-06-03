using UnityEngine;

public class Beacon : MonoBehaviour
{
    [Header("Beacon")]
    public int index = 0; // Which beacon in the sequence (0, 1, 2...)
    public float spinSpeed = 720f;
    
    [Header("Physics")]
    public Rigidbody2D rb2d;
    
    void Start()
    {
        // Get rigidbody for physics interactions
        rb2d = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        // Always spin like a dying bee
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}