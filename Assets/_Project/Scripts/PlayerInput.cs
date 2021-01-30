using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Rewired.Player _player;

    void Awake()
    {
        _player = ReInput.players.GetPlayer(0);
    }

    public float GetHorizontalMovementInput()
    {
        return _player.GetAxis("Move Horizontal");
    }

    public float GetVerticalMovementInput()
    {
        return _player.GetAxis("Move Vertical");
    }

    public bool IsJumpKeyPressed()
    {
        return false;
    }
}