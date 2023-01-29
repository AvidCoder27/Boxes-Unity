using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerMovement: MonoBehaviour
{
    enum Move { None, Left, Right }

    PlayerInput playerInput;
    InputAction moveLeftAction;
    InputAction moveRightAction;
    InputAction selectTopAction;
    InputAction selectBottomAction;
    InputAction interactAction;
    InputAction climbAction;

    LevelHandler levelHandler;
    Level level;
    AudioSource moveSound;

    [SerializeField] float moveTime; // 0.5f
    [SerializeField] float distanceFromCircle; // 4f
    Move queuedMove;
    bool moving;

    float moveTimeElapsed;
    Quaternion orientPreMove;
    Quaternion orientSetpoint;
    Vector3 posPreMove;
    Vector3 posSetpoint;
    int column;
    int floor;

    private void Awake()
    {
        levelHandler = LevelHandler.GetInstance();
        playerInput = GetComponent<PlayerInput>();

        moveLeftAction = playerInput.actions["MoveLeft"];
        moveRightAction = playerInput.actions["MoveRight"];
        selectTopAction = playerInput.actions["SelectTop"];
        selectBottomAction = playerInput.actions["SelectBottom"];
        interactAction = playerInput.actions["Interact"];
        climbAction = playerInput.actions["Climb"];
    }

    private void OnEnable()
    {
        moveLeftAction.performed += QueueMoveLeft;
        moveRightAction.performed += QueueMoveRight;
        selectTopAction.performed += TopAction_performed;
        selectBottomAction.performed += BottomAction_performed;
        interactAction.performed += InteractAction_performed;
        climbAction.performed += ClimbAction_performed;
        Actions.OnGameEnd += HandleGameEnd;
    }

    private void OnDisable()
    {
        moveLeftAction.performed -= QueueMoveLeft;
        moveRightAction.performed -= QueueMoveRight;
        selectTopAction.performed -= TopAction_performed;
        selectBottomAction.performed -= BottomAction_performed;
        interactAction.performed -= InteractAction_performed;
        climbAction.performed -= ClimbAction_performed;
        Actions.OnGameEnd -= HandleGameEnd;
    }

    void Start()
    {
        playerInput.DeactivateInput();
        level = levelHandler.GetCurrentLevel();
        moveSound = GetComponent<AudioSource>();
        moveTimeElapsed = 0f;

        floor = 0;
        column = UnityEngine.Random.Range(0, level.NumberOfColumns);
        StartMoving(moveInstantly: true);
    }

    private void InteractAction_performed(InputAction.CallbackContext ctx)
    {
        if (playerInput.currentControlScheme == "Keyboard")
        {
            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
                if (hit.transform.TryGetComponent(out Interactable interactable))
                    interactable.InteractedWith(transform);
        }
    }

    private void ClimbAction_performed(InputAction.CallbackContext obj)
    {
        Actions.OnTryClimbLadder?.Invoke(floor, column, transform);
    }

    void HandleGameEnd(Actions.GameEndState endState)
    {
        queuedMove = Move.None;
        playerInput.DeactivateInput();
    }

    public void ChangeFloor(int delta)
    {
        floor += delta;
        queuedMove = Move.None;
        StartMoving(moveInstantly: true);
    }

    void BottomAction_performed(InputAction.CallbackContext ctx)
    {
        if (!moving) Actions.OnTryInteractBox.Invoke(new int3(floor, column,0));
    }
    void TopAction_performed(InputAction.CallbackContext ctx)
    {
        if (!moving) Actions.OnTryInteractBox.Invoke(new int3(floor, column, 1));
    }
    void QueueMoveLeft(InputAction.CallbackContext ctx) => queuedMove = Move.Left;
    void QueueMoveRight(InputAction.CallbackContext ctx) => queuedMove = Move.Right;

    void Update()
    {
        if (moving) MovePlayer();
        else
        {
            if (queuedMove != Move.None)
            {
                StartMoving();
            }
        }
    }

    void MovePlayer()
    {
        if (moveTimeElapsed > moveTime)
        {
            moveTimeElapsed = 0;
            moving = false;
            transform.SetPositionAndRotation(posSetpoint, orientSetpoint);
        }
        else
        {
            moveTimeElapsed += Time.deltaTime;
            float timeRatio = moveTimeElapsed / moveTime;
            timeRatio = timeRatio * timeRatio * (3f - 2f * timeRatio);
            // Spherically interpolate position and rotation
            transform.SetPositionAndRotation(
                Vector3.Slerp(posPreMove, posSetpoint, timeRatio),
                Quaternion.Slerp(orientPreMove, orientSetpoint, timeRatio)
            );
        }
    }

    void StartMoving(bool moveInstantly = false)
    {
        switch (queuedMove)
        {
            case Move.Left:
                column++;
                break;
            case Move.Right:
                column--;
                break;
        }

        int numberOfColumns = level.NumberOfColumns;

        if (column < 0)
            column += numberOfColumns;
        else if (column >= numberOfColumns)
            column -= numberOfColumns;

        moving = !moveInstantly;
        queuedMove = Move.None;
        posPreMove = transform.position;
        orientPreMove = transform.rotation;

        double2 newPosition = level.CalculateCoordinatesForColumn(column, distanceFromCircle);
        float yAngle = (float)level.CalculateCameraAngleForColumnInDegrees(column);
        float xAngle = transform.rotation.eulerAngles.x;

        posSetpoint = new Vector3((float)newPosition.x, 2 - floor * Level.DistanceBetweenFloors, (float)newPosition.y);
        orientSetpoint = Quaternion.Euler(xAngle, yAngle, 0);

        if (moveInstantly)
            transform.SetPositionAndRotation(posSetpoint, orientSetpoint);
        else
            moveSound.Play();
    }
}
