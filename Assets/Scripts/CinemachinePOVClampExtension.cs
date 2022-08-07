using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class CinemachinePOVClampExtension : CinemachineExtension
{
    [SerializeField] private float clampAngle;
    // [SerializeField] private float horizontalSpeed = 10;
    // [SerializeField] private float verticalSpeed = 10;
    
    // private InputManager _inputManager;
    private Vector3 _startingRotation;
    
    // protected override void Awake()
    // {
    //     _inputManager = InputManager.Instance;
    //     base.Awake();
    // }
    
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (_startingRotation == Vector3.zero)
                    _startingRotation = transform.localRotation.eulerAngles;
                // var deltaInput = _inputManager.GetMouseDelta();
                // _startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                // _startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                _startingRotation.y = Mathf.Clamp(_startingRotation.y, -clampAngle, clampAngle);
                // state.RawOrientation = quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
    }
}
