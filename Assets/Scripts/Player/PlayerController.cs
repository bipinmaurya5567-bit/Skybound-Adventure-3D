using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SkyboundAdventure
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float runSpeed = 7.2f;
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private float acceleration = 22f;
        [SerializeField] private float deceleration = 26f;

        [Header("Jumping & Gravity")]
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.12f;
        [SerializeField] private float airControlMultiplier = 0.9f;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayers = ~0;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Visual Feedback & Limbs")]
        [SerializeField] private Transform modelTransform;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;

        private CharacterController controller;
        private Vector3 currentVelocity;
        private float verticalVelocity;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private bool isGrounded;
        private bool isRunning;
        private bool movementEnabled = true;

        // Procedural Animation & Squash/Stretch State
        private Vector3 targetModelScale = Vector3.one;
        private float runCycleTime;

        public bool IsGrounded => isGrounded;
        public bool IsRunning => isRunning;
        public Vector3 Velocity => controller != null ? controller.velocity : Vector3.zero;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            // Setup CharacterController capsule matching character proportions (scaled 2x)
            controller.height = 2.6f;
            controller.radius = 0.45f;
            controller.center = new Vector3(0f, 1.3f, 0f);
            controller.skinWidth = 0.04f;
            controller.stepOffset = 0.45f;
            controller.slopeLimit = 45f;

            if (modelTransform == null && transform.childCount > 0)
            {
                modelTransform = transform.Find("Model") ?? transform.GetChild(0);
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            // Exclude Player and Triggers from ground check
            groundLayers &= ~LayerMask.GetMask("Player", "Ignore Raycast", "TransparentFX");
        }

        private void Start()
        {
            SnapToGroundSurface();
        }

        /// <summary>
        /// Snaps the player firmly to the platform surface so they never start floating in mid-air.
        /// </summary>
        public void SnapToGroundSurface()
        {
            Physics.SyncTransforms();
            bool wasEnabled = (controller != null && controller.enabled);
            if (controller != null) controller.enabled = false;

            // Search for ground below player, strictly ignoring the player itself
            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 1.5f, Vector3.down, 12.0f, groundLayers, QueryTriggerInteraction.Ignore);
            RaycastHit bestHit = default;
            float highestGroundY = float.MinValue;
            bool found = false;

            foreach (var h in hits)
            {
                if (h.transform == transform || h.transform.IsChildOf(transform)) continue;
                if (h.collider.isTrigger) continue;

                if (h.point.y > highestGroundY)
                {
                    highestGroundY = h.point.y;
                    bestHit = h;
                    found = true;
                }
            }

            if (found)
            {
                transform.position = bestHit.point;
                verticalVelocity = -2.0f;
                isGrounded = true;
            }

            if (controller != null) controller.enabled = wasEnabled;
        }

        private void Update()
        {
            if (!movementEnabled)
            {
                ApplyGravityOnly();
                UpdateProceduralAnimation(Vector3.zero, false, false);
                UpdateAnimator(Vector3.zero, isGrounded);
                return;
            }

            CheckGrounded();
            HandleInput();
            MoveCharacter();
            CheckCollectiblesAndTriggers();
            UpdateProceduralAnimation(currentVelocity, isGrounded, isRunning);
            UpdateAnimator(currentVelocity, isGrounded);
        }

        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = controller.isGrounded;

            // Downward check for edge and slope stability
            if (!isGrounded && verticalVelocity <= 0.1f)
            {
                Vector3 spherePos = transform.position + Vector3.down * 0.05f;
                Collider[] hits = Physics.OverlapSphere(spherePos, controller.radius * 0.9f, groundLayers, QueryTriggerInteraction.Ignore);
                foreach (var col in hits)
                {
                    if (col.transform == transform || col.transform.IsChildOf(transform) || col.isTrigger) continue;
                    isGrounded = true;
                    break;
                }
            }

            if (isGrounded)
            {
                coyoteTimer = coyoteTime;
                if (!wasGrounded && verticalVelocity < -2.0f)
                {
                    // Gentle landing squash
                    TriggerSquash(1.22f, 0.78f);
                }
            }
            else
            {
                coyoteTimer -= Time.deltaTime;
            }

            jumpBufferTimer -= Time.deltaTime;
        }

        private void HandleInput()
        {
            Vector2 input = ReadMovementInput();
            isRunning = ReadRunInput();

            if (ReadJumpInputThisFrame())
            {
                jumpBufferTimer = jumpBufferTime;
            }

            if (ReadBackflipInputThisFrame())
            {
                ExecuteBackflip();
            }

            // Fixed level-relative movement:
            // W moves forward (+Z), S moves backward (-Z)
            // A moves left (-X), D moves right (+X)
            Vector3 targetDir = new Vector3(input.x, 0f, input.y);
            if (targetDir.sqrMagnitude > 1f) targetDir.Normalize();

            float targetSpeed = isRunning ? runSpeed : walkSpeed;
            Vector3 targetVelocity = targetDir * targetSpeed;

            float currentAccel = (targetDir.sqrMagnitude > 0.01f) ? acceleration : deceleration;
            if (!isGrounded) currentAccel *= airControlMultiplier;

            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, currentAccel * Time.deltaTime);

            // Responsive jump with coyote time and buffer
            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                ExecuteJump();
            }
        }

        private void ExecuteJump()
        {
            verticalVelocity = Mathf.Sqrt(2f * jumpHeight * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
            else
            {
                TriggerSquash(0.82f, 1.25f); // Jump stretch
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
        }

        public void ExecuteBackflip()
        {
            if (animator != null)
            {
                animator.SetTrigger("Backflip");
            }

            // Acrobatic hop on ground
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(2f * (jumpHeight * 0.75f) * gravity);
                isGrounded = false;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
        }

        private void MoveCharacter()
        {
            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2.0f; // Firm ground adherence without excessive accumulation
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
                verticalVelocity = Mathf.Max(verticalVelocity, -25f); // Terminal velocity
            }

            Vector3 finalMove = currentVelocity;
            finalMove.y = verticalVelocity;

            controller.Move(finalMove * Time.deltaTime);

            // Smooth character rotation towards movement direction
            Vector3 horizontalMove = currentVelocity;
            horizontalMove.y = 0f;
            if (horizontalMove.sqrMagnitude > 0.04f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalMove.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void ApplyGravityOnly()
        {
            verticalVelocity -= gravity * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -25f);
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        private Vector2 ReadMovementInput()
        {
            Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            }

            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.sqrMagnitude > 0.04f) input = stick;
            }
#else
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
#endif
            if (input.sqrMagnitude > 1f) input.Normalize();
            return input;
        }

        private bool ReadRunInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed) return true;
            }
            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftTrigger.isPressed || Gamepad.current.rightShoulder.isPressed) return true;
            }
            return false;
