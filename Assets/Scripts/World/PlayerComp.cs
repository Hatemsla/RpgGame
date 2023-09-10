using UnityEngine;

public struct PlayerComp
{
    public Transform Transform;
    public Vector3 Position;
    public Quaternion Rotation;
    public CharacterController CharacterController;
    public Transform PlayerCameraRoot;
    public bool Grounded;
    public float VerticalVelocity;
}