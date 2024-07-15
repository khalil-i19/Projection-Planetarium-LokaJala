using UnityEngine;
using System.Collections;

public class CinematicPlanetaryExperience : MonoBehaviour
{
    public Transform[] planets;
    public Transform directionalLight;
    public float approachDuration = 10f;
    public float orbitDuration = 30f;
    public float transitionDuration = 5f;
    public AnimationCurve approachCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve orbitCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public HDRISkyController hDRISkyController;

    private Camera mainCamera;
    private int currentPlanetIndex = -1;
    private Coroutine currentSequence;

    private Vector3 lastApproachPosition;
    private Quaternion lastApproachRotation;
    private float lastFOV = 60f;

    void Start()
    {
        mainCamera = Camera.main;
        lastApproachPosition = mainCamera.transform.position;
        lastApproachRotation = mainCamera.transform.rotation;
        StartNextPlanetSequence();
    }

    void StartNextPlanetSequence()
    {
        if (currentSequence != null)
            StopCoroutine(currentSequence);

        hDRISkyController.Animate();
        currentPlanetIndex = (currentPlanetIndex + 1) % planets.Length;
        currentSequence = StartCoroutine(PlanetarySequence(planets[currentPlanetIndex]));
    }

    IEnumerator PlanetarySequence(Transform targetPlanet)
    {
        yield return StartCoroutine(ApproachPlanet(targetPlanet));
        yield return StartCoroutine(OrbitPlanet(targetPlanet));
        StartNextPlanetSequence();
    }

    IEnumerator ApproachPlanet(Transform targetPlanet)
    {
        Vector3 startPosition = lastApproachPosition;
        Quaternion startRotation = lastApproachRotation;

        Vector3 directionToPlanet = targetPlanet.position - directionalLight.position;
        directionalLight.rotation = Quaternion.LookRotation(directionToPlanet);

        Renderer planetRenderer = targetPlanet.GetComponent<Renderer>();
        if (planetRenderer == null)
        {
            Debug.LogError("Planet does not have a Renderer component!");
            yield break;
        }

        Bounds planetBounds = planetRenderer.bounds;
        float planetSize = planetBounds.extents.magnitude;

        Vector3 approachPosition = targetPlanet.position - targetPlanet.forward * planetSize * 1.2f;
        Quaternion approachRotation = Quaternion.LookRotation(targetPlanet.position - approachPosition);

        float elapsedTime = 0f;
        while (elapsedTime < approachDuration)
        {
            float t = approachCurve.Evaluate(elapsedTime / approachDuration);
            mainCamera.transform.position = Vector3.Lerp(startPosition, approachPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, approachRotation, t);

            // float startFOV = lastFOV;
            // float targetFOV = Mathf.Clamp(60f * (planetSize / 10f), 30f, 90f);
            // mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Store the last position and rotation for smooth transition to orbit
        lastApproachPosition = mainCamera.transform.position;
        lastApproachRotation = mainCamera.transform.rotation;
    }

    IEnumerator OrbitPlanet(Transform targetPlanet)
    {
        Renderer planetRenderer = targetPlanet.GetComponent<Renderer>();
        if (planetRenderer == null)
        {
            Debug.LogError("Planet does not have a Renderer component!");
            yield break;
        }

        Bounds planetBounds = planetRenderer.bounds;
        float planetSize = planetBounds.extents.magnitude;

        Vector3 orbitCenter = targetPlanet.position;
        float orbitRadius = planetSize * 3f;

        float elapsedTime = 0f;
        Vector3 startPosition = lastApproachPosition;
        Quaternion startRotation = lastApproachRotation;

        while (elapsedTime < orbitDuration)
        {
            float t = orbitCurve.Evaluate(elapsedTime / orbitDuration);

            // Smooth orbit movement
            float angle = t * 360f;
            Vector3 orbitPosition = orbitCenter + Quaternion.Euler(0, angle, 0) * (startPosition - orbitCenter);

            // Add slight up and down movement
            float verticalOffset = Mathf.Sin(t * Mathf.PI * 2) * planetSize * 0.2f;
            orbitPosition += Vector3.up * verticalOffset;

            // // Add slight in and out movement
            // float radiusOffset = Mathf.Sin(t * Mathf.PI * 4) * planetSize * 0.1f;
            // orbitPosition += (orbitPosition - orbitCenter).normalized * radiusOffset;

            mainCamera.transform.position = orbitPosition;
            mainCamera.transform.LookAt(targetPlanet);

            // // Adjust FOV slightly for dynamic feel
            // float baseFOV = Mathf.Clamp(60f * (planetSize / 10f), 30f, 90f);
            // float dynamicFOV = baseFOV + Mathf.Sin(t * Mathf.PI * 6) * 5f;
            // mainCamera.fieldOfView = dynamicFOV;

            elapsedTime += Time.deltaTime;
            // lastFOV = dynamicFOV;
            yield return null;
        }

        lastApproachPosition = mainCamera.transform.position;
        lastApproachRotation = mainCamera.transform.rotation;
    }

    public void SkipToNextPlanet()
    {
        StartNextPlanetSequence();
    }
}
