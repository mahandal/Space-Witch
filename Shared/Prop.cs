using UnityEngine;

public class Prop : MonoBehaviour
{
    [Header("Automated Machinery")]
    public Animator animator;

    void Awake()
    {
        // Get animator.
        animator = GetComponent<Animator>();

        // Vary animation speed.
        animator.speed = Random.Range(0.5f, 1f);
    }
}
