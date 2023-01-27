using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCharacterController : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;

    Rigidbody rigidBody;
    new CapsuleCollider collider;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] [Range(5f, 60f)] float slopeLimit = 45f; // degrees
    // meters per second
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] Vector2 sensitivity;
    [SerializeField] bool isGrounded;
    Vector3 walkInput;
    bool jumpInput;
    float lastVelocityMagnitude;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        GetInput();
        CheckGrounded();
        ProcessActions();
        lastVelocityMagnitude = rigidBody.velocity.magnitude;
    }

    private void GetInput()
    {
        Vector2 rawInput = moveAction.ReadValue<Vector2>();
        walkInput = new Vector3(rawInput.x, 0f, rawInput.y);
        jumpInput = jumpAction.inProgress;
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        float capsuleHeight = Mathf.Max(collider.radius * 2f, collider.height);
        float capsuleRadius = transform.TransformVector(collider.radius, 0f, 0f).magnitude;
        Vector3 capsuleBottom = transform.TransformPoint(collider.center - Vector3.up * capsuleHeight / 2f);
        Ray ray = new(capsuleBottom + transform.up * .01f, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, capsuleRadius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            float maxDist = capsuleRadius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - capsuleRadius + .02f;
            if (normalAngle < slopeLimit)
            {
                if (hit.distance < maxDist)
                    isGrounded = true;
            }
        }
    }

    private void ProcessActions()
    {
        // Process Movement/Jumping
        if (isGrounded)
        {
            // Reset the velocity
            rigidBody.velocity = Vector3.zero;
            // Check if trying to jump
            if (jumpInput)
            {
                // Apply an upward velocity to jump
                rigidBody.velocity += Vector3.up * jumpSpeed;
            }

            // Apply a horizontal velocity based on player input
            rigidBody.velocity += transform.rotation * walkInput * moveSpeed;
        }
        else if (jumpInput)
        {
            // shoots the player off of the surface they are on if they are stuck
            if (Mathf.Abs(rigidBody.velocity.magnitude) < 0.01f && Mathf.Abs(lastVelocityMagnitude) < 0.01f)
            {
                float capsuleHeight = Mathf.Max(collider.radius * 2f, collider.height);
                Vector3 capsuleBottom = transform.TransformPoint(collider.center - Vector3.up * capsuleHeight / 2f);
                Ray ray = new(capsuleBottom + transform.up * .01f, -transform.up);
                Vector3 normal = GetSlightlyRandomUpVector();
                if (Physics.Raycast(ray, out RaycastHit hit, 1f, ~playerLayer))
                    normal = hit.normal;
                rigidBody.AddForce(normal * jumpSpeed, ForceMode.VelocityChange);

            }
        }
    }
    
    private Vector3 GetSlightlyRandomUpVector()
    {
        Vector3 randomDirection = new Vector3(Random.value, Random.value, Random.value);
        return (Vector3.up * 5f + randomDirection).normalized;
    }
}

//private bool CheckGrounded()
//{
//    isGrounded = false;
//    // bottomPosition is a little bit above the exact bottom of the cylinder
//    float aboutZero = 0.01f;
//    float cylinderRadius = 0.5f;
//    Vector3 bottomPosition = transform.position + Vector3.down * (1 - aboutZero);
//    // shoot a ray straight down to check the angle of the surface
//    Ray ray = new(bottomPosition, Vector3.down);
//    if (Physics.Raycast(ray, out RaycastHit hit, 10f, ~player))
//    {
//        float normalAngle = Vector3.Angle(hit.normal, transform.up);

//        // if tangent(normalAngle) == opposite leg (hit.distance) / adjacent (radius of cylinder)
//        float differenceBetweenTanAndDivision = Mathf.Abs(
//            Mathf.Tan(normalAngle * Mathf.Deg2Rad)
//            - (hit.distance - aboutZero) / cylinderRadius
//        );
//        if (normalAngle < slopeLimit && differenceBetweenTanAndDivision < aboutZero)
//        {
//            return true;
//        }
//    }
//    // if it didn't return here, then it checks 4 rays: backward, forward, left, right
//    Vector3 belowBottom = transform.position + Vector3.down * 0.05f;
//    Ray[] sidewaysRays = new Ray[] {
//        new Ray(bottomPosition, transform.rotation * Vector3.back),
//        new Ray(bottomPosition, transform.rotation * Vector3.forward),
//        new Ray(bottomPosition, transform.rotation * Vector3.left),
//        new Ray(bottomPosition, transform.rotation * Vector3.right)
//    }; 
//    for (int i = 0; i < 4; i++)
//    {
//        if (Physics.Raycast(sidewaysRays[i], out RaycastHit sideHit, cylinderRadius + 0.1f, ~player))
//        {
//            return true;
//        }
//        Debug.Log("failed to hit");
//    }

//    return false;
//}