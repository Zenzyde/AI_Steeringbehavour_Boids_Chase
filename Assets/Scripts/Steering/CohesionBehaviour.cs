using UnityEngine;
using UnityEditor;

public class CohesionBehaviour : SteeringBase
{
	private Transform player, AI;
	private float cohesionStrength, cohesionRadius = 2.75f;
	private Vector2 cohesionForce, cohesionPos;
	private int neighbours;
	private bool threeD;
	private Vector3 cohesionForce3D, cohesionPos3D;
	private Camera cam;

	public CohesionBehaviour(Transform player, Transform AI, float cohesionStrength, float radius, bool threeD)
	{
		this.player = player;
		this.AI = AI;
		this.cohesionStrength = cohesionStrength;
		this.cohesionRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			neighbours = 0;
			cohesionForce3D = Vector3.zero;
			cohesionPos3D = Vector3.zero;
			Collider[] AIs = Physics.OverlapSphere(AI.position, cohesionRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				cohesionForce3D += ai.transform.position;
				cohesionPos3D += ai.transform.position;
				neighbours++;
			}
			if (neighbours == 0)
				return;
			cohesionForce3D /= neighbours;
			cohesionPos3D /= neighbours;
			//Get direction to center
			cohesionForce3D = (cohesionForce3D - AI.position).normalized * cohesionStrength;
		}
		else
		{
			neighbours = 0;
			cohesionForce = Vector2.zero;
			cohesionPos = Vector2.zero;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, cohesionRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider2D ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				cohesionForce += (Vector2)ai.transform.position;
				cohesionPos += (Vector2)ai.transform.position;
				neighbours++;
			}
			if (neighbours == 0)
				return;
			cohesionForce /= neighbours;
			cohesionPos /= neighbours;
			//Get direction to center
			cohesionForce = (cohesionForce - (Vector2)AI.position).normalized * cohesionStrength;
		}
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return cohesionForce;
	}

	public override Vector3 GetForce3D()
	{
		return cohesionForce3D;
	}

	public override float GetWeight()
	{
		return cohesionStrength;
	}

	public override void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		if (threeD)
		{
			if (cohesionPos3D == Vector3.zero || neighbours == 0)
				return;
			Handles.DrawWireDisc(cohesionPos3D, cam.transform.forward, cohesionRadius);
			Handles.DrawLine(AI.position, cohesionPos3D);

			Handles.color = Color.green;
			Collider[] AIs = Physics.OverlapSphere(AI.position, cohesionRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				Handles.DrawWireDisc(ai.transform.position, cam.transform.forward, 1f);
			}
		}
		else
		{
			if (cohesionPos == Vector2.zero || neighbours == 0)
				return;
			Handles.DrawWireDisc(cohesionPos, Vector3.forward, cohesionRadius);
			Handles.DrawLine(AI.position, cohesionPos);

			Handles.color = Color.green;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, cohesionRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider2D ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				Handles.DrawWireDisc(ai.transform.position, cam.transform.forward, 1f);
			}
		}
	}
}