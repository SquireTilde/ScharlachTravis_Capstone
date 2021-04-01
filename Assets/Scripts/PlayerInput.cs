using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector3> MoveInput = delegate { };
    public event Action<Vector3> RotateInput = delegate { };
    public event Action JumpInput = delegate { };
    public event Action SprintDown = delegate { };
    public event Action SprintUp = delegate { };
    public event Action<TrapType> TrapInput = delegate { };

    public enum TrapType {Spike = 0, Glue, Noise};

    // Update is called once per frame
    void Update()
    {
        DetectMoveInput();
        DetectRotateInput();
        DetectJumpInput();
        DetectSprintDown();
        DetectSprintUp();
        DetectTrapInput();
    }

    void DetectMoveInput()
    {
        float xInput = Input.GetAxisRaw("Horizontal1");
        float yInput = Input.GetAxisRaw("Vertical1");
        if(xInput != 0 || yInput != 0)
        {
            Vector3 hMove = xInput * transform.right;
            Vector3 vMove = yInput * transform.forward;
            Vector3 totalMove = (vMove + hMove).normalized;
            MoveInput?.Invoke(totalMove);
        }
    }

    void DetectRotateInput()
    {
        float xRInput = Input.GetAxisRaw("Mouse X1");
        float yRInput = Input.GetAxisRaw("Mouse Y1");
        if(xRInput != 0 || yRInput != 0)
        {
            Vector3 rotate = new Vector3(yRInput, xRInput, 0);
            RotateInput?.Invoke(rotate);
        }
    }

    void DetectJumpInput()
    {
        if (Input.GetButtonDown("Jump1"))
        {
            JumpInput?.Invoke();
        }
    }

    void DetectSprintDown()
    {
        if (Input.GetButtonDown("Sprint1"))
        {
            SprintDown?.Invoke();
        }
    }

    void DetectSprintUp()
    {
        if (Input.GetButtonUp("Sprint1"))
        {
            SprintUp?.Invoke();
        }
    }

    void DetectTrapInput()
    {
        bool spikeTrap = Input.GetAxisRaw("Fire11") != 0;
        bool glueTrap = Input.GetAxisRaw("Fire21") != 0;
        bool noiseBall = Input.GetAxisRaw("Fire31") !=0;

        if (spikeTrap)
        {
            TrapInput?.Invoke(TrapType.Spike);
        }
        else if (glueTrap)
        {
            TrapInput?.Invoke(TrapType.Glue);
        }
        else if (noiseBall)
        {
            TrapInput?.Invoke(TrapType.Noise);
        }
    }
}
