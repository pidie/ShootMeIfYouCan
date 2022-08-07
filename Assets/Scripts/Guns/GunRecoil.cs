using System;
using Guns;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunRecoil : MonoBehaviour
{
	[SerializeField] private float snappiness;
	[SerializeField] private float returnSpeed;
	[SerializeField] private GameObject gunModel;

	private Quaternion _initialRotation;
	private Vector3 _currentRotation;
	private Vector3 _targetRotation;
	private float _recoilX;
	private float _recoilY;
	private float _recoilZ;

	public static Action onApplyRecoil;

	private void Awake()
	{
		var recoil = GetComponent<GunBehavior>().GetRecoil();
		
		_recoilX = recoil.x;
		_recoilY = recoil.y;
		_recoilZ = recoil.z;

		onApplyRecoil += ApplyRecoil;

		_initialRotation = gunModel.transform.rotation;
	}

	private void Update()
	{
		_targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		_currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, snappiness * Time.fixedDeltaTime);
		gunModel.transform.localRotation = Quaternion.Euler(_currentRotation) * _initialRotation;
	}

	private void ApplyRecoil()
	{
		_targetRotation += new Vector3(_recoilX, Random.Range(-_recoilY, _recoilY), Random.Range(-_recoilZ, _recoilZ));
	}
}