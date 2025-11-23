using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1f;

    private Animator animator;
    private SpriteRenderer sr;

    private static ChangeScene existingInstance;
    private bool changing = false;

    private void Awake()
    {
        // Singleton
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        existingInstance = this;
        DontDestroyOnLoad(gameObject);

        CacheComponents();
    }

    private void CacheComponents()
    {
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            // Force default sprite material
            sr.material = new Material(Shader.Find("Sprites/Default"));

            // Force sorting layer
            sr.sortingLayerName = "Rideaux";

            // Position in front of camera
            Vector3 pos = sr.transform.position;
            pos.z = Camera.main.transform.position.z + 5f; // devant la caméra
            sr.transform.position = pos;

            // Scale sprite to fill screen
            ScaleSpriteToScreen();
        }
    }

    public void Goto(string sceneName)
    {
        if (changing) return;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be empty!");
            return;
        }

        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneWithTransition(string sceneName)
    {
        changing = true;

        // Play close animation
        if (animator != null)
            animator.SetTrigger("Close");

        yield return new WaitForSeconds(transitionDuration);

        // Load next scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOp.isDone)
            yield return null;

        // After load, refresh references
        CacheComponents();

        // Play open animation
        if (animator != null)
            animator.SetTrigger("Open");

        changing = false;
    }

    private void ScaleSpriteToScreen()
    {
        if (sr == null || sr.sprite == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        float worldScreenHeight = cam.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        float scaleX = worldScreenWidth / spriteWidth;
        float scaleY = worldScreenHeight / spriteHeight;

        sr.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        PositionInFrontOfCamera();
    }

    private void PositionInFrontOfCamera()
    {
        if (sr == null) return;

        Camera cam = Camera.main;

        if (cam == null) return;

        Vector3 camPos = cam.transform.position;
        sr.transform.position = new Vector3(
            camPos.x,                // follow camera X
            camPos.y,                // follow camera Y
            camPos.z + 5f            // in front of camera
        );
    }

}

