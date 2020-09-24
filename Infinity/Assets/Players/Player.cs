using Assets.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Players
{
    [DefaultExecutionOrder(-90)]
    public class Player : MonoBehaviour
    {
        public static Player Active { get; protected set; }

        public PlayerInputActions _inputActions;

        [Header("Components")]
        public Camera Camera;

        public CharacterController CharacterController;

        [Space]
        [Header("Move Settings")]
        public float MoveSpeed = 2f;

        [Tooltip("How much faster going forwards is than backwards or sideways")]
        public float ForwardBoost = 1.2f;

        public float SprintSpeed = 4f;

        public float SneakSlow = 5f;

        public float JumpPower = 0.25f;

        public float GravityFactor = 0.25f;

        [Space]
        [Header("Momentum")]
        public float AirResistance = 0.99f;
        public float Friction = 0.1f;

        public Vector3 Momentum;

        [Space]
        [Header("Look Settings")]
        public float MouseSensitivity = 10f;

        public bool LookLocked = false;

        public bool CursorLock;

        private Vector2 _lookInput;
        private Vector2 _moveInput;
        private float _sprint;
        private float _sneak;
        private bool _pressing;
        private bool _jumping;

        public Vector2 Orientation;

        [Range(-90, 90)]
        public float MinPitch = -80;

        [Range(-90, 90)]
        public float MaxPitch = 80;

        [Space, Tooltip("The target framerate. Set to 0 to disable.")]
        public int TargetFramerate = 0;

        [Space, Header("Debug")]
        public bool ShowDebugStats;

        public float SpeedUpdateRate;

        private Vector3 _lastPos;
        private float _dist;

        [Space, Header("World")]
        public World World;
        public int RenderDistance;

        public Transform HighlightedBlock;
        public GameObject Alex;
        public GameObject Justin;

        public void Awake()
        {
            if (TargetFramerate != 0)
            {
                QualitySettings.vSyncCount = 0;
            }
            Application.targetFrameRate = TargetFramerate;

            Active = this;
        }

        public void Start()
        {
            if (Camera == null) Camera = GetComponentInChildren<Camera>();
            if (CharacterController == null) CharacterController = GetComponentInChildren<CharacterController>();

            InitializeInputActions();

            if (CursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            var rotEuler = Camera.transform.rotation.eulerAngles;
            Orientation = new Vector2(rotEuler.y, rotEuler.x);
            if (Orientation.y > 180) Orientation.y -= 360;

            StartCoroutine(nameof(SpeedMeasuring));
        }

        private IEnumerator SpeedMeasuring()
        {
            while (true)
            {
                var pos = transform.position;
                _dist = Vector3.Distance(pos, _lastPos);
                _lastPos = pos;
                yield return new WaitForSeconds(SpeedUpdateRate);
            }
        }

        private void InitializeInputActions()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>() * MouseSensitivity / Screen.dpi;
            _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>() * MoveSpeed;
            _inputActions.Player.Sprint.performed += ctx => _sprint = ctx.ReadValue<float>();
            _inputActions.Player.Sneak.performed += ctx => _sneak = ctx.ReadValue<float>();
            _inputActions.Player.Click.started += _ => _pressing = true;
            _inputActions.Player.Click.canceled += _ => _pressing = false;
            _inputActions.Player.Jump.started += _ => _jumping = true;
            _inputActions.Player.Jump.canceled += _ => _jumping = false;
        }

        public void OnEnable()
        {
            if (_inputActions == null)
            {
                InitializeInputActions();
            }

            _inputActions.Player.Enable();

            Active = this;
        }

        public void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void Update()
        {
            if ((CursorLock || _pressing) && !LookLocked)
            {
                Orientation += _lookInput * Time.deltaTime;
                Orientation.y = Mathf.Clamp(Orientation.y, MinPitch, MaxPitch);
            }

            var ray = CursorLock ? new Ray(Camera.transform.position, Camera.transform.forward) : Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 5f))
            {
                var place = _sneak > 0.5f;

                Alex.SetActive(!place);
                Justin.SetActive(place);

                var pos = (hit.point - (hit.normal / 2 * (place ? -1 : 1))).ToVector3Int();

                HighlightedBlock.transform.position = pos;

                if (_pressing)
                {
                    World[pos] = place ? BlockTypes.Dirt : null;
                }
            }
            else
            {
                Alex.SetActive(false);
                Justin.SetActive(false);
            }
        }

        public void FixedUpdate()
        {
            var forwardMultiplier = _moveInput.y > 0 ? ForwardBoost * (1 + (_sprint * SprintSpeed)) : 1;
            var sneakMultiplier = 1f / (1f + (_sneak * SneakSlow));
            var forwardSpeed = _moveInput.y * forwardMultiplier * sneakMultiplier * Time.fixedDeltaTime;
            var sidewardSpeed = _moveInput.x * sneakMultiplier * Time.fixedDeltaTime;

            var yaw = Quaternion.AngleAxis(Orientation.x, Vector3.up);

            var jump = _jumping && CharacterController.isGrounded
                ? Vector3.up * JumpPower
                : Vector3.zero;

            Momentum *= AirResistance; // TODO: Account for different frame lengths
            Momentum +=
                (sidewardSpeed * (yaw * Vector3.right))
                + (forwardSpeed * (yaw * Vector3.forward))
                + jump
                + (Time.fixedDeltaTime * Physics.gravity * GravityFactor);

#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute.
            if ((CharacterController.Move(Momentum) & CollisionFlags.CollidedBelow) != 0)
#pragma warning restore RCS1130 // Bitwise operation on enum without Flags attribute.
            {
                Momentum.x *= Friction;
                Momentum.z *= Friction;
            }

            CharacterController.transform.rotation = yaw;

            Camera.transform.localRotation = Quaternion.AngleAxis(Orientation.y, Vector3.left);

            var currentChunkIndex = World.GetChunkIndex(CharacterController.transform.position.ToVector3Int());

            foreach (var chunk in World.Chunks.GetOrSpawnArea(currentChunkIndex, RenderDistance, wake: true)) _ = chunk;
        }

        public void OnDrawGizmos()
        {
            var ray = CursorLock ? new Ray(Camera.transform.position, Camera.transform.forward) : Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 5f))
            {
                Gizmos.DrawLine(hit.point, hit.point + hit.normal);
            }
        }

        public void OnGUI()
        {
            if (!ShowDebugStats) return;
            GUILayout.Label("Move: " + _moveInput);
            GUILayout.Label("Look: " + _lookInput);
            GUILayout.Label("Sprint: " + _sprint);
            GUILayout.Label("Units/s: " + (_dist / SpeedUpdateRate));
        }
    }
}