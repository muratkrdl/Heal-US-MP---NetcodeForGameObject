using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkMovementComponent : NetworkBehaviour
{
#region Variables
    [SerializeField] CharacterController characterController;

    int tick = 60;
    float tickRate = 1f / 60f;
    float tickDeltaTime = 0f;

    const int BUFFER_SIZE = 1024;
    InputState[] inputStates = new InputState[BUFFER_SIZE];
    TransformState[] transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new();
    public TransformState previousTransformState;

    Vector3 inputDirection = Vector3.zero;
    float _speed;
    float _verticalVelocity;

    [SerializeField] float GroundedOffset = -0.14f;
    [SerializeField] float GroundedRadius = 0.5f;
    [SerializeField] LayerMask GroundLayers;
    bool Grounded = true;
    bool jump = false;

    float _jumpTimeoutDelta;
	float _fallTimeoutDelta;

    float _terminalVelocity = 53.0f;

    [Space(10)]
	public float JumpHeight = 1.2f;
	public float Gravity = -15.0f;
	[Space(10)]
	public float JumpTimeout = 0.1f;
	public float FallTimeout = 0.15f;

    [SerializeField] KeyCode jumpkeyCode;
    
    [Header("Cinemachine")]
	public GameObject CinemachineCameraTarget;
	public float TopClamp = 90.0f;
	public float BottomClamp = -90.0f;

	public float Sens = 1.0f;

    float _cinemachineTargetPitch;
    float _rotationVelocity; 
#endregion

    public bool GetIsGrounded
    {
        get
        {
            return Grounded;
        }
    }

    public bool GetIsJump
    {
        get
        {
            return jump;
        }
    }

    void ServerTransformState_OnValueChanged(TransformState previousValue, TransformState newValue)
    {
        previousTransformState = previousValue;
    }

    void OnEnable() 
    {
        ServerTransformState.OnValueChanged += ServerTransformState_OnValueChanged;
    }

    void Start() 
    {
        _jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
    }

    void Update() 
    {
        GroundedCheck();
        JumpAndGravity();
    }

    public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput, float speed)
    {
        tickDeltaTime += Time.deltaTime;
        if(tickDeltaTime > tickRate)
        {
            int bufferIndex = tick % BUFFER_SIZE;

            MovePlayerServerRpc(tick ,movementInput, lookInput, speed);
            if(!IsHost)
            {
                MovePlayer(movementInput, speed);
                RotatePlayer(lookInput);
            }

            InputState inputState = new()
            {
                Tick = tick,
                MovementInput = movementInput,
                MouseLookInput = lookInput
            };

            TransformState transformState = new()
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            inputStates[bufferIndex] = inputState;
            transformStates[bufferIndex] = transformState;

            tickDeltaTime -= tickRate;
            if(tick == BUFFER_SIZE)
            {
                tick = 0;
            }
            else
            {
                tick++;
            }
        }
    }

    public void ProcessSimulatedPlayerMovement()
    {
        tickDeltaTime += Time.deltaTime;
        if(tickDeltaTime > tickRate)
        {
            if(ServerTransformState.Value.HasStartedMoving)
            {
                transform.SetPositionAndRotation(ServerTransformState.Value.Position, ServerTransformState.Value.Rotation);
            }

            tickDeltaTime -= tickRate;

            if(tick == BUFFER_SIZE)
            {
                tick = 0;
            }
            else
            {
                tick++;
            }
        }
    }

    [ServerRpc] void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput, float speed)
    {
        MovePlayer(movementInput, speed);
        RotatePlayer(lookInput);

        TransformState state = new()
        {
            Tick = tick,
            Position = transform.position,
            Rotation = transform.rotation,
            HasStartedMoving = true
        };

        previousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = state;
    }

    void MovePlayer(Vector2 movementInput, float targetSpeed)
    {
        _speed = targetSpeed;

		inputDirection = new Vector3(movementInput.x, 0.0f, movementInput.y).normalized;

		if (movementInput != Vector2.zero)
		{
			inputDirection = (transform.right * movementInput.x + transform.forward * movementInput.y).normalized;
		}

		characterController.Move(inputDirection * (_speed * tickRate) + new Vector3(0.0f, _verticalVelocity, 0.0f) * tickRate);
    }

    void RotatePlayer(Vector2 lookInput)
    {
        float deltaTimeMultiplier = 1.0f;
		
		_cinemachineTargetPitch -= lookInput.y * Sens * deltaTimeMultiplier;
		_rotationVelocity = lookInput.x * Sens * deltaTimeMultiplier;

		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

		transform.Rotate(Vector3.up * _rotationVelocity);
    }

    void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

    void JumpAndGravity()
	{
		if(Input.GetKeyDown(jumpkeyCode) && Grounded)
		{
			jump = true;
		}
		if(Grounded)
		{
			_fallTimeoutDelta = FallTimeout;
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}
			// Jump
			if (jump && _jumpTimeoutDelta <= 0.0f)
			{
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;
			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}
			// if we are not grounded, do not jump
			jump = false;
			inputDirection = Vector2.zero;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}

	static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

}

