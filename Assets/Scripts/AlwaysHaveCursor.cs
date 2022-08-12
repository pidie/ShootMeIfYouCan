using UnityEngine;

public class AlwaysHaveCursor : MonoBehaviour
{
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}