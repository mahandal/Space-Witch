using UnityEngine;

public class Beacon : MonoBehaviour
{
    [Header("Beacon")]
    // Which beacon this is.
    // Note: Beacons are 1-indexed!
    public int index = 1; 

    // How fast beacons spin, multiplied by their index.
    private float baseSpinSpeed = 60f;
    
    [Header("Physics")]
    public Rigidbody2D rb2d;
    
    void Start()
    {
        // Get rigidbody for physics interactions
        rb2d = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        // Spin like a dying bee
        float spinSpeed = baseSpinSpeed * index;
        transform.Rotate(0, 0, -spinSpeed * Time.deltaTime);
    }
}