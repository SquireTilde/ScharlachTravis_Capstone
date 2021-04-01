using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float cameraAngleLimit = 70f;
    [SerializeField] GroundDetector gd;

    Rigidbody rb;

    Vector3 frameMove = Vector3.zero;
    float frameTurn = 0f;
    float frameLook = 0f;
    private float currentCameraRotationX = 0f;
    [SerializeField] bool isGrounded = true;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        gd.GroundDetected += OnGroundDetected;
        gd.GroundVanished += OnGroundVanished;
    }

    void OnDisable()
    {
        gd.GroundDetected -= OnGroundDetected;
        gd.GroundVanished -= OnGroundVanished;
    }

    void OnGroundDetected()
    {
        isGrounded = true;
    }

    void OnGroundVanished()
    {
        isGrounded = false;
    }


    public void Move(Vector3 movement)
    {
        frameMove = movement;
    }

    public void Turn(float turn)
    {
        frameTurn = turn;
    }

    public void Look(float look)
    {
        frameLook = look;
    }

    public void Jump(float jumpForce)
    {
        if (!isGrounded) return;
        rb.AddForce(jumpForce * Vector3.up);
    }

    void FixedUpdate()
    {
        ApplyMove(frameMove);
        ApplyTurn(frameTurn);
        ApplyLook(frameLook);
    }

    void ApplyMove(Vector3 moveVector)
    {
        if(moveVector != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveVector);
            frameMove = Vector3.zero;
        }
    }

    void ApplyTurn(float turnPower)
    {
        if (turnPower != 0)
        {
            Quaternion newRotation = Quaternion.Euler(0, turnPower, 0);
            rb.MoveRotation(rb.rotation * newRotation);

            frameTurn = 0;
        }
    }

    void ApplyLook(float lookPower)
    {
        if(lookPower != 0)
        {
            currentCameraRotationX -= lookPower;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraAngleLimit, cameraAngleLimit);
            _camera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);

            frameLook = 0;
        }
    }


}

