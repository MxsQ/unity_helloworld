using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{

	[SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f;

	[SerializeField, Range(0f, 100f)]
	float maxAcceleration = 20f, maxAirAcceleration = 5f;

	[SerializeField, Range(0f, 10f)]
	float jumpHeight = 2f;

	[SerializeField, Range(0, 5)]
	int maxAirJumps = 0;

	[SerializeField, Range(0, 90)]
	float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField, Min(0f)]
	float probeDistance = 1f;

	[SerializeField]
	LayerMask probeMask = -1, stairMask=-1;

	[SerializeField]
	Transform playerInputSpace = default;

	Rigidbody body;

	Vector3 velocity, desiredVelocity;

	Vector3 contactNormal, steepNormal;

	bool desiredJump;

	int groundContactCount, steepContactCount;

	bool OnGround => groundContactCount > 0;

	bool OnSteep => steepContactCount > 0;

	int jumpPhase;

	float minGroundDotProduct, minStairsDotProduct;

	int stepsSinceLastGrounded, stepsSinceLastJump;

	void OnValidate()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
	}

	void Awake()
	{
		body = GetComponent<Rigidbody>();
		OnValidate();
	}

	void Update()
	{

		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =
			new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

		desiredJump |= Input.GetButtonDown("Jump");

		GetComponent<Renderer>().material.SetColor(
			"_Color", OnGround ? Color.black : Color.white
			);

		if (playerInputSpace)
		{
			Vector3 forward = playerInputSpace.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 right = playerInputSpace.right;
			right.y = 0f;
			right.Normalize();
			desiredVelocity = (forward * playerInput.y + right * playerInput.x) * maxSpeed;
		}
        else
        {
			desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }
	}

	void FixedUpdate()
	{
		UpdateState();
		AdjustVelocity();

		if (desiredJump)
		{
			desiredJump = false;
			Jump();
		}

		body.velocity = velocity;
		ClearState();
	}

	void ClearState()
	{
		groundContactCount = steepContactCount =0;
		contactNormal = steepNormal = Vector3.zero;
	}

	void UpdateState()
	{
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if (OnGround || SnapToGround() || checkSteepContacts())
		{
			stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1)
            {
				jumpPhase = 0;
            }
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = Vector3.up;
		}
	}

	float GetMinDot(int layer)
    {
		return (stairMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
    }

	bool SnapToGround()
    {
		if(stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
			return false;
        }
		float speed = velocity.magnitude;
		if(speed > maxSnapSpeed)
        {
			return false;
        }
		if(!Physics.Raycast(body.position, Vector2.down, out RaycastHit hit, probeDistance, probeMask))
        {
			return false;
        }
		if(hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
        {
			return false;
        }

		// still in gourd , but just has not touch.
		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if(dot > 0f)
        {
			velocity = (velocity - hit.normal * dot).normalized * speed;
        }

		return true;
    }

	bool checkSteepContacts()
    {
		// deal with crevice
		if(steepContactCount > 1)
        {
			steepNormal.Normalize();
			if(steepNormal.y >= minGroundDotProduct)
            {
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
            }
        }
		return false;
    }

	void AdjustVelocity()
	{
		Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

		float currentX = Vector3.Dot(velocity, xAxis);
		float currentZ = Vector3.Dot(velocity, zAxis);

		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX =
			Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}

	void Jump()
	{
		Vector3 jumpDirection;
        if (OnGround)
        {
			jumpDirection = contactNormal;
        }
		else if (OnSteep)
        {
			jumpDirection = steepNormal;
			jumpPhase = 0;
        }
		else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
        {
			if(jumpPhase == 0)
            {
				jumpPhase = 1;
            }
			jumpDirection = contactNormal;
        }
        else
        {
			return;
        }

		//if (OnGround || jumpPhase < maxAirJumps)
		//{
			stepsSinceLastJump = 0;
			jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
			if (alignedSpeed > 0f)
			{
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			velocity += jumpDirection * jumpSpeed;
		//}
	}

	void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void EvaluateCollision(Collision collision)
	{
		float minDot = GetMinDot(collision.gameObject.layer);
		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			if (normal.y >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
			} else if(normal.y > -0.01f)
            {
				// to check steep
				steepContactCount += 1;
				steepNormal += normal;
            }
		}
	}

	Vector3 ProjectOnContactPlane(Vector3 vector)
	{
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}
}