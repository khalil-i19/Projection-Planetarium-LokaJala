using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CalculateAzimuttAltitude : MonoBehaviour
{
    public float ra;
    public float dec;
    public Transform starReference;
    public TMP_Text azimuthText;
    public TMP_Text altitudeText;
    Camera camera;

    void Start()
    {
        camera = Camera.main;

    }

    void Update()
    {
        if (!starReference) return;

        Calculate(starReference.position, out float azimuth, out float altitude);

        // Convert azimuth and altitude to DMS format
        string azimuthDMS = ConvertToDMS(azimuth);
        string altitudeDMS = ConvertToDMS(altitude);


        azimuthText.text = azimuthDMS;
        altitudeText.text = altitudeDMS;
    }
    void Calculate(Vector3 starWorldPosition, out float azimuth, out float altitude)
    {
        // Convert world position to camera-relative position
        Vector3 cameraRelativePosition = camera.transform.InverseTransformPoint(starWorldPosition);

        // Normalize the vector
        Vector3 normPos = cameraRelativePosition.normalized;

        // Altitude calculation
        altitude = Mathf.Asin(normPos.y) * Mathf.Rad2Deg;

        // Azimuth calculation
        azimuth = Mathf.Atan2(normPos.x, normPos.z) * Mathf.Rad2Deg;

        // Adjust azimuth to 0-360 degrees
        if (azimuth < 0)
            azimuth += 360f;
    }

    private string ConvertToDMS(float decimalDegrees)
    {
        int degrees = Mathf.FloorToInt(decimalDegrees);
        float minutesFloat = (decimalDegrees - degrees) * 60f;
        int minutes = Mathf.FloorToInt(minutesFloat);
        float seconds = (minutesFloat - minutes) * 60f;

        return $"{degrees}°{minutes}'{seconds:0.0}\"";
    }
}
