using UnityEngine;

public class SceneLoaderCallback : MonoBehaviour
{
	private bool _isFirstUpdate = true;

	// this update happens only once the scene is fully loaded
	private void Update()
	{
		if (_isFirstUpdate)
		{
			_isFirstUpdate = false;
			SceneLoader.LoaderCallback();
		}
	}
}