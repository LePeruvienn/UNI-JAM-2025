using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
    // Propriétés existantes
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1f;

    // Nouveaux paramètres demandés
    [Header("Shake Parameters")]
    public float speed = 50f;
    public float spacing = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float globalStrength = curve.Evaluate(elapsedTime / duration);

            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * 2f - 1f) * spacing * globalStrength;
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * 2f - 1f) * spacing * globalStrength;

            float z = (Mathf.PerlinNoise(Time.time * speed, Time.time * speed) * 2f - 1f) * spacing * globalStrength;

            transform.position = initialPosition + new Vector3(x, y, z);

            yield return null;
        }

        // Remise à la position initiale pour s'assurer qu'il n'y a pas de décalage résiduel
        transform.position = startPosition;
    }
}
