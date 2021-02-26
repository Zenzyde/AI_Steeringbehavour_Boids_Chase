using UnityEngine;
using UnityEditor;

public class WanderBehaviour : SteeringBase
{
	private Transform AI;
	private float wanderStrength, nextWander, wanderDelay = 1.4f, displacementCircleRadius = 0.5f;
	private Vector2 wanderForce, target;
	private Bounds bounds;
	private bool threeD;
	private Vector3 wanderForce3D, target3D;

	public WanderBehaviour(Transform AI, Bounds bounds, float wanderStrength, float radius, bool threeD)
	{
		this.AI = AI;
		this.wanderStrength = wanderStrength;
		this.bounds = bounds;
		this.displacementCircleRadius = radius;
		this.threeD = threeD;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			if (!bounds.Intersects(AI.GetComponent<Renderer>().bounds))
			{
				wanderForce3D = Vector3.zero;
				return;
			}
			if (Time.time > nextWander)
			{
				target3D = GetDisplacementCircleCenter3D() + GetDisturbanceDirection3D();
				nextWander = Time.time + Random.Range(wanderDelay, wanderDelay + wanderDelay);
			}
			wanderForce3D = target3D;
		}
		else
		{
			if (!bounds.Intersects(AI.GetComponent<Renderer>().bounds))
			{
				wanderForce = Vector2.zero;
				return;
			}
			if (Time.time > nextWander)
			{
				target = GetDisplacementCircleCenter() + GetDisturbanceDirection();
				nextWander = Time.time + Random.Range(wanderDelay, wanderDelay + wanderDelay);
			}
			wanderForce = target;
		}
	}

	Vector2 GetDisturbanceDirection()
	{
		Vector2 direction = Random.insideUnitCircle;
		return direction * wanderStrength;
	}

	Vector3 GetDisturbanceDirection3D()
	{
		Vector3 direction = Random.insideUnitSphere;
		return direction * wanderStrength;
	}

	Vector2 GetDisplacementCircleCenter()
	{
		Vector2 currentHeading = AI.GetComponent<SteeringController>().GetVelocity();
		if (currentHeading == Vector2.zero)
			return AI.right * displacementCircleRadius;
		currentHeading.Normalize();
		return currentHeading * displacementCircleRadius;
	}

	Vector3 GetDisplacementCircleCenter3D()
	{
		Vector3 currentHeading = AI.GetComponent<SteeringController>().GetVelocity3D();
		if (currentHeading == Vector3.zero)
			return AI.right * displacementCircleRadius;
		currentHeading.Normalize();
		return currentHeading * displacementCircleRadius;
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return wanderForce;
	}

	public override Vector3 GetForce3D()
	{
		return wanderForce3D;
	}

	public override float GetWeight()
	{
		return wanderStrength;
	}

	public override void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		if (threeD)
		{
			Handles.DrawLine(AI.position, AI.position + wanderForce3D);
		}
		else
		{
			Handles.DrawLine(AI.position, (Vector2)AI.position + wanderForce);
		}
	}
}