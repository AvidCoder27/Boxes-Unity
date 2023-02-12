using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction interactAction;
    private InputAction lookAction;
    private Transform cam;

    [SerializeField] private float sensitivity;
    [SerializeField] private float inputLagPeriod;
    [SerializeField] private float maxInteractionDistance;
    public bool AllowLooking
    {
        get
        {
            return allowLooking;
        }
        set
        {
            allowLooking = value;
            // if turning back on allowLooking, set rotation to current rotation to prevent snapping
            if (allowLooking)
            {
                rotation.x = transform.rotation.eulerAngles.y;
                rotation.y = -cam.rotation.eulerAngles.x;
            }
        }
    }
    private bool allowLooking;
    private Vector2 rotation;
    private Vector2 lastInputEvent; // last recieved non-zero input value
    private float inputLagTimer; // time since non-zero input

    private void Awake()
    {
        allowLooking = true;
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (allowLooking)
        {
            UpdateLooking();
        }
    }

    private void OnEnable()
    {
        inputLagTimer = 0;
        lastInputEvent = Vector2.zero;

        playerInput = GetComponentInChildren<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
        lookAction = playerInput.actions["Look"];

        interactAction.performed += InteractPerformed;
        Actions.OnGamePause += UnlockCursor;
        Actions.OnGameResume += LockCursor;
        LockCursor();
    }

    private void OnDisable()
    {
        interactAction.performed -= InteractPerformed;
        Actions.OnGamePause -= UnlockCursor;
        Actions.OnGameResume -= LockCursor;
        UnlockCursor();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void InteractPerformed(InputAction.CallbackContext ctx)
    {
        // cast a ray out of camera until it hits object within reach
        Ray ray = new(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance))
        {
            if (hit.collider.gameObject.TryGetComponent<Interactable>(out var hitInteractable))
            {
                hitInteractable.InteractedWith(transform);
            }
        }
    }

    private Vector2 GetLookInput()
    {
        inputLagTimer += Time.deltaTime;
        Vector2 input = lookAction.ReadValue<Vector2>();

        // this if statement should be true if the player is truly not providing input
        // if X is not 0 OR Y is not 0 OR the timer isn't met
        if (!Mathf.Approximately(0, input.x) || !Mathf.Approximately(0, input.y) || inputLagTimer >= inputLagPeriod)
        {
            lastInputEvent = input;
            inputLagTimer = 0;
        }
        return lastInputEvent;
    }

    private void UpdateLooking()
    {
        Vector2 wantedVelocity = GetLookInput() * sensitivity;

        rotation += wantedVelocity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -90, 90);

        SetLook(rotation);
    }

    public void SetLook(Vector2 rotation)
    {
        transform.localEulerAngles = new Vector3(0, rotation.x, 0);
        cam.localEulerAngles = new Vector3(-rotation.y, 0, 0);
    }

    public void LookAt(Vector3 position)
    {
        // get cam to look at it
        cam.LookAt(position);
        // take out the y component and put it into this transform
        transform.localEulerAngles = new Vector3(0, cam.localEulerAngles.y, 0);
        // Then, keep only the X component on the camera
        cam.localEulerAngles = new Vector3(cam.localEulerAngles.x, 0, 0);
    }
}
