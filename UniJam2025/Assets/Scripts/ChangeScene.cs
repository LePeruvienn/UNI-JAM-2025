using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static ChangeScene Instance;

    [Header("Transition")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 1f;

    private bool changing = false;

    private void Awake()
    {
        // Make this a singleton and avoid duplicates on next scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this from anywhere: ChangeScene.Instance.Goto("Level2");
    /// </summary>
    public void Goto(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be empty!");
            return;
        }

        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneWithTransition(string sceneName)
    {
        if (changing)
            yield break;

        changing = true;

        // Play close animation
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Close");

        // Wait for animation to finish
        yield return new WaitForSeconds(transitionDuration);

        // Load next scene async
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOp.isDone)
            yield return null;

        // Play open animation
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Open");

        changing = false;
    }
}

