using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.IK;

public class ArrowInputController : MonoBehaviour
{
    public Transform arrow =  null; // Assign arrow GameObject
    public Transform leftHand;
    public float maxDragDistance = 2f; // Max drag distance before firing
    public float arrowForceMultiplier = 5f; // Adjust force applied
    public InputActionReference dragAction; // Assign Input Action in Inspector

    private Vector2 startPos;
    private Vector2 endPos;
    private bool isDragging = false;

    private Vector2 dragDirection;
    private float dragDistance;

    [SerializeField] Transform RightHandTarget;

    [SerializeField] LimbSolver2D LimbSolver;


    public Transform leftPoint, rightPoint, arrowPoint;
    [SerializeField] private LineRenderer lineRenderer;


    [SerializeField] private float arrowOffsetX = 1.480001f;
    [SerializeField] private  Transform ArrowPrefab;

    [SerializeField] private GameObject _point;
    GameObject[] _points;
    [SerializeField] private int _numberOfPoints;
    [SerializeField] private float _spaceBetweenPoints;
    [SerializeField] private Transform Bow;


    [SerializeField] private Transform TrajectoryPosition;
    void Start()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3;

        Application.targetFrameRate = 120; // Set to 60 FPS
        QualitySettings.vSyncCount = 0;   // Disable VSync (so FPS is not capped by monitor refresh rate)

        ArrowStart();
    }

    private void OnEnable()
    {
        dragAction.action.Enable();
        dragAction.action.started += OnDrag;
        dragAction.action.canceled += OnDrag;
    }

    private void OnDisable()
    {
        dragAction.action.started -= OnDrag;
        dragAction.action.canceled -= OnDrag;
        dragAction.action.Disable();
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 currentPos = GetPointerPosition();
            Vector2 worldCurrentPos = Camera.main.ScreenToWorldPoint(currentPos);
            Vector2 rawDirection = worldCurrentPos - startPos;

            
            // Allow only right-side dragging
            if (rawDirection.x < 0)
            {
                endPos = worldCurrentPos;
                dragDistance = Mathf.Clamp(rawDirection.magnitude, 0, maxDragDistance);
                dragDirection = rawDirection.normalized;
                RotateObject();            }
            else
            {
                endPos = startPos; // Reset if dragging left
                dragDirection = Vector2.zero;
                dragDistance = 0;
            }
            PullHand();
        }
        bowRoap();

        ResetTrajectory();
    }


    void bowRoap()
    {
        lineRenderer.SetPosition(0, leftPoint.position);
        lineRenderer.SetPosition(1, arrowPoint.position); // Middle part follows arrow
        lineRenderer.SetPosition(2, rightPoint.position);
    }
    public void OnDrag(InputAction.CallbackContext context)
    {
      
        if (context.started) // Pressed down (Start drag)
        {
            Vector2 inputPosition = GetPointerPosition();
            startPos = Camera.main.ScreenToWorldPoint(inputPosition);
            isDragging = true;

            if (arrow == null)
            {
                GetAnArrow();
            }
        }
        else if (context.canceled) // Released (End drag)
        {
            
            isDragging = false;
            FireArrow();
        }
    }

    void GetAnArrow()
    {
        //arrow = 

        arrow = Instantiate(ArrowPrefab, Vector3.zero, RightHandTarget.rotation);

        arrow.transform.SetParent(RightHandTarget);
        arrow.transform.localPosition = new Vector3(arrowOffsetX, 0, 0);
    }



    private void FireArrow()
    {
        ResetHandPos();
        if (arrow != null && dragDistance > 0)
        {
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            ArrowController arrowController = rb.GetComponent<ArrowController>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (rb != null)
            {

                //rb.linearVelocity = -dragDirection * dragDistance * arrowForceMultiplier * Time.deltaTime;// Apply force

                rb.AddForce(-dragDirection * dragDistance * arrowForceMultiplier, ForceMode2D.Impulse);

                arrowController.FireArrow();

            }

            arrow.transform.SetParent(null);
            arrow = null;
        }
    }

    private void RotateObject()
    {
        float angle = Mathf.Atan2(dragDirection.y,dragDirection.x) * Mathf.Rad2Deg;

        leftHand.eulerAngles = new Vector3(0, 0, angle);

        if (angle > 0 && angle < 160)
        {
            LimbSolver.flip = false;
        }
        else
        {
            LimbSolver.flip = true;
        }


       
    }

    private void ResetHandPos()
    {
        Vector3 pos = RightHandTarget.localPosition;
        pos.x = 0;
        RightHandTarget.localPosition = pos;
    }

    private void PullHand()
    {
        Vector3 pos = RightHandTarget.localPosition;
        pos.x = dragDistance <1.5 ? dragDistance * -1 : -1.5f;
        RightHandTarget.localPosition = pos;


        Vector3 TrajectoryPos = TrajectoryPosition.localPosition;
        TrajectoryPos.x =arrowOffsetX;
        TrajectoryPosition.localPosition = TrajectoryPos;

       
    }

    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }


    #region ArrowTragectory
  
    private void ArrowStart()
    {
        _points = new GameObject[_numberOfPoints];
        for (int i = 0; i < _numberOfPoints; i++)
        {

            _points[i] = Instantiate(_point, Bow.position, Quaternion.identity);

            _points[i].gameObject.transform.SetParent(Bow);

        }

    }

    void ResetTrajectory()
    {

        if (arrow == null) { 
            return; 
        }


        Vector2 startPosition = Bow.position;
        Vector2 startVelocity = Bow.right * dragDistance * arrowForceMultiplier;

        float gravity = Physics2D.gravity.y; // Apply Rigidbody gravity scale

        for (int i = 0; i < _numberOfPoints; i++)
        {
            float time = i * _spaceBetweenPoints; // Time step for each point

            // Calculate position using kinematic equations
            float x = startPosition.x + startVelocity.x * time;
            float y = startPosition.y + (startVelocity.y * time) + (0.5f * gravity * time * time);

            _points[i].transform.position = new Vector2(x, y);
        }
    }


    #endregion
}
