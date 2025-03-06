
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class SekeltonKnifStateMachine : MonoBehaviour
{
    private ISekeltonKnif currentState;
    private Animator animator;

    // Define available states
    public readonly KnifSekelton_Idle idleState = new KnifSekelton_Idle();
    public readonly KnifSekelton_Move moveState = new KnifSekelton_Move();
    public readonly KnifSekelton_Kill killState = new KnifSekelton_Kill();

    public float _moveSpeed;

    private Rigidbody2D rb;

    public bool IsMove;
    public bool IsNearPlayer;

    public float totalHealth = 100;
    public float healthPercent = 100;

    [SerializeField] private Image _heakthIndicator;

    [SerializeField] private IKManager2D _manager;

    [SerializeField] private List<Rigidbody2D> bodyParts;

    [SerializeField] private Transform Platform;

    [SerializeField] private float PlatformOffset = 1.11f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        ChangeState(idleState); // Set initial state
        healthPercent = totalHealth;

       
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
    }

    public void SetDamage(float damage)
    {
        healthPercent -= damage;

        if (healthPercent <0)
        {
            healthPercent = 0;
            _manager.enabled = false;
        }

        if (healthPercent > 0)
        {
            _heakthIndicator.fillAmount = healthPercent / totalHealth;
        }
        else {
            _heakthIndicator.fillAmount = 0;
        }
    }

    public void ChangeState(ISekeltonKnif newState)
    {
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        currentState.EnterState(this);
    }


    public void ActivateRegDol()
    {
        foreach (Rigidbody2D rb in bodyParts)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    private void FixedUpdate()
    {
        if (!IsNearPlayer && IsMove)
        {
            rb.linearVelocity = new Vector2(_moveSpeed, rb.linearVelocity.y);
        }

        Platform.position = new Vector3(transform.position.x+ PlatformOffset, 0, 0);
    }

    public void SetVeleocityX(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }
}


public interface ISekeltonKnif
{
    void EnterState(SekeltonKnifStateMachine character);
    void UpdateState(SekeltonKnifStateMachine character);
    void ExitState(SekeltonKnifStateMachine character);
}