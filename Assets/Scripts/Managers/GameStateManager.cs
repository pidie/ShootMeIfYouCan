using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
	    [SerializeField] private Texture2D cursorImage;
	 
	    private void Awake() => Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);

	    public void Retry() => SceneManager.LoadScene(1);

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