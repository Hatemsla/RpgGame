using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    private MainInput _mainInput;

    public event Action<Vector2> MoveEvent;
    
    private void Awake()
    {
        Instance = this;
        _mainInput ??= new MainInput();

        _mainInput.Player.Move.performed += _ => MoveEvent?.Invoke(_.ReadValue<Vector2>());
    }
}
