using UnityEngine;
using System.Collections;

public class PlanetOrbitCamera : MonoBehaviour
{
    public float orbitSpeed = 10f; // Kecepatan orbit dalam derajat per detik
    private float orbitRadius;
    public float verticalTilt = 30f; // Kemiringan vertikal orbit dalam derajat

    private Transform targetPlanet;
    private float currentAngle = 0f;
    private bool isOrbiting = false;
    private Vector3 initialOffset;

    public void SetTargetPlanet(Transform newTarget, Vector3 arrivalPosition)
    {
        targetPlanet = newTarget;
        initialOffset = arrivalPosition - targetPlanet.position;
        ResetOrbit();
        StartCoroutine(StartOrbiting());
    }

    void ResetOrbit()
    {
        if (targetPlanet != null)
        {
            orbitRadius = initialOffset.magnitude;
            currentAngle = Mathf.Atan2(initialOffset.z, initialOffset.x) * Mathf.Rad2Deg;
            verticalTilt = Mathf.Asin(initialOffset.y / orbitRadius) * Mathf.Rad2Deg;
            UpdateCameraPosition();
        }
    }

    IEnumerator StartOrbiting()
    {
        yield return new WaitForSeconds(0.5f); // Tunggu sebentar setelah zoom selesai
        isOrbiting = true;
    }

    void Update()
    {
        if (targetPlanet == null || !isOrbiting) return;

        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        if (targetPlanet == null) return;

        float horizontalRadius = orbitRadius * Mathf.Cos(verticalTilt * Mathf.Deg2Rad);
        float height = orbitRadius * Mathf.Sin(verticalTilt * Mathf.Deg2Rad);

        Vector3 orbitPosition = new Vector3(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad) * horizontalRadius,
            height,
            Mathf.Sin(currentAngle * Mathf.Deg2Rad) * horizontalRadius
        );

        transform.position = targetPlanet.position + orbitPosition;
        transform.LookAt(targetPlanet);
    }

    public void StopOrbiting()
    {
        isOrbiting = false;
    }
}