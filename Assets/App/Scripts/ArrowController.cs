
using Unity.Mathematics;
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

    public GameObject floatingTextPrefab;
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
            forceMultiplier = rb.linearVelocityX;
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


    void DitachHead(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            StopArrow(null);
            Destroy(gameObject);
        }


   

        SekeltonKnifStateMachine skeltonWalkenemy = collision.GetComponentInParent<SekeltonKnifStateMachine>();

        if (skeltonWalkenemy != null)
        {
           
            
           

            float damage = 0;
           
            switch (collision.gameObject.tag)
            {
                case "Head":
                   
                    damage = forceMultiplier * 8;
                    break;
                case "Body":
                    damage = forceMultiplier * 3;
                    break;
                case "Leg":
                    damage = forceMultiplier * 1.5f; 
                    break;
              
                default: break;
            }

            ShowFloatingText(collision.transform.position, CalculateDamage(), Color.red);

            float CalculateDamage()
            {
                float damageRecived = skeltonWalkenemy.healthPercent - damage;

                if (damageRecived < 0)
                {
                    damageRecived = skeltonWalkenemy.healthPercent;
                }
                else
                {
                    damageRecived = damage;
                }
                

                return Mathf.Round(damageRecived);
            }

            skeltonWalkenemy.SetDamage(damage);
            
            if (skeltonWalkenemy.healthPercent <= 0)
            {
                skeltonWalkenemy.ActivateRegDol();
                
                Animator animator = collision.GetComponentInParent<Animator>();

                EnemyController enemyController = collision.GetComponentInParent<EnemyController>();

                if (enemyController !=  null)
                {
                    enemyController.DestroyPlatform();
                }


                if (animator != null)
                {
                    animator.enabled = false;
                }

                    skeltonWalkenemy.enabled = false;
                if (collision.CompareTag("Head")) // Ensure the head object has the "Head" tag
                {
                  
                    Rigidbody2D headRb = collision.GetComponent<Rigidbody2D>();
                    //HingeJoint2D hinge = other.GetComponent<HingeJoint2D>();

                    if (headRb != null)
                    {
                        // Apply force based on arrow's velocity
                        Vector2 impactForce = rb.linearVelocity.normalized * forceMultiplier ;
                        headRb.AddForce(impactForce, ForceMode2D.Impulse);

                        // Add torque for rotation effect
                        //headRb.AddTorque(Random.Range(-0.1f, 0.1f), ForceMode2D.Impulse);
                    }

                    //if (hinge != null)
                    //{
                    //    hinge.useConnectedAnchor
                    //        = false; 
                    //}
                }

                if (collision.CompareTag("Body"))
                {
                    Rigidbody2D bodyRb = collision.GetComponent<Rigidbody2D>();
                    if (bodyRb != null)
                    {
                        Vector2 impactForce = rb.linearVelocity.normalized * forceMultiplier ;
                        bodyRb.AddForce(impactForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

    }
    void ShowFloatingText(Vector3 position, float text, Color color)
    {
        if (text <=0)
        {
            return;
        }
        if (floatingTextPrefab)
        {
            GameObject floatingText = Instantiate(floatingTextPrefab, position, Quaternion.identity);
            floatingText.GetComponent<FloatingText>().SetText($"-{text}", color);
        }
    }
}
