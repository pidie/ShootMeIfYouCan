using System;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
	public enum Scene
	{
		TitleScene,
		LoadingScene,
		TestLevel,
	}

	private static Action _onLoaderCallback;
	
	public static void Load(Scene scene)
	{
		// store the scene we want to load
		_onLoaderCallback = () => SceneManager.LoadScene(scene.ToString());

		// start loading the "Loading..." screen
		SceneManager.LoadScene(Scene.LoadingScene.ToString());
	}

	// this method is called by the SceneLoaderCallback script
	public static void LoaderCallback()
	{
		if (_onLoaderCallback != null)
		{
			_onLoaderCallback.Invoke();
			_onLoaderCallback = null;
		}
	}
}