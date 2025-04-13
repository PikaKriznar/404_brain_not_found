using UnityEngine;

public class isometricCameraPanner : MonoBehaviour
{
    public float panSpeed = 6f;
    private Camera _camera;

    public Vector2 panLimitX;
    public Vector2 panLimitZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() {
        _camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 panPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * new Vector3(panPosition.x, 0, panPosition.y) * (panSpeed * Time.deltaTime);

    }
}
