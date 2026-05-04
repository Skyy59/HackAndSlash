using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player_Controller : MonoBehaviour, IControllablePlayer
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CapsuleCollider2D playerCollider;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform weaponsTransform;

    private float _initialWeaponPosition;
    private float _initialHeight;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float decceleration = 5f;

    private float _accelerationTimer = 0f;

    private Vector2 _acceleratedInputs;

    private float _currentSpeed = 0f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int totalJumps = 2;

    private int _currentJumps = 0;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransition = 0.25f;
    [SerializeField] private float crouchSpeed = 2.5f;

    private float _crouchTimer = 0f;
    private bool _isCrouched = false;
    private bool _transitionToCrouch;
    private bool _requestCrouch;

    [Header("Slide")]
    [SerializeField] private float slideHeight = 0.75f;
    [SerializeField] private float slideTransition = 0.25f;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private float slideDuration = 1.5f;
    [SerializeField] private Vector2 slideInput;

    private float _slideTimer = 0f;
    private bool _isSliding = false;
    private bool _transitionToSlide = false;

    [Header("WallCheck")]
    [SerializeField] private Vector2 detectionOffset;
    [SerializeField] private Vector2 detectionSize;
    [SerializeField] private LayerMask wallMask;

    

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize = Vector2.zero;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask headLayer;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalSpeedClamp = 20f;

    [Header("Slope System")]
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private LayerMask slopeLayer;

    

    private float _verticalVelocity;

    private Vector2 _velocity;
    private Vector2 _inputs;
    private Vector2 _lastInputs;
    private Vector2 _playerSize;
    private Vector2 _playerCenter;

    public Action<Vector2> SendLookInput;
    public static Action OnRequestPause;
    public Action<bool> OnRequestAttack;
    public Action<float> OnRequestScroll;

    public Action OnInteractAction;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y + _initialHeight / 2f + 0.15f), new Vector2(groundCheckSize.x,
            _initialHeight - 0.15f));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(detectionOffset.x *
            _lastInputs.x, (_isCrouched || _isSliding) ? detectionOffset.y * 0.5f : detectionOffset.y),
            (_isCrouched || _isSliding) ? new Vector2(detectionSize.x, detectionSize.y * 0.25f) :
            detectionSize);
    }



    private void Awake()
    {
        _playerSize = playerCollider.size;
        _playerCenter = playerCollider.offset;
        _initialHeight = playerCollider.size.y;
        _initialWeaponPosition = weaponsTransform.localPosition.y;
        _currentSpeed = moveSpeed;
        _lastInputs.x = 1;
    }

    private void Update()
    {
        Acceleration();
        Movement();
        Gravity();
        

        TransitionToCrouch();
        TransitionToSlide();
        Slide();

        if (!CantStandUp() && _requestCrouch && !_isSliding) Crouch(false);

        playerSprite.localScale = new(crosshair.position.x > transform.position.x ? 1f : -1f, playerSprite.localScale.y, playerSprite.localScale.y);
        PlayerAnimatorParameters();
    }


    private void FixedUpdate()
    {
        ApplySpeed();
    }



    public void Movement()
    {
        float _move = (_isSliding ? slideInput.x : _acceleratedInputs.x) * _currentSpeed;
        _move = IsWalkingTowardsWall(_isSliding ? slideInput.x : _acceleratedInputs.x) ? 0f : _move;
        SetVelocity(_move); 

    }

    private void Acceleration()
    {
        _accelerationTimer = Mathf.Clamp(_accelerationTimer, 0, 1);

        if (_inputs.x != 0)
        {
            _accelerationTimer += Time.deltaTime * acceleration;
            _acceleratedInputs.x = Mathf.Lerp(0, _inputs.x, _accelerationTimer); 
        }
        else
        {
            _accelerationTimer -= Time.deltaTime * decceleration;
            _acceleratedInputs.x = Mathf.Lerp(0,_lastInputs.x, _accelerationTimer);
        }
    }

    private void SetVelocity(float _movementInput)
    {
        _velocity = new Vector2(_movementInput, _verticalVelocity);
    }

    private void ApplySpeed()
    {
        Vector2 finalVelocity = _velocity;

        if(IsGrounded() && _verticalVelocity <= 0)
        {
            finalVelocity = GetSlopeVelocity(_velocity);
        }
         
        
        _rb.linearVelocity = finalVelocity;
    }

    private void Gravity()
    {
        if (IsGrounded())
        {
            if (_verticalVelocity < 0) _verticalVelocity = -0.1f;
            _currentJumps = 0;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
            if (_verticalVelocity < 0) 
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -verticalSpeedClamp, 0);
        }


    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer);
    }

    private Vector2 GetSlopeVelocity(Vector2 horizontalVelocity)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, slopeLayer);

        if(hit.collider != null)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);

            if (angle > maxSlopeAngle) return new Vector2(0, _verticalVelocity);

            if(angle > 0.01f)
            {
                Vector2 slopeDir = Vector2.Perpendicular(hit.normal);
                return slopeDir * -horizontalVelocity.x;
            }

            
        }

        return horizontalVelocity;
    }

    private void Jump()
    {
        if (_isSliding || _transitionToSlide) return;

        if (IsGrounded() || (!IsGrounded() && _currentJumps < totalJumps))
        {
            _verticalVelocity = jumpForce;
            _currentJumps++;
            playerAnimator.Play("Jump", 0, 0f);
            if (_isCrouched) Crouch(false);
        }
    }

    private void Crouch(bool _state)
    {
        if (CantStandUp())
        {
            _requestCrouch = true;
            return;
        }

        _requestCrouch = false;

        if (IsGrounded() && Mathf.Abs(_inputs.x) == 0 && !_isCrouched && _state)
        {
            _transitionToCrouch = true;
            if (_crouchTimer > 0) _crouchTimer = 1f - _crouchTimer;
            else _crouchTimer = 0;
            _isCrouched = true;
        }
        else if (IsGrounded() && Mathf.Abs(_inputs.x) > 0 && !_isSliding  && !_transitionToSlide && _state)
        {
            _transitionToSlide = true;
            if (_slideTimer > 0) _slideTimer = 1f - _slideTimer;
            else _slideTimer = 0;
            _isSliding = true;
            slideInput = _inputs;
            playerAnimator.SetBool("Sliding", true);
        }
        else if ((IsGrounded() && _isCrouched) || !IsGrounded())
        {
            _transitionToCrouch = true;
            if (_crouchTimer > 0) _crouchTimer = 1f - _crouchTimer;
            else _crouchTimer = 0;
            _isCrouched = false;
        }
    }

    private bool CantStandUp()
    {
        return Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + _initialHeight / 2f + 0.15f), 
        new Vector2(groundCheckSize.x, _initialHeight - 0.15f), 0, headLayer) && (_isCrouched || _isSliding);
    }

    private void TransitionToCrouch()
    {
        if (!_transitionToCrouch) return;

        _crouchTimer += Time.deltaTime / crouchTransition;

        float _initialSize = _isCrouched ? _initialHeight : crouchHeight;
        float _desiredSize = _isCrouched ? crouchHeight : _initialHeight;

        float _initialCenter = _initialSize / 2f;
        float _desiredCenter = _desiredSize / 2f;

        float _initialSpeed = _isCrouched ? moveSpeed : crouchSpeed;
        float _desiredSpeed = _isCrouched ? crouchSpeed : moveSpeed;

        float _initialPosition = _isCrouched ? _initialWeaponPosition : _initialWeaponPosition / 2f;
        float _desiredPosition = _isCrouched ? _initialWeaponPosition / 2f : _initialWeaponPosition;

        _playerSize.y = Mathf.Lerp(_initialSize, _desiredSize, _crouchTimer);
        _playerCenter.y = Mathf.Lerp(_initialCenter, _desiredCenter, _crouchTimer);
        _currentSpeed = Mathf.Lerp(_initialSpeed, _desiredSpeed, _crouchTimer);
        weaponsTransform.localPosition = new Vector3(0, Mathf.Lerp(_initialPosition, _desiredPosition, _crouchTimer), 0);

        playerCollider.size = _playerSize;
        playerCollider.offset = _playerCenter;

        if (_crouchTimer >= 1f)
        {
            _transitionToCrouch = false;
            _crouchTimer = 0f;
            playerCollider.size = _playerSize;
            playerCollider.offset = _playerCenter;
            _currentSpeed = _desiredSpeed;
            weaponsTransform.localPosition = new Vector3(0, _desiredPosition, 0);
        }
    }

    private void TransitionToSlide()
    {
        if (!_transitionToSlide) return;

        _slideTimer += Time.deltaTime / slideTransition;

        float _initialSize = _isSliding ? _initialHeight : slideHeight;
        float _desiredSize = _isSliding ? slideHeight : _initialHeight;

        float _initialCenter = _initialSize / 2f;
        float _desiredCenter = _desiredSize / 2f;

        float _initialSpeed = _isSliding ? moveSpeed : slideSpeed;
        float _desiredSpeed = _isSliding ? slideSpeed : moveSpeed;

        float _initialPosition = _isSliding ? _initialWeaponPosition : _initialWeaponPosition / 2f;
        float _desiredPosition = _isSliding ? _initialWeaponPosition / 2f : _initialWeaponPosition;

        _playerSize.y = Mathf.Lerp(_initialSize, _desiredSize, _slideTimer);
        _playerCenter.y = Mathf.Lerp(_initialCenter, _desiredCenter, _slideTimer);
        _currentSpeed = Mathf.Lerp(_initialSpeed, _desiredSpeed, _slideTimer);
        weaponsTransform.localPosition = new Vector3(0, Mathf.Lerp(_initialPosition, _desiredPosition, _slideTimer), 0);

        if (_slideTimer >= 1f)
        {
            _transitionToSlide = false;
            _slideTimer = 0f;
            playerCollider.size = _playerSize;
            playerCollider.offset = _playerCenter;
            _currentSpeed = _desiredSpeed;
            weaponsTransform.localPosition = new Vector3(0, _desiredPosition, 0);
        }

    }

    private void Slide()
    {
        if (!_isSliding || _transitionToSlide) return;

        _slideTimer += Time.deltaTime / slideDuration;

        if(_slideTimer >= 1)
        {
            if (CantStandUp())
            {
                _slideTimer = 0f;
                return;
            }
            _slideTimer = 0f;
            _transitionToSlide = true;
            _isSliding = false;
            playerAnimator.SetBool("Sliding", false);
        }
    }


    private bool IsWalkingTowardsWall(float _direction)
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(detectionOffset.x *
            _direction, (_isCrouched || _isSliding) ? detectionOffset.y * 0.5f : detectionOffset.y),
            (_isCrouched || _isSliding) ? new Vector2(detectionSize.x, detectionSize.y * 0.25f) :
            detectionSize, 0, wallMask);
    }
    
    private void PlayerAnimatorParameters()
    {
        playerAnimator.SetFloat("Speed", Mathf.Abs(_rb.linearVelocityX) * (crosshair.position.x > transform.position.x ? 1 : -1));
        playerAnimator.SetBool("Grounded", IsGrounded());
        playerAnimator.SetBool("Crouched", _isCrouched);
    }

    public void PlayAnimation(string _animation)
    {
        // playerAnimator.Play(_animation, 0, 0f);
        // playerAnimator.SetInteger("Attack", (playerAnimator.GetInteger("Attack") + 1) % 3);
    }

    public float ReturnDirection() => _lastInputs.x;

    #region INPUTS
    public void OnAttack(bool _state)
    {
        OnRequestAttack?.Invoke(_state);
    }

    public void OnCrouch(bool _state)
    {
        Crouch(_state);
    }

    public void OnInteract()
    {
        OnInteractAction?.Invoke();
    }

    public void OnJump()
    {
        Jump();
    }

    public void OnLook(Vector2 _look)
    {
        SendLookInput?.Invoke(_look);
    }

    public void OnMovement(Vector2 _movement)
    {
        _inputs = _movement;
        if (_inputs.x != 0) _lastInputs = _inputs;
    }

    public void OnPause()
    {
        Game_Controller.instance.RequestPause();
        OnRequestPause?.Invoke();
    }

    public void OnScroll(float _value)
    {
        OnRequestScroll?.Invoke(_value);
    }
    #endregion



}
