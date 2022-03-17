using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpherePhysics : MonoBehaviour {

    [SerializeField, Range(0f, 100f)] private float maxSpeed = 5f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0f, 5f)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;
    private Vector3 velocity, desiredVelocity, contactNormal;
    private float minGroundDotProduct;
    private bool desiredjump;
    private int jumpPhase, groundContactCount;
    private Rigidbody body;
    private MeshRenderer meshRenderer;
    private bool OnGround => groundContactCount > 0;

    private void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake() {
        body = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        OnValidate();
    }

    void Update() {
        GetInput();
        ChangeSphereColor();
    }

    private void FixedUpdate() {
        UpdateState();
        AdjustVelocity();
        if (desiredjump)
            Jump();
        body.velocity = velocity;
        ClearState();
    }

    private void OnCollisionEnter(Collision other) {
        EvaluateCollision(other);
    }

    private void OnCollisionStay(Collision other) {
        EvaluateCollision(other);
    }

    private void GetInput() {
        Vector2 playerInput;

        desiredjump |= Input.GetButtonDown("Jump");
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
    }

    private void Jump() {
        if (!OnGround && jumpPhase >= maxAirJumps)
            return;

        desiredjump = false;
        jumpPhase++;
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        float alignedSpeed = Vector3.Dot(velocity, contactNormal);
        if (alignedSpeed > 0f)
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        velocity += contactNormal * jumpSpeed;
    }

    private void UpdateState() {
        velocity = body.velocity;
        if (OnGround) {
            jumpPhase = 0;
            if(groundContactCount > 1)
                contactNormal.Normalize();
            return;
        }
        contactNormal = Vector3.up;
    }

    private void EvaluateCollision(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                groundContactCount++;
                contactNormal += normal;
            }
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector) {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    private void AdjustVelocity() {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void ClearState() {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    private void ChangeSphereColor(){
        meshRenderer.material.SetColor("_BaseColor", Color.white * (groundContactCount * 0.25f));
    }
}
