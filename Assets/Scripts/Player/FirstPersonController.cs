using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum HumanType
{
	none,
	patrick,
	doctor,
	villager
}

public class FirstPersonController : NetworkBehaviour
{
	[SerializeField] NetworkMovementComponent networkMovementComponent;

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

	PlayerManager playerManager;

	public PlayerManager SetPlayerManager
	{
		set
		{
			playerManager = value;
		}
	}

	Vector2 moveInput;
	Vector2 mouseInput;

	[Space(10)]
	[Header("SFX & VFX")]
	[SerializeField] AudioSource walkSFX;
	[SerializeField] AudioSource runSFX;

	[Header("Player")]
	public float MoveSpeed = 4.0f;
	public float SprintSpeed = 6.0f;

	bool canSprint;

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

	bool IsPressingW => Input.GetKey(KeyCode.W);
	bool IsPressingS => Input.GetKey(KeyCode.S);
	bool IsPressingA => Input.GetKey(KeyCode.A);
	bool IsPressingD => Input.GetKey(KeyCode.D);

	HumanType humanType;

	public EscMenu GetEscMenu
	{
		get
		{
			return escMenu;
		}
	}

	public HumanType HumanType
	{
		get
		{
			return humanType;
		}
		set
		{
			humanType = value;
		}
	}

	void Awake() 
	{
		_controller.enabled = false;
	}

 	public void PlayerDead()
	{
		characterAnimatorScript.Die();
		
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
		GetComponentInChildren<MouseLookObj>().enabled = false;
		
		DeadServerRpc();
	}

	[ServerRpc(RequireOwnership = false)] void DeadServerRpc()
	{
		DeadClientRpc();
	}

	[ClientRpc] void DeadClientRpc()
	{
		StopRunSFX();
		StopWalkSFX();
		Destroy(NicknameCanvas.gameObject);
		myCollider.enabled = false;
		enabled = false;
	}

	void Start()
	{
		SetCursorState(true);

		initialMoveSpeed = MoveSpeed;
		initialSprintSpeed = SprintSpeed;
	}

    public override void OnNetworkSpawn()
    {
		if(!IsOwner)
		{
			Destroy(cameraHolder.gameObject);
			Destroy(canvas.gameObject);
			Destroy(hand.gameObject);
			return;
		}

		foreach (var item in characterModelItems)
		{
			item.enabled = false;
		}

		PlayerManager[] managers = FindObjectsOfType<PlayerManager>();
		foreach (var item in managers)
		{
			if(item.OwnerClientId == OwnerClientId)
			{
				SetPlayerManager = item;
				humanType = playerManager.playerData.characterIndex switch
				{
            		1 => HumanType.patrick,
            		2 => HumanType.doctor,
            		_ => HumanType.villager
				};

				CharacterSpawnedServerRpc();
				Manager.Instance.ClientConnected();
				break;
			}
			else
			{
				continue;
			}
		}

		SceneManager.sceneLoaded += SceneManager_SceneLoaded;
		escMenu = Manager.Instance.GetEscMenu;
		escMenu.FirstPersonController = this;
		_controller.enabled = true;
    }
	
	[ServerRpc(RequireOwnership = false)] void CharacterSpawnedServerRpc(ServerRpcParams serverRpcParams = default)
	{
		CharacterSpawnedClientRpc(serverRpcParams.Receive.SenderClientId);
	}

	[ClientRpc] void CharacterSpawnedClientRpc(ulong _clientId)
	{
		if(NetworkManager.LocalClientId == _clientId)
		{
			Transform spawnTransform = HumanType switch
        	{
        	    HumanType.patrick => SpawnManager.Instance.GetPatrickTransforms[UnityEngine.Random.Range(0,SpawnManager.Instance.GetPatrickTransforms.Length)],
        	    HumanType.doctor => SpawnManager.Instance.GetDoctorTransforms[UnityEngine.Random.Range(0,SpawnManager.Instance.GetDoctorTransforms.Length)],
        	    _ => SpawnManager.Instance.GetVillagerTransforms[UnityEngine.Random.Range(0,SpawnManager.Instance.GetVillagerTransforms.Length)]
        	};

			transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
			SetTransformToServerRpc(spawnTransform.position, spawnTransform.rotation);
		}

		_controller.enabled = true;
	}

	[ServerRpc(RequireOwnership = false)] void SetTransformToServerRpc(Vector3 pos, Quaternion rot)
	{
		SetTransformToClientRpc(pos, rot);
	}

	[ClientRpc] void SetTransformToClientRpc(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

    void SceneManager_SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
		DestroyObjServerRpc();
    }

	[ServerRpc(RequireOwnership = false)] void DestroyObjServerRpc()
	{
		GetComponent<NetworkObject>().Despawn();
	}

