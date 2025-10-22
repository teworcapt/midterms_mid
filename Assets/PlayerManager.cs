using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startLine;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        if (!playerPrefab || !startLine)
        {
            Debug.LogError("Assign playerPrefab and startLine in the inspector.");
            return;
        }

        // Spawn player
        GameObject player = Instantiate(playerPrefab, startLine.position + Vector3.up, startLine.rotation);

        // Ensure physics setup
        rb = player.GetComponent<Rigidbody>() ?? player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 2f;

        if (!player.GetComponent<Collider>())
        {
            var col = player.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 0.5f;
        }
    }

    private void Update()
    {
        if (!rb) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        if (move.sqrMagnitude > 0.01f)
            rb.transform.forward = move.normalized;
    }
}
