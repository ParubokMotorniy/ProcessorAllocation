using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0, 250)]
    [SerializeField] private float camTranslationSpeed;
    [Range(0, 1)]
    [SerializeField] private float camLerping;
    [Range(0, 150)]
    [SerializeField] private float camRotationSpeed;
    [Range(15, 60)]
    [SerializeField] private float thresholdAngle;

    private Vector3 translationInput;
    private Vector3 rotationInput;
    private Rigidbody cameraBody;

    private void Start()
    {
        cameraBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        translationInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rotationInput = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse1)) { cameraBody.velocity = Vector3.zero; return; }

        Vector3 directionDelta = (transform.forward * translationInput.z + transform.right.normalized * translationInput.x);
        directionDelta.Normalize();
        cameraBody.velocity = directionDelta * camTranslationSpeed * Time.deltaTime;
    }
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse1)) { return; }

        Quaternion yRotation = Quaternion.AngleAxis(rotationInput.x * camRotationSpeed * Time.deltaTime, Vector3.up);
        Quaternion xRotation = Quaternion.AngleAxis(rotationInput.y * camRotationSpeed * Time.deltaTime, yRotation * transform.right);
        Vector3 yForward = yRotation * transform.forward;
        Vector3 xForward = xRotation * yForward;

        float upAngle = Vector3.Angle(Vector3.up, xForward);
        if (upAngle > thresholdAngle && upAngle < (180 - thresholdAngle))
        {
            transform.rotation = Quaternion.LookRotation(xForward, Vector3.up);
        }
    }
}
