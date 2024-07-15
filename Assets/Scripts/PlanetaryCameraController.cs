using UnityEngine;
using System.Collections;

public class PlanetaryCameraController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float initialMoveSpeed = 5f;
    public float fastMoveSpeed = 1000f;
    public float slowdownDistance = 1000f;
    public float zoomAdjustmentSpeed = 2f;
    public LayerMask planetLayer;
    public float zoomPadding = 1.1f;
    public bool debugMode = true;

    private Transform currentPlanet;
    private Transform targetPlanet;
    private Camera cam;
    private PlanetOrbitCamera orbitCamera;
    private Vector3 arrivalPosition;



    void Start()
    {
        cam = GetComponent<Camera>();
        orbitCamera = GetComponent<PlanetOrbitCamera>();

        currentPlanet = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, planetLayer))
            {
                MoveToPlanet(hit.transform);
            }
        }
    }

    public void MoveToPlanet(Transform target)
    {
        if (target == currentPlanet) return;

        targetPlanet = target;
        StopAllCoroutines();
        StartCoroutine(MoveToPlanetSequence());
    }

    IEnumerator MoveToPlanetSequence()
    {
        if (orbitCamera != null) orbitCamera.StopOrbiting();

        yield return StartCoroutine(RotateTowards(targetPlanet.position));
        yield return StartCoroutine(MoveTowardsPlanet());
        yield return StartCoroutine(AdjustZoom());
        
        currentPlanet = targetPlanet;
        arrivalPosition = transform.position;

        // Setelah selesai bergerak, aktifkan orbit camera
        if (orbitCamera != null)
        {
            orbitCamera.SetTargetPlanet(targetPlanet, arrivalPosition);
        }

    }

    IEnumerator RotateTowards(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveTowardsPlanet()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        float distance = Vector3.Distance(transform.position, targetPosition);
        float initialDistance = distance;

        while (distance > initialDistance * 0.1f)
        {
            float speed = distance > slowdownDistance ? fastMoveSpeed : initialMoveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }

        transform.position = targetPosition;
    }

    IEnumerator AdjustZoom()
    {
        float targetFOV = CalculateTargetFOV();
        while (Mathf.Abs(cam.fieldOfView - targetFOV) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomAdjustmentSpeed * Time.deltaTime);
            yield return null;
        }

        if (debugMode)
        {
            Debug.Log($"Final FOV: {cam.fieldOfView}");
            Debug.Log($"Final camera position: {transform.position}");
            Debug.Log($"Distance to planet center: {Vector3.Distance(transform.position, targetPlanet.position)}");
        }
    }

    float CalculateTargetFOV()
    {
        Renderer planetRenderer = targetPlanet.GetComponent<Renderer>();
        if (planetRenderer == null) return 60f;

        Bounds bounds = planetRenderer.bounds;
        float planetRadius = bounds.extents.magnitude;
        float distanceToSurface = Vector3.Distance(transform.position, targetPlanet.position) - planetRadius;

        float requiredFOV = 2f * Mathf.Atan(planetRadius / distanceToSurface) * Mathf.Rad2Deg;

        if (debugMode)
        {
            Debug.Log($"Planet radius: {planetRadius}");
            Debug.Log($"Distance to surface: {distanceToSurface}");
            Debug.Log($"Calculated FOV: {requiredFOV}");
        }

        return Mathf.Clamp(requiredFOV * zoomPadding, 10f, 60f);
    }

    Vector3 CalculateTargetPosition()
    {
        Renderer planetRenderer = targetPlanet.GetComponent<Renderer>();
        if (planetRenderer == null) return targetPlanet.position;

        Bounds bounds = planetRenderer.bounds;
        float planetRadius = bounds.extents.magnitude;
        float distanceFromCenter = planetRadius * 2f * zoomPadding;

        Vector3 directionToCamera = (transform.position - targetPlanet.position).normalized;
        Vector3 targetPosition = targetPlanet.position + directionToCamera * distanceFromCenter;

        if (debugMode)
        {
            Debug.Log($"Planet radius: {planetRadius}");
            Debug.Log($"Target distance from center: {distanceFromCenter}");
            Debug.Log($"Calculated target position: {targetPosition}");
        }

        return targetPosition;
    }
}