#else
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
        }

        private bool ReadJumpInputThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
            return false;
#else
            return Input.GetButtonDown("Jump");
#endif
        }

        private bool ReadBackflipInputThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) return true;
            if (Gamepad.current != null && (Gamepad.current.buttonWest.wasPressedThisFrame || Gamepad.current.buttonNorth.wasPressedThisFrame)) return true;
            return false;
#else
            return Input.GetKeyDown(KeyCode.R);
#endif
        }

        private void TriggerSquash(float horizontalScale, float verticalScale)
        {
            targetModelScale = new Vector3(horizontalScale, verticalScale, horizontalScale);
        }

        private void UpdateProceduralAnimation(Vector3 horizVel, bool grounded, bool running)
        {
            if (animator != null)
            {
                if (modelTransform != null)
                {
                    modelTransform.localPosition = Vector3.zero;
                    modelTransform.localScale = Vector3.one;
                }
                return;
            }

            if (modelTransform == null) return;

            // Spring scale back to (1,1,1)
            targetModelScale = Vector3.MoveTowards(targetModelScale, Vector3.one, 5.0f * Time.deltaTime);
            modelTransform.localScale = Vector3.Lerp(modelTransform.localScale, targetModelScale, 18f * Time.deltaTime);

            float speed = horizVel.magnitude;

            if (grounded && speed > 0.2f)
            {
                float cycleSpeed = running ? 12f : 8.5f;
                runCycleTime += Time.deltaTime * cycleSpeed;

                float swing = Mathf.Sin(runCycleTime) * (running ? 36f : 24f);
                float bounce = Mathf.Abs(Mathf.Sin(runCycleTime * 2f)) * (running ? 0.08f : 0.04f);

                modelTransform.localPosition = new Vector3(0f, bounce, 0f);

                if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(swing, 0f, 0f);
                if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-swing, 0f, 0f);
                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(-swing * 0.8f, 0f, 0f);
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(swing * 0.8f, 0f, 0f);
            }
            else if (!grounded)
            {
                if (verticalVelocity < -6f)
                {
                    targetModelScale = new Vector3(0.92f, 1.10f, 0.92f);
                }

                if (leftLeg != null) leftLeg.localRotation = Quaternion.Slerp(leftLeg.localRotation, Quaternion.Euler(-15f, 0f, 0f), 8f * Time.deltaTime);
                if (rightLeg != null) rightLeg.localRotation = Quaternion.Slerp(rightLeg.localRotation, Quaternion.Euler(-15f, 0f, 0f), 8f * Time.deltaTime);
                if (leftArm != null) leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, Quaternion.Euler(20f, 0f, 0f), 8f * Time.deltaTime);
                if (rightArm != null) rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, Quaternion.Euler(20f, 0f, 0f), 8f * Time.deltaTime);
            }
            else
            {
                // Idle breathing bob
                float idleBob = Mathf.Sin(Time.time * 2.5f) * 0.025f;
                modelTransform.localPosition = new Vector3(0f, idleBob, 0f);

                if (leftLeg != null) leftLeg.localRotation = Quaternion.Slerp(leftLeg.localRotation, Quaternion.identity, 12f * Time.deltaTime);
                if (rightLeg != null) rightLeg.localRotation = Quaternion.Slerp(rightLeg.localRotation, Quaternion.identity, 12f * Time.deltaTime);
                if (leftArm != null) leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, Quaternion.identity, 12f * Time.deltaTime);
                if (rightArm != null) rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, Quaternion.identity, 12f * Time.deltaTime);
            }
        }

        private void UpdateAnimator(Vector3 horizVel, bool grounded)
        {
            if (animator == null) return;

            Vector3 flatVel = horizVel;
            flatVel.y = 0f;
            float speed = flatVel.magnitude;

            animator.SetFloat("Speed", speed);
            animator.SetBool("IsGrounded", grounded);
        }

        public void SetMovementEnabled(bool enabled)
        {
            movementEnabled = enabled;
            if (!enabled)
            {
                currentVelocity = Vector3.zero;
            }
        }

        public void ResetToSpawn(Vector3 position, Quaternion rotation)
        {
            if (controller != null) controller.enabled = false;

            transform.position = position;
            transform.rotation = rotation;
            currentVelocity = Vector3.zero;
            verticalVelocity = 0f;
            targetModelScale = Vector3.one;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetBool("IsGrounded", true);
                animator.ResetTrigger("Jump");
            }

            if (controller != null) controller.enabled = true;
            SnapToGroundSurface();
            SetMovementEnabled(true);
        }

        public void Bounce(float bounceHeight = 5.2f)
        {
            verticalVelocity = Mathf.Sqrt(2f * bounceHeight * gravity);
            isGrounded = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
            else
            {
                TriggerSquash(0.70f, 1.40f);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBounce();
            }
        }

        public void CheckCollectiblesAndTriggers()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 1.3f, 1.35f, ~0, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                if (col == null || col.transform == transform || col.transform.IsChildOf(transform)) continue;

                var coin = col.GetComponent<Coin>() ?? col.GetComponentInParent<Coin>();
                if (coin != null)
                {
                    coin.Collect();
                    continue;
                }

                var pad = col.GetComponent<BouncePad>();
                if (pad != null)
                {
                    pad.OnPlayerBounce(this);
                    continue;
                }

                var dis = col.GetComponent<DisappearingPlatform>();
                if (dis != null)
                {
                    dis.OnPlayerStepped();
                    continue;
                }

                var cp = col.GetComponent<Checkpoint>();
                if (cp != null)
                {
                    cp.ActivateCheckpoint();
                    continue;
                }

                var goal = col.GetComponent<LevelGoal>();
                if (goal != null)
                {
                    goal.TriggerGoal();
                    continue;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var coin = other.GetComponent<Coin>() ?? other.GetComponentInParent<Coin>();
            if (coin != null)
            {
                coin.Collect();
                return;
            }

            var pad = other.GetComponent<BouncePad>();
            if (pad != null)
            {
                pad.OnPlayerBounce(this);
                return;
            }

            var dis = other.GetComponent<DisappearingPlatform>();
            if (dis != null)
            {
                dis.OnPlayerStepped();
                return;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var dis = hit.collider.GetComponent<DisappearingPlatform>();
            if (dis != null && hit.normal.y > 0.5f)
            {
                dis.OnPlayerStepped();
            }

            var pad = hit.collider.GetComponent<BouncePad>();
            if (pad != null && hit.normal.y > 0.5f)
            {
                pad.OnPlayerBounce(this);
            }
        }

        public void SetupLimbReferences(Transform model, Transform lLeg, Transform rLeg, Transform lArm, Transform rArm)
        {
            modelTransform = model;
            leftLeg = lLeg;
            rightLeg = rLeg;
            leftArm = lArm;
            rightArm = rArm;
        }

        public void SetAnimator(Animator anim)
        {
            animator = anim;
        }

        public Animator GetAnimator()
        {
            return animator;
        }
    }
}
