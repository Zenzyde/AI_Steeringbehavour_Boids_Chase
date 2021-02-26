using UnityEngine;
using UnityEditor;
using System;

public class AvoidanceBehaviour : SteeringBase
{
	private Transform player, AI;
	private float avoidanceStrength, lineOfSightLength = 1.5f, lineOfSightRadius = 2.5f, lineOfSightAngleRange = -0.25f;
	private Vector2 avoidanceForce, lastVelocityDirection = Vector2.zero;
	private bool threeD;
	private Vector3 avoidanceForce3D, lastVelocityDirection3D = Vector3.zero;
	private Camera cam;

	public AvoidanceBehaviour(Transform player, Transform AI, float avoidanceStrength, float radius, bool threeD)
	{
		this.player = player;
		this.AI = AI;
		this.avoidanceStrength = avoidanceStrength;
		this.lineOfSightRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			if (ObstacleAhead3D())
			{
				Vector3 avoidanceDirection = (GetAheadLineOfSightCenter3D() - FindClosestObstacleCenter3D()).normalized;
				avoidanceForce3D = avoidanceDirection * avoidanceStrength;
			}
			else if (!ObstacleAhead3D())
				avoidanceForce = Vector3.zero;
			lastVelocityDirection3D = AI.GetComponent<SteeringController>().GetVelocity3D().normalized;
		}
		else
		{
			//Check several times for higher accuracy
			if (ObstacleAhead())
			{
				Vector2 avoidanceDirection = (GetAheadLineOfSightCenter() - FindClosestObstacleCenter()).normalized;
				avoidanceForce = avoidanceDirection * avoidanceStrength;
			}
			else if (!ObstacleAhead())
				avoidanceForce = Vector2.zero;
			lastVelocityDirection = AI.GetComponent<SteeringController>().GetVelocity().normalized;
		}
	}

	//The greater the length of the radius, the earlier the AI will "see" & thus react to a threat
	Vector2 GetAheadLineOfSightCenter()
	{
		if (lastVelocityDirection == Vector2.zero)
			return AI.position + AI.right * lineOfSightLength;
		return (Vector2)AI.position + lastVelocityDirection * lineOfSightLength;
	}
	Vector3 GetAheadLineOfSightCenter3D()
	{
		if (lastVelocityDirection3D == Vector3.zero)
			return AI.position + AI.right * lineOfSightLength;
		return AI.position + lastVelocityDirection3D * lineOfSightLength;
	}

	Vector2 FindClosestObstacleCenter()
	{
		Vector2 lineOfSightCenter = GetAheadLineOfSightCenter();
		Collider2D[] obstacles = Physics2D.OverlapCircleAll(lineOfSightCenter, lineOfSightRadius);
		int closestIndex = 0;
		float closestDist = Mathf.Infinity;
		foreach (Collider2D obstacle in obstacles)
		{
			Vector2 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
			Vector2 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
			if (lastVelocityDirection == Vector2.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector2.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange &&
					Vector2.Distance(lineOfSightCenter, obstaclePoint) < closestDist)
					closestIndex = Array.IndexOf(obstacles, obstacle);
			}
			else if (lastVelocityDirection != Vector2.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector2.Dot(lastVelocityDirection, obstacleDirection) > lineOfSightAngleRange &&
					Vector2.Distance(lineOfSightCenter, obstaclePoint) < closestDist)
					closestIndex = Array.IndexOf(obstacles, obstacle);
			}
		}
		return obstacles[closestIndex].transform.position;
	}

	Vector3 FindClosestObstacleCenter3D()
	{
		Vector3 lineOfSightCenter = GetAheadLineOfSightCenter3D();
		Collider[] obstacles = Physics.OverlapSphere(lineOfSightCenter, lineOfSightRadius);
		int closestIndex = 0;
		float closestDist = Mathf.Infinity;
		foreach (Collider obstacle in obstacles)
		{
			Vector3 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
			Vector3 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
			if (lastVelocityDirection3D == Vector3.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector3.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange &&
					Vector3.Distance(lineOfSightCenter, obstaclePoint) < closestDist)
					closestIndex = Array.IndexOf(obstacles, obstacle);
			}
			else if (lastVelocityDirection3D != Vector3.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector3.Dot(lastVelocityDirection, obstacleDirection) > lineOfSightAngleRange &&
					Vector3.Distance(lineOfSightCenter, obstaclePoint) < closestDist)
					closestIndex = Array.IndexOf(obstacles, obstacle);
			}
		}
		return obstacles[closestIndex].transform.position;
	}

	bool ObstacleAhead()
	{
		Vector2 lineOfSightCenter = GetAheadLineOfSightCenter();
		Collider2D[] obstacles = Physics2D.OverlapCircleAll(lineOfSightCenter, lineOfSightRadius);
		foreach (Collider2D obstacle in obstacles)
		{
			Vector2 obstacleDirection = (obstacle.ClosestPoint(lineOfSightCenter) - lineOfSightCenter).normalized;
			if (lastVelocityDirection == Vector2.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector2.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange)
				{
					return true;
				}
			}
			else if (lastVelocityDirection != Vector2.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector2.Dot(lastVelocityDirection, obstacleDirection) > lineOfSightAngleRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool ObstacleAhead3D()
	{
		Vector3 lineOfSightCenter = GetAheadLineOfSightCenter3D();
		Collider[] obstacles = Physics.OverlapSphere(lineOfSightCenter, lineOfSightRadius);
		foreach (Collider obstacle in obstacles)
		{
			Vector3 obstacleDirection = (obstacle.ClosestPoint(lineOfSightCenter) - lineOfSightCenter).normalized;
			if (lastVelocityDirection3D == Vector3.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector3.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange)
				{
					return true;
				}
			}
			else if (lastVelocityDirection3D != Vector3.zero)
			{
				if (obstacle.name.Contains("Obstacle") && Vector3.Dot(lastVelocityDirection, obstacleDirection) > lineOfSightAngleRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return avoidanceForce;
	}

	public override Vector3 GetForce3D()
	{
		return avoidanceForce3D;
	}

	public override float GetWeight()
	{
		return avoidanceStrength;
	}

	public override void OnDrawGizmos()
	{
		float range = ((lineOfSightAngleRange + 1) * 0.5f);
		float angle = (range * 360f);
		Handles.color = Color.black;
		if (threeD)
		{
			Gizmos.color = Color.black;
			Vector3 lineOfSightCenter = GetAheadLineOfSightCenter3D();
			Gizmos.DrawWireSphere(lineOfSightCenter, lineOfSightRadius);
			if (lastVelocityDirection3D == Vector3.zero)
			{
				Handles.DrawLine(AI.position, AI.position + AI.right * lineOfSightLength);
				Handles.color = new Color(1, 1, 0, 0.15f);
				Handles.DrawSolidArc(lineOfSightCenter, cam.transform.forward, AI.right, angle, lineOfSightRadius);
				Handles.DrawSolidArc(lineOfSightCenter, cam.transform.forward, AI.right, -angle, lineOfSightRadius);

				Collider[] obstacles = Physics.OverlapSphere(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider obstacle in obstacles)
				{
					Vector3 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					Vector3 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
					if (obstacle.name.Contains("Obstacle") && Vector3.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange)
					{
						Handles.color = Color.red;
						Handles.DrawSolidDisc(obstaclePoint, cam.transform.forward, .2f);
					}
				}
			}
			else if (lastVelocityDirection3D != Vector3.zero)
			{
				Handles.DrawLine(AI.position, AI.position + lastVelocityDirection3D * lineOfSightLength);
				Handles.color = new Color(1, 1, 0, 0.15f);
				Handles.DrawSolidArc(lineOfSightCenter, cam.transform.forward, lastVelocityDirection3D, angle, lineOfSightRadius);
				Handles.DrawSolidArc(lineOfSightCenter, cam.transform.forward, lastVelocityDirection3D, -angle, lineOfSightRadius);

				Collider[] obstacles = Physics.OverlapSphere(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider obstacle in obstacles)
				{
					Vector3 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					Vector3 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
					if (obstacle.name.Contains("Obstacle") && Vector3.Dot(lastVelocityDirection3D, obstacleDirection) > lineOfSightAngleRange)
					{
						Handles.color = Color.red;
						Handles.DrawSolidDisc(obstaclePoint, cam.transform.forward, .2f);
					}
				}
			}

			if (ObstacleAhead3D())
			{
				Handles.color = Color.red;
				Collider[] obstacles = Physics.OverlapSphere(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider obstacle in obstacles)
				{
					Vector3 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					if (obstacle.name.Contains("Obstacle"))
					{
						Handles.DrawLine(obstaclePoint, lineOfSightCenter);
					}
				}
			}
		}
		else
		{
			Vector2 lineOfSightCenter = GetAheadLineOfSightCenter();
			Handles.DrawWireDisc(lineOfSightCenter, Vector3.forward, lineOfSightRadius);
			if (lastVelocityDirection == Vector2.zero)
			{
				Handles.DrawLine(AI.position, AI.position + AI.right * lineOfSightLength);
				Handles.color = new Color(1, 1, 0, 0.15f);
				Handles.DrawSolidArc(lineOfSightCenter, Vector3.forward, AI.right, angle, lineOfSightRadius);
				Handles.DrawSolidArc(lineOfSightCenter, Vector3.forward, AI.right, -angle, lineOfSightRadius);
				Collider2D[] obstacles = Physics2D.OverlapCircleAll(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider2D obstacle in obstacles)
				{
					Vector2 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					Vector2 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
					if (obstacle.name.Contains("Obstacle") && Vector2.Dot(AI.right, obstacleDirection) > lineOfSightAngleRange)
					{
						Handles.color = Color.red;
						Handles.DrawSolidDisc(obstaclePoint, Vector3.forward, .2f);
					}
				}
			}
			else if (lastVelocityDirection != Vector2.zero)
			{
				Handles.DrawLine(AI.position, (Vector2)AI.position + lastVelocityDirection * lineOfSightLength);
				Handles.color = new Color(1, 1, 0, 0.15f);
				Handles.DrawSolidArc(lineOfSightCenter, Vector3.forward, lastVelocityDirection, angle, lineOfSightRadius);
				Handles.DrawSolidArc(lineOfSightCenter, Vector3.forward, lastVelocityDirection, -angle, lineOfSightRadius);
				Collider2D[] obstacles = Physics2D.OverlapCircleAll(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider2D obstacle in obstacles)
				{
					Vector2 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					Vector2 obstacleDirection = (obstaclePoint - lineOfSightCenter).normalized;
					if (obstacle.name.Contains("Obstacle") && Vector2.Dot(lastVelocityDirection, obstacleDirection) > lineOfSightAngleRange)
					{
						Handles.color = Color.red;
						Handles.DrawSolidDisc(obstaclePoint, Vector3.forward, .2f);
					}
				}
			}

			if (ObstacleAhead())
			{
				Handles.color = Color.red;
				Collider2D[] obstacles = Physics2D.OverlapCircleAll(lineOfSightCenter, lineOfSightRadius);
				foreach (Collider2D obstacle in obstacles)
				{
					Vector2 obstaclePoint = obstacle.ClosestPoint(lineOfSightCenter);
					if (obstacle.name.Contains("Obstacle"))
					{
						Handles.DrawLine(obstaclePoint, lineOfSightCenter);
					}
				}
			}
		}
	}
}