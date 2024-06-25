using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] Transform updateLookObj;

    int tick = 60;
    float tickRate = 1f / 60f;
    float tickDeltaTime = 0f;

    const int BUFFER_SIZE = 1024;
    TransformState[] transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new();
    public TransformState previousTransformState;

    void ServerTransformState_OnValueChanged(TransformState previousValue, TransformState newValue)
    {
        previousTransformState = previousValue;
    }

    void OnEnable()
    {
        ServerTransformState.OnValueChanged += ServerTransformState_OnValueChanged;
    }

    public void ProcessLocalPlayerMovement()
    {
        tickDeltaTime += Time.deltaTime;
        if(tickDeltaTime > tickRate)
        {
            int bufferIndex = tick % BUFFER_SIZE;

            MovePlayerServerRpc();
            if(!IsHost)
            {
                MovePlayer();
            }

            TransformState transformState = new()
            {
                Tick = tick,
                Position = transform.position,
                HasStartedMoving = true
            };

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

    [ServerRpc] void MovePlayerServerRpc()
    {
        MovePlayer();

        TransformState state = new()
        {
            Tick = tick,
            Position = transform.position,
            HasStartedMoving = true
        };

        previousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = state;
    }

    void MovePlayer()
    {
        transform.position = updateLookObj.position;
    }

}
