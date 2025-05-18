using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private PlayerStats stats;
        [SerializeField] private ParticleSystem dashParticles;
        [SerializeField] private TrailRenderer dashTrail;

        private Rigidbody2D _rb;
        private BoxCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        public event Action Dashed;
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        private float _time;
        
        private Vector3 _originalScale;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
            _originalScale = transform.localScale;
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Start()
        {
            dashTrail.emitting = false;
            
            _jumpToConsume = false;
            _frameInput = default;
            _timeJumpWasPressed = float.MinValue;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                DashDown = Input.GetButtonDown("Dash"),
            };

            if (stats.snapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < stats.horizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < stats.verticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (_frameInput.DashDown)
            {
                _dashToConsume = true;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDash();
            HandleDirection();
            HandleGravity();
            HandleSpriteFlip();

            if (_isDashing && _time >= _dashStartTime + stats.dashDuration)
            {
                _isDashing = false;
                
                float reducedX = Mathf.Lerp(_frameVelocity.x, Mathf.Clamp(_frameVelocity.x, -stats.maxSpeed, stats.maxSpeed), 0.5f);
                _frameVelocity = new Vector2(reducedX, _frameVelocity.y);
                
                transform.localScale = _originalScale;
                dashTrail.emitting = false;
            }

            ApplyMovement();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, CapsuleDirection2D.Vertical, 0, Vector2.down, stats.grounderDistance, ~stats.playerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, CapsuleDirection2D.Vertical, 0, Vector2.up, stats.grounderDistance, ~stats.playerLayer);

            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _dashUsed = false;
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + stats.jumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + stats.coyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = stats.jumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Dashing

        private bool _dashToConsume;
        private bool _isDashing;
        private bool _dashUsed;
        private float _dashStartTime;
        private Vector2 _dashDirection;
        
        private float _lastDashTime;

        private bool CanDash => !_isDashing && !_dashUsed && (_time >= _lastDashTime + stats.dashCooldown);

        private void HandleDash()
        {
            if (CanDash && _dashToConsume)
            {
                ExecuteDash();
            }
            
            _dashToConsume = false;
        }

        private void ExecuteDash()
        {
            _dashDirection = _frameInput.Move != Vector2.zero ? _frameInput.Move.normalized : new Vector2(transform.localScale.x, 0);
            _frameVelocity = _dashDirection * stats.dashPower;
            _isDashing = true;
            _dashStartTime = _time;
            _lastDashTime = _time;
            _dashUsed = true;
            
            dashParticles.Play();
            dashTrail.emitting = true;
            ScreenShake.Instance.ShakeCamera(10f, 0.2f);
            SoundManager.PlaySound(SoundType.PLAYER_DASH);
            
            float facingSign = Mathf.Sign(transform.localScale.x);
            // Stretch the player in dash direction
            transform.localScale = new Vector3(_originalScale.x * stats.stretchFactor * facingSign, _originalScale.y * stats.squashFactor, _originalScale.z);

            Dashed?.Invoke();
        }

        #endregion

        #region Movement

        private void HandleDirection()
        {
            if (_isDashing) return;

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? stats.groundFriction : stats.airDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * stats.maxSpeed, stats.acceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleGravity()
        {
            if (_isDashing) return;

            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = stats.groundingForce;
            }
            else
            {
                var inAirGravity = stats.fallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= stats.jumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -stats.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void HandleSpriteFlip()
        {
            if (_frameVelocity.x > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (_frameVelocity.x < -0.01f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        
        private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
        public bool DashDown;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public event Action Dashed;
        public Vector2 FrameInput { get; }
    }
}