using UnityEngine;

public enum SimonState {

    Idle,
    Walking,
    Posing
}

public class Simon : MonoBehaviour
{
    [Header("Tempo settings")]
    [SerializeField] private float tempoDuration = 1f;

    [Header("Idle settings")]
    [SerializeField] private float minIdleDuration = 1f;
    [SerializeField] private float maxIdleDuration = 3f;

    [Header("Walk settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minWalkingDuration = 1f;
    [SerializeField] private float maxWalkingDuration = 3f;
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    [Header("Posing settings")]
    [SerializeField] private Color posingColor;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private SimonState state;

    private float idleTimer;
    private float walkTimer;

    private int direction = 1;
    private Color normalColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        normalColor = sr.color;

        ChangeState(SimonState.Walking);
    }

    private void Update()
    {
        switch (state)
        {
            case SimonState.Walking:
                HandleWalking();
                break;

            case SimonState.Idle:
                HandleIdle();
                break;
        }
    }

    private void HandleWalking()
    {

        // Boundaries
        if (transform.position.x > rightBoundary.position.x && direction == 1)
            direction = -1;
        else if (transform.position.x < leftBoundary.position.x && direction == -1)
            direction = 1;

        rb.linearVelocity = new Vector2(direction * speed, 0);

        walkTimer -= Time.deltaTime;

        if (walkTimer <= 0)
            ChangeState(SimonState.Idle);
    }

    private void HandleIdle()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
            ChangeState(SimonState.Walking);
    }

    private void PickRandomDirection()
    {
        direction = Random.value > 0.5f ? 1 : -1;
    }

    public void ChangeState(SimonState newState)
    {
        state = newState;

        switch (newState)
        {
            case SimonState.Walking:
                PickRandomDirection();
                sr.color = normalColor;
                walkTimer = Random.Range(minWalkingDuration, maxWalkingDuration);
                break;

            case SimonState.Idle:
                sr.color = normalColor;
                rb.linearVelocity = Vector2.zero;
                idleTimer = Random.Range(minIdleDuration, maxIdleDuration);
                break;

            case SimonState.Posing:
                sr.color = posingColor;
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    public SimonState GetSimonState()
    {
        return state;
    }
}
