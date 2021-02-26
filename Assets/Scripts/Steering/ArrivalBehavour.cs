using UnityEngine;
using UnityEditor;

public class ArrivalBehaviour : SteeringBase
{
	private Transform AI;
	private Bounds bounds;
	private float arrivalStrength, boundsRadius, arrivalRadius = 2f, arrivalTime, currentArrivalTime;
	private Vector2 arrivalForce, target;
	private bool aquiredTarget, threeD;
	private Vector3 arrivalForce3D, target3D;
	private Camera cam;

	public ArrivalBehaviour(Transform AI, Bounds bounds, float arrivalStrength, float radius, bool threeD)
	{
		this.AI = AI;
		this.bounds = bounds;
		this.arrivalStrength = arrivalStrength;
		this.arrivalRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			if (!bounds.Intersects(AI.GetComponent<Renderer>().bounds) && !aquiredTarget)
			{
				target3D = GetRandomPointWithinBounds3D();
				aquiredTarget = true;
			}
			else if (aquiredTarget && Vector3.Distance(target3D, AI.position) > arrivalRadius)
			{
				//https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-flee-and-arrival--gamedev-1303 
				//desired_velocity = normalize(desired_velocity) * max_velocity * (distance / slowingRadius)
				arrivalForce3D = (target3D - AI.position).normalized;
				arrivalForce3D = arrivalForce3D.normalized * arrivalStrength * (Vector3.Distance(target3D, AI.position) / arrivalRadius);
			}
			if (Vector3.Distance(target3D, AI.position) <= arrivalRadius && bounds.Intersects(AI.GetComponent<Renderer>().bounds))
			{
				arrivalForce3D = Vector3.zero;
				target3D = Vector3.zero;
				aquiredTarget = false;
			}
		}
		else
		{
			if (!bounds.Intersects(AI.GetComponent<Renderer>().bounds) && !aquiredTarget)
			{
				target = GetRandomPointWithinBounds();
				aquiredTarget = true;
			}
			else if (aquiredTarget && Vector3.Distance(target, AI.position) > arrivalRadius)
			{
				//https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-flee-and-arrival--gamedev-1303 
				//desired_velocity = normalize(desired_velocity) * max_velocity * (distance / slowingRadius)
				arrivalForce = (target - (Vector2)AI.position).normalized;
				arrivalForce = arrivalForce.normalized * arrivalStrength * (Vector3.Distance(target, AI.position) / arrivalRadius);
			}
			if (Vector3.Distance(target, AI.position) <= arrivalRadius && bounds.Intersects(AI.GetComponent<Renderer>().bounds))
			{
				arrivalForce = Vector2.zero;
				target = Vector2.zero;
				aquiredTarget = false;
			}
		}
	}

	Vector2 GetRandomPointWithinBounds()
	{
		Vector2 randomXY = new Vector2(
			Random.Range(bounds.min.x + arrivalRadius, bounds.max.x - arrivalRadius),
			Random.Range(bounds.min.y + arrivalRadius, bounds.max.y - arrivalRadius));
		return new Vector2(randomXY.x, randomXY.y);
	}

	Vector3 GetRandomPointWithinBounds3D()
	{
		Vector3 randomXYZ = new Vector3(
			Random.Range(bounds.min.x + arrivalRadius, bounds.max.x - arrivalRadius),
			Random.Range(bounds.min.y + arrivalRadius, bounds.max.y - arrivalRadius),
			Random.Range(bounds.min.z + arrivalRadius, bounds.max.z - arrivalRadius));
		return randomXYZ;
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return arrivalForce;
	}

	public override Vector3 GetForce3D()
	{
		return arrivalForce3D;
	}

	public override float GetWeight()
	{
		return aquiredTarget ? arrivalStrength : 0.0f;
	}

	public override void OnDrawGizmos()
	{
		if (target == Vector2.zero)
		{
			return;
		}
		if (threeD)
		{
			Handles.color = Color.blue;
			Handles.DrawLine(AI.position, target3D);
			Handles.DrawWireDisc(target3D, cam.transform.forward, arrivalRadius);
			Handles.color = Color.magenta;
			Handles.SphereHandleCap(0, target3D, Quaternion.identity, 1f, EventType.Ignore);
		}
		else
		{
			Handles.color = Color.blue;
			Handles.DrawLine(AI.position, target);
			Handles.DrawWireDisc(target, Vector3.forward, arrivalRadius);
			Handles.color = Color.magenta;
			Handles.DrawLine(target, (Vector3)target - Vector3.forward * 1.5f);
		}
	}
}