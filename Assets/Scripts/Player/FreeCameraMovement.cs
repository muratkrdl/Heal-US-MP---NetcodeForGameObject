using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class FreeCameraMovement : MonoBehaviour
{
	[SerializeField] CharacterController _controller;

	[SerializeField] EscMenu escMenu;

	[SerializeField] KeyCode sprintKeyCode;

	Vector3 inputDirection = Vector3.zero;
	Vector2 moveInput;
	Vector2 mouseInput;

	public float MoveSpeed = 4.0f;
	public float SprintSpeed = 6.0f;
	public float Sens = 1.0f;
	public float SpeedChangeRate = 10.0f;

	bool canSprint;

	public GameObject CinemachineCameraTarget;

	public float TopClamp = 90.0f;
	public float BottomClamp = -90.0f;

	float _cinemachineTargetPitch;

	float _speed;
	float _rotationVelocity;
	float _verticalVelocity;

	void Start()
	{
        escMenu.SetUnFade();
		SetCursorState(true);
	}

	void Update()
	{
		if(escMenu.GetIsThinking) { return; }

		GetInputs();
		Move();
	}

	void LateUpdate()
	{
		CameraRotation();
	}

	void GetInputs()
	{
		moveInput.x = Input.GetAxisRaw("Horizontal");
		moveInput.y = Input.GetAxisRaw("Vertical");
		mouseInput.x = Input.GetAxisRaw("Mouse X");
		mouseInput.y = Input.GetAxisRaw("Mouse Y");

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
	}

	void CameraRotation()
	{
		float deltaTimeMultiplier = 1.0f;
		
		_cinemachineTargetPitch -= mouseInput.y * Sens * deltaTimeMultiplier;
		_rotationVelocity = mouseInput.x * Sens * deltaTimeMultiplier;

		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

		transform.Rotate(Vector3.up * _rotationVelocity);
	}

	void Move()
	{
		float targetSpeed = canSprint ? SprintSpeed : MoveSpeed;

		if (moveInput == Vector2.zero)
		{
			targetSpeed = 0.0f;
		}

		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
		float speedOffset = 0.1f;
		float inputMagnitude = 1f;

		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

		if (moveInput != Vector2.zero)
		{
			inputDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
		}

		_controller.Move(inputDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	public void SetCursorState(bool lockCursor)
	{
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

}
