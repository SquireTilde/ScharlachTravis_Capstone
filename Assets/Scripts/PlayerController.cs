using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    PlayerInput _input = null;
    PlayerMotor _motor = null;

    [SerializeField] float moveSpeed = .1f;
    [SerializeField] float sprintSpeed = .3f;
    [SerializeField] float turnSpeed = 6f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float currentSpeed = 0;

    enum TrapType { Spike = 0, Glue, Noise };

    // Start is called before the first frame update
    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        _input.MoveInput += OnMove;
        _input.RotateInput += OnRotate;
        _input.JumpInput += OnJump;
        _input.SprintDown += OnSprintDown;
        _input.SprintUp += OnSprintUp;
        _input.TrapInput += OnTrap;
    }

    void OnDisable()
    {
        _input.MoveInput -= OnMove;
        _input.RotateInput -= OnRotate;
        _input.JumpInput -= OnJump;
        _input.SprintDown -= OnSprintDown;
        _input.SprintUp -= OnSprintUp;
        _input.TrapInput -= OnTrap;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = moveSpeed;
    }

    void OnMove(Vector3 movement)
    {
        _motor.Move(movement * currentSpeed);
    }

    void OnRotate(Vector3 rotation)
    {
        _motor.Turn(rotation.y * turnSpeed);
        _motor.Look(rotation.x * turnSpeed);
    }

    void OnJump()
    {
        _motor.Jump(jumpPower);
    }

    void OnSprintDown()
    {
        currentSpeed = sprintSpeed;
    }
    
    void OnSprintUp()
    {
        currentSpeed = moveSpeed;
    }

    void OnTrap(PlayerInput.TrapType trapType)
    {
        Debug.Log("I try to place a " + trapType + "Trap");
    
    }
}
