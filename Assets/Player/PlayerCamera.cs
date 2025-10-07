using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // singleton
    public static PlayerCamera instance;

    // objects
    public Camera playerCam;
    public PlayerManager player;
    [SerializeField] public InputReader input;
    [SerializeField] Transform cameraPivotTransform;

    // tweakable parameters
    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 10f; // bigger number, longer it takes camera to reach target position
    [SerializeField] float leftRightRotationSpeed = 180f;
    [SerializeField] float upDownRotationSpeed = 180f;
    [SerializeField] float minimumPivot = -30f; // lowest point we can look down at
    [SerializeField] float maximumPivot = 60f; // highest point we can look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collisionMask;

    // data display
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; // used for collisions (moves camera object to this position)
    [SerializeField] float leftRightLookAngle;
    [SerializeField] float upDownLookAngle;
    private float cameraZPos; // value for camera collision
    private float targetCamZPos; // value for camera collision

    #region Monobehavior
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cameraZPos = playerCam.transform.localPosition.z;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = playerCam.transform.position - cameraPivotTransform.position;
        Gizmos.DrawRay(playerCam.transform.position, direction); // shows a ray right behind the camera's look dir!
    }

    #endregion

    #region Logic
    public void HandleAllCameraActions()
    {
        if (player)
        {
            HandleFollowPlayer();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowPlayer()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position,
                                                          player.transform.position,
                                                          ref cameraVelocity,
                                                          cameraSmoothSpeed * Time.deltaTime);

        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        // if locked-on, force towards target

        // otherwise normal

        // rotate left and right based on horizontal mouse input
        leftRightLookAngle += input.cameraHorizontalInput * leftRightRotationSpeed * Time.deltaTime;
        // rotate up and down based on vertical mouse input
        upDownLookAngle -= input.cameraVerticalInput * upDownRotationSpeed * Time.deltaTime;
        // clamp up and down angles
        upDownLookAngle = Mathf.Clamp(upDownLookAngle, minimumPivot, maximumPivot);

        Vector3 camRotation = Vector3.zero;
        Quaternion targetRotation;
        // rotates THIS gameobject left and right
        camRotation.y = leftRightLookAngle; // y --> L/R in rotations
        targetRotation = Quaternion.Euler(camRotation);
        transform.rotation = targetRotation;

        // rotates the PIVOT gameobject up and down
        camRotation = Vector3.zero;
        camRotation.x = upDownLookAngle;
        targetRotation = Quaternion.Euler(camRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    // this function is from a tutorial, play around with different ideas based on game feeling later
    // basically, we want to nudge this camera away from a default Z value 
    // if we detect a collision with anything based on a spherecast out the back of the camera
    private void HandleCollisions()
    {
        targetCamZPos = cameraZPos; // LOCAL SPACE
        RaycastHit hit;
        Vector3 direction = playerCam.transform.position - cameraPivotTransform.position; // WORLD SPACE, behind camera's look dir!
        direction.Normalize();

        // project sphere out the back of the camera along its facing dir, check if there's an object in front of our desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCamZPos), collisionMask))
        {
            // if there is an object, get our distance from it
            float distanceFromHit = Vector3.Distance(cameraPivotTransform.position, hit.point); // WORLD, but reference point is rather arbitrary in this
            // we then equate our target z position to below
            targetCamZPos = -(distanceFromHit - cameraCollisionRadius); // LOCAL
        }

        // if our target position is less than our collision radius, subtract our collision radius (make it snap back)
        if (Mathf.Abs(targetCamZPos) < cameraCollisionRadius)
        {
            targetCamZPos = -cameraCollisionRadius; // LOCAL
        }

        // LOCAL SPACE, lerp to new position over 0.2f
        cameraObjectPosition.z = Mathf.Lerp(playerCam.transform.localPosition.z, targetCamZPos, 0.2f);
        playerCam.transform.localPosition = cameraObjectPosition;
    }
    #endregion
}
