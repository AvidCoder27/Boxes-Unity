using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private enum Move { None, Left, Right }

    [SerializeField] private float _moveTime; // 0.5f
    [SerializeField] private float _distanceFromCircle; // 4f
    [SerializeField] private UnityEvent<Key.Colors> _onGetKey;
    private Move _queuedMove;
    private bool _moving;
    private float _moveTimeElapsed;
    private Quaternion _orientPreMove;
    private Quaternion _orientSetpoint;
    private Vector3 _posPreMove;
    private Vector3 _posSetpoint;
    private int _column;
    private int _floor;
    private Key.Colors _collectedKeys = Key.Colors.Undefined;

    private PlayerInput _playerInput;
    private InputAction _moveLeft;
    private InputAction _moveRight;
    private InputAction _selectTop;
    private InputAction _selectBottom;
    private InputAction _interact;
    private InputAction _climb;

    private AudioSource _moveSound;
    private Level _level;

    private void OnEnable()
    {
        _moveSound = GetComponent<AudioSource>();
        _playerInput = GetComponentInChildren<PlayerInput>();
        _playerInput.ActivateInput();
        _playerInput.SwitchCurrentActionMap("Playing Phase");
        _playerInput.DeactivateInput();

        _moveLeft = _playerInput.actions["MoveLeft"];
        _moveRight = _playerInput.actions["MoveRight"];
        _selectTop = _playerInput.actions["SelectTop"];
        _selectBottom = _playerInput.actions["SelectBottom"];
        _interact = _playerInput.actions["PlayInteract"];
        _climb = _playerInput.actions["Climb"];

        _moveLeft.performed += MoveLeft_performed;
        _moveRight.performed += MoveRight_performed;
        _selectTop.performed += SelectTop_performed;
        _selectBottom.performed += SelectBottom_performed;
        _interact.performed += Interact_performed;
        _climb.performed += Climb_performed;
        Actions.OnGameEnd += HandleGameEnd;

        _level = LevelHandler.GetInstance().GetCurrentLevel();
        _moveTimeElapsed = 0f;
        _floor = 0;
        _column = UnityEngine.Random.Range(0, _level.NumberOfColumns);
        StartMoving(moveInstantly: true);
    }

    private void OnDisable()
    {
        _moveLeft.performed -= MoveLeft_performed;
        _moveRight.performed -= MoveRight_performed;
        _selectTop.performed -= SelectTop_performed;
        _selectBottom.performed -= SelectBottom_performed;
        _interact.performed -= Interact_performed;
        _climb.performed -= Climb_performed;
        Actions.OnGameEnd -= HandleGameEnd;
    }

    private void HandleGameEnd(Actions.GameEndState endState)
    {
        _queuedMove = Move.None;
        _playerInput.DeactivateInput();
    }

    public void ChangeFloor(int delta)
    {
        _floor += delta;
        _queuedMove = Move.None;
        StartMoving(moveInstantly: true);
    }

    public bool HasKey(Key.Colors keyColor)
    {
        return _collectedKeys.HasFlag(keyColor);
    }

    public void GiveKey(Key.Colors keyColor)
    {
        _collectedKeys |= keyColor;
        _onGetKey.Invoke(keyColor);
    }

    private void Interact_performed(InputAction.CallbackContext ctx)
    {
        if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                if (hit.transform.TryGetComponent(out Interactable interactable))
                {
                    interactable.InteractedWith(transform);
                }
            }
        }
    }

    private void Climb_performed(InputAction.CallbackContext ctx)
    {
        Actions.OnTryClimbLadder?.Invoke(_floor, _column, transform);
    }

    private void SelectBottom_performed(InputAction.CallbackContext ctx)
    {
        if (!_moving)
        {
            Actions.OnTryInteractBox?.Invoke(new int3(_floor, _column, 0));
        }
    }

    private void SelectTop_performed(InputAction.CallbackContext ctx)
    {
        if (!_moving)
        {
            Actions.OnTryInteractBox?.Invoke(new int3(_floor, _column, 1));
        }
    }

    private void MoveLeft_performed(InputAction.CallbackContext ctx)
    {
        _queuedMove = Move.Left;
    }

    private void MoveRight_performed(InputAction.CallbackContext ctx)
    {
        _queuedMove = Move.Right;
    }

    private void Update()
    {
        if (_moving)
        {
            MovePlayer();
        }
        else
        {
            if (_queuedMove != Move.None)
            {
                StartMoving();
            }
        }
    }

    private void MovePlayer()
    {
        if (_moveTimeElapsed > _moveTime)
        {
            _moveTimeElapsed = 0;
            _moving = false;
            transform.SetPositionAndRotation(_posSetpoint, _orientSetpoint);
        }
        else
        {
            _moveTimeElapsed += Time.deltaTime;
            float timeRatio = _moveTimeElapsed / _moveTime;
            timeRatio = timeRatio * timeRatio * (3f - (2f * timeRatio));
            // Spherically interpolate position and rotation
            transform.SetPositionAndRotation(
                Vector3.Slerp(_posPreMove, _posSetpoint, timeRatio),
                Quaternion.Slerp(_orientPreMove, _orientSetpoint, timeRatio)
            );
        }
    }

    private void StartMoving(bool moveInstantly = false)
    {
        switch (_queuedMove)
        {
            case Move.Left:
                _column++;
                break;
            case Move.Right:
                _column--;
                break;
        }

        int numberOfColumns = _level.NumberOfColumns;

        if (_column < 0)
        {
            _column += numberOfColumns;
        }
        else if (_column >= numberOfColumns)
        {
            _column -= numberOfColumns;
        }

        _moving = !moveInstantly;
        _queuedMove = Move.None;
        _posPreMove = transform.position;
        _orientPreMove = transform.rotation;

        float2 newPosition = _level.CalculateCoordinatesForColumn(_column, _distanceFromCircle);
        float yAngle = _level.CalculateCameraAngleForColumnInDegrees(_column);
        float xAngle = transform.rotation.eulerAngles.x;

        _posSetpoint = new Vector3(newPosition.x, 2 - (_floor * Level.DistanceBetweenFloors), newPosition.y);
        _orientSetpoint = Quaternion.Euler(xAngle, yAngle, 0);

        if (moveInstantly)
        {
            transform.SetPositionAndRotation(_posSetpoint, _orientSetpoint);
        }
        else
        {
            _moveSound.Play();
        }
    }

    /// <summary>
    /// Lock the player input and get a callback to unlock it.
    /// </summary>
    /// <returns>please invoke this callback in the future to unlock the movement</returns>
    public Action LockInputWithCallback()
    {
        _playerInput.DeactivateInput();
        return UnlockInput;
    }

    private void UnlockInput()
    {
        _playerInput.ActivateInput();
    }
}
