using System;
using UnityEngine;

/// <summary>
/// Camera controller driven by player input.
/// </summary>
public sealed class CameraController : MonoBehaviour
{
    #region Local POCOs
    [Serializable]
    private struct RendererColliderPair
    {
        public MeshRenderer renderer;
        public Collider collider;
        [HideInInspector] public float currentOpacity;
    }
    #endregion
    #region Private Fields
    private Ray downRay;
    private Vector3 sensitivity;
    private Vector3 inversion;
    private float pivotAngle;
    private float zoomDistance;
    private float verticalAngle;
    // TODO this should be retrieved in a better way.
    private GameObject player;
    #endregion
    #region Inspector Fields
    [Header("Ceiling Parameters")]
    [Tooltip("Contains renderer-collider pairs for each ceiling that can be occluded.")]
    [SerializeField] private RendererColliderPair[] ceilingPairs = null;
    [Tooltip("Directly controls the speed at which ceilings fade in and out.")]
    [SerializeField] private float fadeSpeed = 1f;
    [Header("Camera Parameters")]
    [Tooltip("Restrains the angle against the ground that the camera can move along.")]
    [SerializeField] private Range pitchRange = new Range { min = 45f, max = 80f };
    [Tooltip("Restrains the distance the camera can zoom in and out.")]
    [SerializeField] private Range zoomRange = new Range { min = 4f, max = 12f };
    private void OnValidate()
    {
        // Clamp fields to sane values.
        fadeSpeed = Mathf.Clamp(fadeSpeed, 0.001f, float.MaxValue);
        if (pitchRange.max < pitchRange.min)
            pitchRange.max = pitchRange.min;
        if (zoomRange.max < zoomRange.min)
            zoomRange.max = zoomRange.min;
    }
    #endregion
    #region Initialization (Start)
    private void Start()
    {
        // Initialize ceiling check ray.
        downRay = new Ray(Vector3.zero, Vector3.down);
        // Set initial camera state.
        zoomDistance = zoomRange.Average;
        verticalAngle = pitchRange.Average;

        player = GameObject.FindWithTag("Player");

        // TODO once settings writes to sensitivity,
        // remove these lines and uncomment following lines.
        sensitivity = Vector3.one * 600f;
        inversion = Vector3.one;
        /*
        // Set intial sensitivity from settings.
        sensitivity = new Vector3
        {
            x = SettingsState.SensitivityRevolve,
            y = SettingsState.SensitivityPitch,
            z = SettingsState.SensitivityZoom
        };
        inversion = new Vector3
        {
            x = SettingsState.InvertRevolve? -1f : 1f,
            y = SettingsState.InvertPitch? -1f : 1f,
            z = SettingsState.InvertZoom? -1f : 1f
        };
        // Subscribe to settings changes.
        SettingsState.SensitivityRevolveChanged += (float newValue) =>
        { sensitivity.x = newValue; };
        SettingsState.SensitivityPitchChanged += (float newValue) =>
        { sensitivity.y = newValue; };
        SettingsState.SensitivityZoomChanged += (float newValue) =>
        { sensitivity.z = newValue; };
        SettingsState.InvertRevolveChanged += (bool newValue) =>
        { inversion.x = newValue ? -1f : 1f; };
        SettingsState.InvertPitchChanged += (bool newValue) =>
        { inversion.y = newValue ? -1f : 1f; };
        SettingsState.InvertZoomChanged += (bool newValue) =>
        { inversion.z = newValue ? -1f : 1f; };
        */
    }
    #endregion
    #region Update Logic
    private void Update()
    {
        UpdateInput();
        UpdateCeilings();
    }
    private void UpdateInput()
    {
        // If the right mouse button is held down:
        if (Input.GetMouseButton((int)MouseButton.Right))
        {
            // Retrieve new movement from mouse.
            pivotAngle += Time.deltaTime * sensitivity.x * inversion.x * Input.GetAxis("Mouse X");
            verticalAngle += Time.deltaTime * sensitivity.y * inversion.y * Input.GetAxis("Mouse Y");
            zoomDistance += Time.deltaTime * sensitivity.z * inversion.z * Input.GetAxis("Mouse ScrollWheel");
            // Restrain added movement.
            verticalAngle = Mathf.Clamp(verticalAngle, pitchRange.min, pitchRange.max);
            zoomDistance = Mathf.Clamp(zoomDistance, zoomRange.min, zoomRange.max);
        }
        // Update camera rotation and positioning.
        transform.parent.rotation = Quaternion.AngleAxis(pivotAngle, Vector3.up)
            * Quaternion.AngleAxis(verticalAngle, Vector3.right);
        transform.localPosition = Vector3.back * zoomDistance;
        // Make the camera look at the player.
        transform.LookAt(transform.parent.position);
    }
    private void UpdateCeilings()
    {
        // TODO remove use of magic numbers here.
        // Define a ray that will cast down upon the player position.
        downRay.origin = player.transform.position + Vector3.up * 20f;
        // Iterate through each ceiling pair.
        for (int i = 0; i < ceilingPairs.Length; i++)
        {
            RendererColliderPair pair = ceilingPairs[i];
            // If the raycast hits the ceiling mesh from above:
            if (pair.collider.Raycast(downRay, out RaycastHit _, 20f))
            {
                // Fade away the ceiling opacity.
                pair.currentOpacity -= Time.deltaTime * fadeSpeed;
                if (pair.currentOpacity < 0f)
                    pair.currentOpacity = 0f;
            }
            else
            {
                // Otherwise fade in the ceiling opacity.
                pair.currentOpacity += Time.deltaTime * fadeSpeed;
                if (pair.currentOpacity > 1f)
                    pair.currentOpacity = 1f;
            }
            ceilingPairs[i] = pair;
            // Update the transparency shader.
            pair.renderer.material.SetFloat("_Opacity", pair.currentOpacity);
        }
    }
    #endregion
}
