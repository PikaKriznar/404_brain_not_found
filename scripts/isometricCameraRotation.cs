using UnityEngine;

public class isometricCameraRotation : MonoBehaviour
{
    public float rotationSpeed = 5f;

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(1)) {
            float mouseDeltaX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseDeltaX * rotationSpeed * Time.deltaTime);
        }
    }
}
