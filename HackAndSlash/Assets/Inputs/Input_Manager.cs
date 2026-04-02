using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Manager : MonoBehaviour, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject playerGameObject;

    [SerializeField] private GameObject uiGameObject;

    private IControllablePlayer _controllablePlayer;
    private IControllableUI _controllableUI;
    
    private InputSystem_Actions _inputSystem;

    private void OnValidate() 
    {
        if (playerGameObject && !playerGameObject.TryGetComponent(out _controllablePlayer)) playerGameObject = null;
        if (uiGameObject && !uiGameObject.TryGetComponent(out _controllableUI)) uiGameObject = null;
    }

    private void OnEnable()
    {
        _inputSystem.Player.SetCallbacks(this);
        _inputSystem.UI.SetCallbacks(this);
    }

    private void OnDisable()
    {
        _inputSystem.Player.RemoveCallbacks(this);
        _inputSystem.UI.RemoveCallbacks(this);
    }

    private void Awake()
    {
        playerGameObject.TryGetComponent(out _controllablePlayer);
        if (uiGameObject) uiGameObject.TryGetComponent(out _controllableUI);

        _inputSystem = new InputSystem_Actions();
    }

    void Start()
    {
        _inputSystem.Enable();
        
        _inputSystem.Player.Enable();
        _inputSystem.UI.Disable();
    }

    public void SetPlayerInputs(bool _state)
    {
        if (_state) _inputSystem.Player.Enable();
        else _inputSystem.Player.Disable();
    }

    public void SetUIInputs(bool _state)
    {
        if (_state) _inputSystem.UI.Enable();
        else _inputSystem.UI.Disable();
    }

    public void RemoveAllInputs()
    {
        _inputSystem.Disable();
        _inputSystem.Dispose();
    }

    #region Player

    public void OnMove(InputAction.CallbackContext context)
    {
        _controllablePlayer.OnMovement(context.performed ? context.ReadValue<Vector2>() : Vector2.zero);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 _look = context.ReadValue<Vector2>();
        _controllablePlayer.OnLook(context.performed ? new Vector2(_look.x, _look.y) : Vector2.zero);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        switch (context)
        {
            case { performed: true, duration: >= 0.05f }:
                _controllablePlayer.OnAttack(true);
                break;
            case { canceled: true, duration: >= 0.05f}:
                _controllablePlayer.OnAttack(false);
                break;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) _controllablePlayer.OnInteract();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        switch (context)
        {
            case { performed: true, duration: >= 0.05f }:
                _controllablePlayer.OnCrouch(true);
                break;
            case { canceled: true, duration: >= 0.05f}:
                _controllablePlayer.OnCrouch(false);
                break;
        }  
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) _controllablePlayer.OnJump();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            float _scroll = context.ReadValue<float>();
            _controllablePlayer.OnScroll(_scroll);  
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started) _controllablePlayer.OnPause();
    }

    #endregion

    #region UI

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.started) _controllableUI.OnUnPause();
    }

    #endregion
}
