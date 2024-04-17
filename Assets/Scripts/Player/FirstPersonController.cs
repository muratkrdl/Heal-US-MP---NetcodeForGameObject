using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
	[SerializeField] PhotonView PV;
	[SerializeField] Transform cameraHolder;
	[SerializeField] Transform hand;
	[SerializeField] Canvas canvas;
	[SerializeField] Canvas NicknameCanvas;
	[SerializeField] SkinnedMeshRenderer[] characterModelItems;
	[SerializeField] CharacterController _controller;
	[SerializeField] BoxCollider myCollider;

	[SerializeField] CharacterAnimation characterAnimatorScript;
	[SerializeField] FPSAnimation fpsAnimatorScript;

	[SerializeField] EscMenu escMenu;
 
	[SerializeField] float lerpTime;
	Vector2 animationInput;

	[Space(10)]
	[Header("KeyCodes")]
	[SerializeField] KeyCode jumpkeyCode;
	[SerializeField] KeyCode sprintKeyCode;

#region Variables
 	HumanType state;

	// inputs
	Vector3 inputDirection = Vector3.zero;
	Vector2 moveInput;
	Vector2 mouseInput;
	bool jump;

	[Space(10)]
	[Header("SFX & VFX")]
	[SerializeField] AudioSource walkSFX;
	[SerializeField] AudioSource runSFX;

	PlayerManager playerManager;

	[Header("Player")]
	public float MoveSpeed = 4.0f;
	public float SprintSpeed = 6.0f;
	public float Sens = 1.0f;
	public float SpeedChangeRate = 10.0f;

	bool canSprint;

	[Space(10)]
	public float JumpHeight = 1.2f;
	public float Gravity = -15.0f;
	[Space(10)]
	public float JumpTimeout = 0.1f;
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	public bool Grounded = true;
	public float GroundedOffset = -0.14f;
	public float GroundedRadius = 0.5f;
	public LayerMask GroundLayers;

	[Header("Cinemachine")]
	public GameObject CinemachineCameraTarget;
	public float TopClamp = 90.0f;
	public float BottomClamp = -90.0f;

	float initialMoveSpeed;
	float initialSprintSpeed;

	public float GetInitialMoveSpeed
	{
		get
		{
			return initialMoveSpeed;
		}
	}
	public float GetInitialSprintSpeed
	{
		get
		{
			return initialSprintSpeed;
		}
	}

	float realSprintSpeed;

	// cinemachine
	float _cinemachineTargetPitch;

	// player
	float _speed;
	float _rotationVelocity;
	float _verticalVelocity;
	float _terminalVelocity = 53.0f;

	// timeout deltatime
	float _jumpTimeoutDelta;
	float _fallTimeoutDelta;

