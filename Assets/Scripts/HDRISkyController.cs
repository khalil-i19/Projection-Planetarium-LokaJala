using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class HDRISkyController : MonoBehaviour
{
    public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0.1f, 10, 1.0f);
    public float animationDuration = 10.0f;

    private Volume volume;
    private HDRISky hdriSky;
    private float initialIntensity;
    private bool isAnimating = false;
    private float elapsedTime = 0f;

    void Start()
    {
        // Find the Volume component on the same GameObject or in the scene
        volume = GetComponent<Volume>();
        if (volume == null)
        {
            Debug.LogError("Volume component not found!");
            return;
        }

        // Try to get the HDRISky component from the Volume profile
        if (volume.profile.TryGet<HDRISky>(out hdriSky))
        {
            // Store the initial intensity multiplier value
            initialIntensity = hdriSky.multiplier.value;
        }
        else
        {
            Debug.LogError("HDRISky component not found in the Volume profile!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && !isAnimating)
        {
            // Start the animation
            isAnimating = true;
            elapsedTime = 0f;
        }

        if (isAnimating)
        {
            isAnimating = true;
            AnimateIntensity();
        }
    }
    public void Animate()
    {

        isAnimating = true;
    }
    private void AnimateIntensity()
    {
        if (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            float newIntensity = intensityCurve.Evaluate(t);
            hdriSky.multiplier.value = Mathf.Lerp(initialIntensity, newIntensity, t);
        }
        else
        {
            // Animation finished, reset values
            hdriSky.multiplier.value = initialIntensity;
            isAnimating = false;
        }
    }

    // Reset intensity when exiting play mode
    private void OnDisable()
    {
        if (hdriSky != null)
        {
            hdriSky.multiplier.value = initialIntensity;
        }
    }
}
