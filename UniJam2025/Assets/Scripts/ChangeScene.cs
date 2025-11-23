using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 1f;

    private static ChangeScene existingInstance;
    private bool changing = false;

    private void Awake()
    {
        // If another instance exists, destroy this one
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        // This is the first/only instance
        existingInstance = this;
        DontDestroyOnLoad(gameObject);
    }

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
        Debug.Log($"Changing scene to {sceneName} : changing = {changing}");
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

