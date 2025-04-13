using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float throwForce = 700f;
    public LayerMask pickupLayer;

    private bool isHolding = false;
    private Rigidbody rb;
    private Camera cam;
    private float holdDistance = 2f; // Distance from camera when picked up

    void Start() {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update() {
        if (isHolding) {
            MoveWithMouse();

            if (Input.GetMouseButtonDown(1)) {
                rb.isKinematic = false;
                rb.AddForce(cam.transform.forward * throwForce);
                isHolding = false;
            }
        }
    }

    void OnMouseDown() {
        holdDistance = Vector3.Distance(transform.position, cam.transform.position); // Store initial distance
        isHolding = true;
        rb.isKinematic = true;
    }

    void OnMouseUp() {
        DropObject();
    }

    void DropObject() {
        isHolding = false;
        rb.isKinematic = false;
    }

    void MoveWithMouse() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.origin + ray.direction * holdDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}
