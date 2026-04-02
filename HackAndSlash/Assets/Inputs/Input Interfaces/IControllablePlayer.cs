using UnityEngine;

public interface IControllablePlayer
{
    public void OnMovement(Vector2 _movement);

    public void OnLook(Vector2 _look);

    public void OnJump();

    public void OnAttack(bool _state);

    public void OnInteract();

    public void OnCrouch(bool _state);

    public void OnScroll(float _value);

    public void OnPause();
}