    public override void OnNetworkDespawn()
    {
		SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
    }

	void Update()
	{
		if(!Manager.Instance.GetCanStart) return;

		if(escMenu != null)
		{
			if(escMenu.GetIsThinking && IsOwner) { return; }
		}

		if(IsOwner)
		{
			mouseInput.x = Input.GetAxisRaw("Mouse X");
			mouseInput.y = Input.GetAxisRaw("Mouse Y");

			if(humanType == HumanType.doctor)
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
		}

		GetInputs();
		Move();	
	}

	void GetInputs()
	{
		if(!IsOwner) return;

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

		if(Stamina.Instance.GetCanSprint)
		{
			if(!networkMovementComponent.GetIsJump)
			{
				StopWalkSFX();
				PlayRunSFX();
			}
			realSprintSpeed = SprintSpeed;
			characterAnimatorScript.SpeedUpAnimatorSpeed();
			if(fpsAnimatorScript == null) return;
			fpsAnimatorScript.SetWalkMultiplier(1.5f);
		}
		else
		{
			if(!networkMovementComponent.GetIsJump)
			{
				PlayWalkSFX();
				StopRunSFX();
			}
			realSprintSpeed = MoveSpeed;
			characterAnimatorScript.NormalAnimatorSpeed();
			if(fpsAnimatorScript == null) return;
			fpsAnimatorScript.SetWalkMultiplier(1);
		}

		if(moveInput == Vector2.zero)
		{
			StopWalkSFX();
			StopRunSFX();
			animationInput = moveInput;
			ResetWalkAnimation();
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

		if(Input.GetKeyDown(jumpkeyCode) && networkMovementComponent.GetIsGrounded)
		{
			StopWalkSFX();
			StopRunSFX();
			characterAnimatorScript.Jump();
		}
	}

	public void ResetAllInputs()
	{
		moveInput = Vector2.zero;
		mouseInput = Vector2.zero;
		animationInput = Vector2.zero;
		characterAnimatorScript.SetCharacterAnimationFloatDirectly(animationInput);
		StopWalkSFX();
		StopRunSFX();
		if(fpsAnimatorScript == null) return;
		fpsAnimatorScript.Idle();
	}

	void Move()
	{
		float targetSpeed = canSprint ? realSprintSpeed : MoveSpeed;

		if(moveInput == Vector2.zero)
		{
			targetSpeed = 0.0f;
		}

		if(IsClient && IsOwner)
		{
			networkMovementComponent.ProcessLocalPlayerMovement(moveInput, mouseInput, targetSpeed);
		}
		if(!IsOwner)
		{
			networkMovementComponent.ProcessSimulatedPlayerMovement();
		}
	}

	public void SetCursorState(bool lockCursor)
	{
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void ResetWalkAnimation()
	{
		if(fpsAnimatorScript == null) return;
		fpsAnimatorScript.SetWalkAnimationFalse();
	}

	void FPSWalkAnimationTrue()
	{
		if(fpsAnimatorScript == null) return;
		if(!fpsAnimatorScript.GetWalkAnimationBool())
		{
			fpsAnimatorScript.SetWalkAnimationTrue();
		}
	}

#region PlayStopSFX
 	void PlayWalkSFX()
	{
		PlayWalkSFXServerRpc();
	}

	[ServerRpc(RequireOwnership = false)] void PlayWalkSFXServerRpc()
	{
		PlayWalkSFXClientRpc();
	}

	[ClientRpc] void PlayWalkSFXClientRpc()
	{
		if(walkSFX.isPlaying) return;
		walkSFX.Play();
	}

	void StopWalkSFX()
	{
		StopWalkSFXServerRpc();
	}

	[ServerRpc(RequireOwnership = false)] void StopWalkSFXServerRpc()
	{
		StopWalkSFXClientRpc();
	}

	[ClientRpc] void StopWalkSFXClientRpc()
	{
		if(!walkSFX.isPlaying) return;
		walkSFX.Stop();
	}

	void PlayRunSFX()
	{
		PlayRunSFXServerRpc();
	}

	[ServerRpc(RequireOwnership = false)] void PlayRunSFXServerRpc()
	{
		PlayRunSFXClientRpc();
	}

	[ClientRpc] void PlayRunSFXClientRpc()
	{
		if(runSFX.isPlaying) return;
		runSFX.Play();
	}

	void StopRunSFX()
	{
		StopRunSFXServerRpc();
	}

	[ServerRpc(RequireOwnership = false)] void StopRunSFXServerRpc()
	{
		StopRunSFXClientRpc();
	}

	[ClientRpc] void StopRunSFXClientRpc()
	{
		if(!runSFX.isPlaying) return;
		runSFX.Stop();
	}
#endregion

}