#endregion

	bool IsPressingW => Input.GetKey(KeyCode.W);
	bool IsPressingS => Input.GetKey(KeyCode.S);
	bool IsPressingA => Input.GetKey(KeyCode.A);
	bool IsPressingD => Input.GetKey(KeyCode.D);

	public PlayerManager GetPlayerManager
	{
		get
		{
			return playerManager;
		}
	}

 	public void PlayerDead()
	{
		if(!PV.IsMine) { return; }
		foreach (var item in characterModelItems)
		{
			if(!item.enabled)
			{
				item.enabled = true;
			}
		}

		Destroy(_controller);
		Destroy(canvas.gameObject);
		Destroy(hand.gameObject);
		PV.RPC(nameof(RPC_Dead),RpcTarget.All);
	}

	[PunRPC] void RPC_Dead()
	{
		Destroy(NicknameCanvas.gameObject);
		myCollider.enabled = false;
		enabled = false;
	}

	void Start()
	{
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		state = playerManager.GetHumanType;

		if(!PV.IsMine)
		{
			foreach (Transform item in cameraHolder.GetComponentsInChildren<Transform>())
            {
                Destroy(item.gameObject);
            }
            Destroy(_controller);
			Destroy(canvas.gameObject);
			Destroy(hand.gameObject);
		}
		else
		{
			foreach (var item in characterModelItems)
			{
				item.enabled = false;
			}
		}

		if(!PV.IsMine) { return; }

		SetCursorState(true);

		initialMoveSpeed = MoveSpeed;
		initialSprintSpeed = SprintSpeed;

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	void Update()
	{
		if(!PV.IsMine) { return; }
		if(escMenu.GetIsThinking || playerManager.GetIsDead) { return; }

		mouseInput.x = Input.GetAxisRaw("Mouse X");
		mouseInput.y = Input.GetAxisRaw("Mouse Y");

		if(state == HumanType.doctor)
		{
			if(GetComponent<Doctor>().GetIsStunned)
			{
				if(moveInput != Vector2.zero)
					moveInput = Vector2.zero;
				if(animationInput != Vector2.zero)
					animationInput = Vector2.zero;
				return;
			}
		}

		GetInputs();
		JumpAndGravity();
		GroundedCheck();
		Move();
	}

	void LateUpdate()
	{
		if(!PV.IsMine) { return; }

		CameraRotation();
	}

	void GetInputs()
	{
		if(IsPressingA || IsPressingD)
		{
			moveInput.x = Input.GetAxis("Horizontal");
			FPSWalkAnimationTrue();
		}
		else
		{
			moveInput.x = 0;
		}

		if(IsPressingW || IsPressingS)
		{
			moveInput.y = Input.GetAxis("Vertical");
			FPSWalkAnimationTrue();
		}
		else
		{
			moveInput.y = 0;
		}

		animationInput = moveInput;

		characterAnimatorScript.SetCharacterAnimationFloat(animationInput);

		if(Input.GetKeyDown(sprintKeyCode))
		{
			canSprint = true;
		}
		else if(Input.GetKeyUp(sprintKeyCode))
		{
			canSprint = false;
		}
	}

	public void ResetAllInputs()
	{
		moveInput = Vector2.zero;
		mouseInput = Vector2.zero;
		fpsAnimatorScript.Idle();
		animationInput = Vector2.zero;
	}

	void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

	void CameraRotation()
	{
		//Don't multiply mouse input by Time.deltaTime
		float deltaTimeMultiplier = 1.0f;
		
		_cinemachineTargetPitch -= mouseInput.y * Sens * deltaTimeMultiplier;
		_rotationVelocity = mouseInput.x * Sens * deltaTimeMultiplier;

		// clamp our pitch rotation
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Update Cinemachine camera target pitch
		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

		// rotate the player left and right
		transform.Rotate(Vector3.up * _rotationVelocity);
	}

	void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		if(Stamina.Instance.GetCanSprint)
		{
			if(!jump)
			{
				StopWalkSFX();
				PlayRunSFX();
			}
			realSprintSpeed = SprintSpeed;
			characterAnimatorScript.SpeedUpAnimatorSpeed();
			fpsAnimatorScript.SetWalkMultiplier(1.5f);
		}
		else
		{
			if(!jump)
			{
				PlayWalkSFX();
				StopRunSFX();
			}
			realSprintSpeed = MoveSpeed;
			characterAnimatorScript.NormalAnimatorSpeed();
			fpsAnimatorScript.SetWalkMultiplier(1);
		}

		float targetSpeed = canSprint ? realSprintSpeed : MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if(moveInput == Vector2.zero)
		{
			StopWalkSFX();
			StopRunSFX();
			animationInput = moveInput;
			targetSpeed = 0.0f;
			ResetWalkAnimation();
		}

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
		float speedOffset = 0.1f;
		float inputMagnitude = 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (moveInput != Vector2.zero)
		{
			// move
			inputDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
		}

		// move the player
		_controller.Move(inputDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	void JumpAndGravity()
	{
		if(Input.GetKeyDown(jumpkeyCode) && Grounded)
		{
			jump = true;
			StopWalkSFX();
			StopRunSFX();
			characterAnimatorScript.Jump();
		}
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;
			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}
			// Jump
			if (jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}
			// jump timeout
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

	void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}

	public void SetCursorState(bool lockCursor)
	{
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void ResetWalkAnimation()
	{
		fpsAnimatorScript.SetWalkAnimationFalse();
	}

	void FPSWalkAnimationTrue()
	{
		if(!fpsAnimatorScript.GetWalkAnimationBool())
		{
			fpsAnimatorScript.SetWalkAnimationTrue();
		}
	}

 	void PlayWalkSFX()
	{
		PV.RPC(nameof(RPC_PlayWalkSFX),RpcTarget.All);
	}

	[PunRPC] void RPC_PlayWalkSFX()
	{
		if(walkSFX.isPlaying) return;
		walkSFX.Play();
	}

	void StopWalkSFX()
	{
		PV.RPC(nameof(RPC_StopWalkSFX),RpcTarget.All);
	}

	[PunRPC] void RPC_StopWalkSFX()
	{
		if(!walkSFX.isPlaying) return;
		walkSFX.Stop();
	}

	void PlayRunSFX()
	{
		PV.RPC(nameof(RPC_PlayRunSFX),RpcTarget.All);
	}

	[PunRPC] void RPC_PlayRunSFX()
	{
		if(runSFX.isPlaying) return;
		runSFX.Play();
	}

	void StopRunSFX()
	{
		PV.RPC(nameof(RPC_StopRunSFX),RpcTarget.All);
	}

	[PunRPC] void RPC_StopRunSFX()
	{
		if(!runSFX.isPlaying) return;
		runSFX.Stop();
	}

}
