using UnityEngine;

public class MoveScript : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody2D rb;

    public bool IsMove;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        if (IsMove)
        {
            rb.linearVelocity = new Vector2(_moveSpeed, rb.linearVelocity.y);
        }
    }
}
