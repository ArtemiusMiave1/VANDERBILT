using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("Holding")]
    public Transform holdPos;

    [Header("Pickup")]
    public float pickUpRange = 5f;

    [Header("Cork Board")]
    public CorkBoard corkBoard;

    [Header("Request Line")]
    public RequestLineManager requestLineManager;

    [Header("Held Object Rotation")]
    public Vector3 heldRotationOffset;

    private GameObject heldObj;
    private Rigidbody heldObjRb;

    private int LayerNumber;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        LayerNumber =
            LayerMask.NameToLayer("holdLayer");


        if (corkBoard == null)
        {
            corkBoard =
                FindObjectOfType<CorkBoard>();
        }


        if (requestLineManager == null)
        {
            requestLineManager =
                FindObjectOfType<RequestLineManager>();
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // =================================================
            // NOT HOLDING ANYTHING
            // =================================================

            if (heldObj == null)
            {
                TryPickUp();
            }


            // =================================================
            // ALREADY HOLDING SOMETHING
            // =================================================

            else
            {
                TryDrop();
            }
        }


        // =====================================================
        // KEEP OBJECT IN HAND
        // =====================================================

        if (heldObj != null)
        {
            MoveObject();
        }
    }


    // =========================================================
    // PICK UP
    // =========================================================

    private void TryPickUp()
    {
        RaycastHit hit;


        if (Physics.Raycast(
            transform.position,
            transform.TransformDirection(
                Vector3.forward
            ),
            out hit,
            pickUpRange
        ))
        {
            Debug.Log(
                "Looking at: " +
                hit.collider.name
            );


            // Must have canPickUp tag
            if (
                !hit.transform.gameObject.CompareTag(
                    "canPickUp"
                )
            )
            {
                return;
            }


            PickUpObject(
                hit.transform.gameObject
            );
        }
    }


    // =========================================================
    // PICK UP OBJECT
    // =========================================================

    private void PickUpObject(GameObject pickUpObj)
    {
        Rigidbody rb =
            pickUpObj.GetComponent<Rigidbody>();


        if (rb == null)
        {
            Debug.LogWarning(
                "Object cannot be picked up because it has no Rigidbody."
            );

            return;
        }


        // =====================================================
        // REQUEST
        // =====================================================

        RequestPaper request =
            pickUpObj.GetComponent<RequestPaper>();


        if (request != null)
        {
            // Remove request from the TOP request line
            // immediately when picked up.
            if (requestLineManager == null)
            {
                requestLineManager =
                    FindObjectOfType<RequestLineManager>();
            }


            if (requestLineManager != null)
            {
                requestLineManager.RemoveRequest(
                    pickUpObj
                );
            }
        }


        // =====================================================
        // STORE HELD OBJECT
        // =====================================================

        heldObj = pickUpObj;
        heldObjRb = rb;


        heldObjRb.isKinematic = true;


        heldObj.transform.SetParent(
            holdPos
        );


        heldObj.layer = LayerNumber;


        // =====================================================
        // IGNORE PLAYER COLLISION
        // =====================================================

        Collider objectCollider =
            heldObj.GetComponent<Collider>();

        Collider playerCollider =
            player.GetComponent<Collider>();


        if (
            objectCollider != null &&
            playerCollider != null
        )
        {
            Physics.IgnoreCollision(
                objectCollider,
                playerCollider,
                true
            );
        }


        Debug.Log(
            "Picked up: " +
            heldObj.name
        );
    }


    // =========================================================
    // DROP / PLACE OBJECT
    // =========================================================

    private void TryDrop()
    {
        // =====================================================
        // IS THIS A REQUEST?
        // =====================================================

        RequestPaper request =
            heldObj.GetComponent<RequestPaper>();


        if (request != null)
        {
            TryPlaceOrMoveRequest(
                request
            );

            return;
        }


        // =====================================================
        // NORMAL OBJECT
        // =====================================================

        DropObject();
    }


    // =========================================================
    // PLACE OR MOVE REQUEST
    // =========================================================

    private void TryPlaceOrMoveRequest(
        RequestPaper request
    )
    {
        Ray ray =
            new Ray(
                transform.position,
                transform.forward
            );


        // =====================================================
        // RAYCAST
        // =====================================================

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            pickUpRange
        ))
        {
            Debug.Log(
                "You are not looking at a valid request destination."
            );

            return;
        }


        // =====================================================
        // CHECK CORKBOARD
        // =====================================================

        CorkBoard board =
            hit.collider.GetComponentInParent<CorkBoard>();


        if (board != null)
        {
            bool placed =
                board.TryPlaceRequest(
                    request
                );


            if (!placed)
            {
                // Request stays in player's hands.
                Debug.Log(
                    "Request could not be placed on corkboard."
                );

                return;
            }


            // Successfully placed on corkboard.
            FinalizeHeldObject();

            Debug.Log(
                "Request placed on corkboard."
            );

            return;
        }


        // =====================================================
        // CHECK OLD REQUEST DROP ZONE
        // =====================================================

        RequestDropZone dropZone =
            hit.collider.GetComponentInParent<RequestDropZone>();


        if (dropZone != null)
        {
            if (requestLineManager == null)
            {
                requestLineManager =
                    FindObjectOfType<RequestLineManager>();
            }


            if (requestLineManager == null)
            {
                Debug.LogError(
                    "No RequestLineManager found!"
                );

                return;
            }


            requestLineManager.MoveRequestToOldLine(
                request.gameObject
            );


            // Successfully moved to bottom line.
            FinalizeHeldObject();

            Debug.Log(
                "Request moved to old request line."
            );

            return;
        }


        // =====================================================
        // NOTHING VALID
        // =====================================================

        // IMPORTANT:
        // Do NOT drop the request.
        // It stays in the player's hands.

        Debug.Log(
            "You must be looking at the corkboard or request drop zone."
        );
    }


    // =========================================================
    // FINALIZE HELD OBJECT
    // =========================================================

    private void FinalizeHeldObject()
    {
        if (heldObj == null)
            return;


        // =====================================================
        // RESTORE COLLISION
        // =====================================================

        Collider objectCollider =
            heldObj.GetComponent<Collider>();

        Collider playerCollider =
            player.GetComponent<Collider>();


        if (
            objectCollider != null &&
            playerCollider != null
        )
        {
            Physics.IgnoreCollision(
                objectCollider,
                playerCollider,
                false
            );
        }


        // =====================================================
        // RESTORE LAYER
        // =====================================================

        heldObj.layer = 0;


        // =====================================================
        // KEEP KINEMATIC
        // =====================================================

        heldObjRb.isKinematic = true;


        // =====================================================
        // CLEAR HELD OBJECT
        // =====================================================

        heldObj = null;
        heldObjRb = null;
    }


    // =========================================================
    // NORMAL DROP
    // =========================================================

    private void DropObject()
    {
        if (heldObj == null)
            return;


        // =====================================================
        // RESTORE COLLISION
        // =====================================================

        Collider objectCollider =
            heldObj.GetComponent<Collider>();

        Collider playerCollider =
            player.GetComponent<Collider>();


        if (
            objectCollider != null &&
            playerCollider != null
        )
        {
            Physics.IgnoreCollision(
                objectCollider,
                playerCollider,
                false
            );
        }


        // =====================================================
        // RESTORE LAYER
        // =====================================================

        heldObj.layer = 0;


        // =====================================================
        // ENABLE PHYSICS
        // =====================================================

        heldObjRb.isKinematic = false;


        // =====================================================
        // REMOVE FROM HAND
        // =====================================================

        heldObj.transform.SetParent(
            null
        );


        heldObj = null;
        heldObjRb = null;
    }


    // =========================================================
    // MOVE HELD OBJECT
    // =========================================================

    private void MoveObject()
    {
        if (heldObj == null)
            return;

        if (holdPos == null)
            return;

        // Keep object at holding position
        heldObj.transform.position =
            holdPos.position;

        // =====================================================
        // FACE OBJECT TOWARDS PLAYER
        // =====================================================

        Vector3 direction =
            player.transform.position -
            heldObj.transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            // Rotate the object to compensate for
            // the model's original orientation.
            targetRotation *=
                Quaternion.Euler(
                    heldRotationOffset
                );

            heldObj.transform.rotation =
                targetRotation;
        }
    }
}