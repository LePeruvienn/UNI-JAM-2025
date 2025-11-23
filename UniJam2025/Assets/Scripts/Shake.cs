using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
    // Propriétés existantes
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1f; // conservé pour compatibilité mais non utilisé pour la boucle permanente

    // Nouveaux paramètres demandés
    [Header("Shake Parameters")]
    public float speed = 50f;
    public float spacing = 0.1f;

    // état interne
    private Coroutine _shakeRoutine;
    private Vector3 _originalPosition;

    void Start()
    {
        _originalPosition = transform.position;
        // si start est déjà true dans l'inspecteur, on démarre le shake
        if (start && _shakeRoutine == null)
            _shakeRoutine = StartCoroutine(Shaking());
    }

    // Update is called once per frame
    void Update()
    {
        if (start && _shakeRoutine == null)
        {
            _shakeRoutine = StartCoroutine(Shaking());
        }
        else if (!start && _shakeRoutine != null)
        {
            StopShake();
        }
    }

    public void StopShake()
    {
        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
            _shakeRoutine = null;
            transform.position = _originalPosition;
        }
    }

    public void StartShake()
    {
        if (_shakeRoutine == null)
        {
            start = true;
            _shakeRoutine = StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 initialPosition = _originalPosition;

        // boucle infinie : shake permanent jusqu'à StopShake() ou start = false
        while (true)
        {
            // utilise la courbe en boucle (PingPong) pour obtenir une force oscillante
            float t = Mathf.PingPong(Time.time, 1f);
            float globalStrength = (curve != null) ? curve.Evaluate(t) : 1f;

            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * 2f - 1f) * spacing * globalStrength;
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * 2f - 1f) * spacing * globalStrength;
            float z = (Mathf.PerlinNoise(Time.time * speed, Time.time * speed) * 2f - 1f) * spacing * globalStrength;

            transform.position = initialPosition + new Vector3(x, y, z);

            yield return null;
        }
    }
}
