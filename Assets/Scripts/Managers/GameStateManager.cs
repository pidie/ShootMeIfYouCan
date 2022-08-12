using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
	    [SerializeField] private Texture2D cursorImage;
	 
	    private void Awake() => Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);

	    public void Retry() => SceneLoader.Load(SceneLoader.Scene.TestLevel);

	    public void ToTitleScreen() => SceneLoader.Load(SceneLoader.Scene.TitleScene);

	    public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}