using UnityEngine;

public class ShotBehavior : MonoBehaviour
{
	[SerializeField] private float projectileSpeed = 1000f;

	private void Update () => transform.position += transform.forward * (Time.deltaTime * projectileSpeed);
}
