
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasCollided = false;
    public bool IsFired = false;

    [SerializeField] private SpriteRenderer BackSprite;
    [SerializeField] private SpriteRenderer FrontSprite;

    [SerializeField] float forceMultiplier;

    private Collider2D arrowCollider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = rb.GetComponent<Collider2D>();
        arrowCollider.enabled = false;
    }

    private void Update()
    {
        if (IsFired && !hasCollided)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }

    public void FireArrow()
    {
        IsFired = true;
        SetSortingLayerToEnemy(FrontSprite);
        SetSortingLayerToEnemy(BackSprite);
        arrowCollider.enabled = true;
    }

    void SetSortingLayerToEnemy(SpriteRenderer sr)
    {
        sr.sortingLayerName = "Enemy";
        sr.sortingOrder = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (!hasCollided)
        {
            DitachHead(collision);
        }

        StopArrow(collision.transform);
    }

    void StopArrow(Transform parent)
    {
        if (!hasCollided)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;
            rb.linearVelocity = Vector3.zero;
            transform.parent = parent;
            hasCollided = true;
            IsFired = false;
        }
    }


    void DitachHead(Collider2D other)
    {
        if (other.CompareTag("End"))
        {
            StopArrow(null);
            Destroy(gameObject);
        }

        SekeltonKnifStateMachine skeltonWalkenemy = other.GetComponentInParent<SekeltonKnifStateMachine>();

        if (skeltonWalkenemy != null)
        {
            float damage = 0;
           
            switch (other.gameObject.tag)
            {
                case "Head":
                    damage = 150;
                    break;
                case "Body":
                    damage = 60;
                    break;
                case "Leg":
                    damage = 35; 
                    break;
              
                default: break;
            }
            skeltonWalkenemy.SetDamage(damage);
            
            if (skeltonWalkenemy.healthPercent <= 0)
            {
                skeltonWalkenemy.ActivateRegDol();
                
                Animator animator = other.GetComponentInParent<Animator>();

                EnemyController enemyController = other.GetComponentInParent<EnemyController>();

                if (enemyController !=  null)
                {
                    enemyController.DestroyPlatform();
                }


                if (animator != null)
                {
                    animator.enabled = false;
                }

                    skeltonWalkenemy.enabled = false;
                if (other.CompareTag("Head")) // Ensure the head object has the "Head" tag
                {
                  
                    Rigidbody2D headRb = other.GetComponent<Rigidbody2D>();
                    HingeJoint2D hinge = other.GetComponent<HingeJoint2D>();

                    if (headRb != null)
                    {
                        // Apply force based on arrow's velocity
                        Vector2 impactForce = rb.linearVelocity.normalized * forceMultiplier;
                        headRb.AddForce(impactForce, ForceMode2D.Impulse);

                        // Add torque for rotation effect
                        //headRb.AddTorque(Random.Range(-0.1f, 0.1f), ForceMode2D.Impulse);
                    }

                    if (hinge != null)
                    {
                        hinge.useConnectedAnchor
                            = false; 
                    }
                }

                if (other.CompareTag("Body"))
                {
                    Rigidbody2D bodyRb = other.GetComponent<Rigidbody2D>();
                    if (bodyRb != null)
                    {
                        Vector2 impactForce = rb.linearVelocity.normalized * forceMultiplier;
                        bodyRb.AddForce(impactForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

    }

}
