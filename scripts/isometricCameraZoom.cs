using UnityEngine;

public class isometricCameraZoom : MonoBehaviour
{   
    public float zoomSpeed = 6;
    public float zoomSmoothness = 5;

    public float minZoom = 2;
    public float maxZoom = 40;

    private float _currentZoom;

    private Camera _camera;

    private void Awake() {
        _camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        _currentZoom = Mathf.Clamp(_currentZoom - Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, zoomSmoothness * Time.deltaTime);
    }
}
