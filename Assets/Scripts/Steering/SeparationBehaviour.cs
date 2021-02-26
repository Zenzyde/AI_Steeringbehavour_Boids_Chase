using UnityEngine;
using UnityEditor;

public class SeparationBehaviour : SteeringBase
{
	private Transform player, AI;
	private float separationStrength, separationRadius = 2.25f;
	private Vector2 separationForce, separationPos;
	private int neighbours;
	private bool threeD;
	private Vector3 separationForce3D, separationPos3D;
	private Camera cam;

	public SeparationBehaviour(Transform player, Transform AI, float separationStrength, float radius, bool threeD)
	{
		this.player = player;
		this.AI = AI;
		this.separationStrength = separationStrength;
		this.separationRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			neighbours = 0;
			separationForce3D = Vector3.zero;
			separationPos3D = Vector3.zero;
			Collider[] AIs = Physics.OverlapSphere(AI.position, separationRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				separationForce3D += (AI.transform.position - ai.ClosestPoint(AI.position));
				separationPos3D += (AI.transform.position - ai.ClosestPoint(AI.position));
				neighbours++;
			}
			if (neighbours == 0)
				return;
			separationForce3D /= neighbours;
			separationPos3D /= neighbours;
			separationPos3D = separationPos3D.normalized * separationStrength;
			//Invert direction
			separationForce3D = separationForce3D.normalized * separationStrength;
		}
		else
		{
			neighbours = 0;
			separationForce = Vector2.zero;
			separationPos = Vector2.zero;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, separationRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider2D ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				separationForce += ((Vector2)AI.transform.position - ai.ClosestPoint(AI.position));
				separationPos += ((Vector2)AI.transform.position - ai.ClosestPoint(AI.position));
				neighbours++;
			}
			if (neighbours == 0)
				return;
			separationForce /= neighbours;
			separationPos /= neighbours;
			separationPos = separationPos.normalized * separationStrength;
			//Invert direction
			separationForce = separationForce.normalized * separationStrength;
		}
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return separationForce;
	}

	public override Vector3 GetForce3D()
	{
		return separationForce3D;
	}

	public override float GetWeight()
	{
		return separationStrength;
	}

	public override void OnDrawGizmos()
	{
		Handles.color = Color.red;
		if (threeD)
		{
			if (separationPos3D == Vector3.zero || neighbours == 0)
				return;
			Handles.DrawWireDisc(separationPos3D, cam.transform.forward, separationRadius);
			Handles.DrawLine(AI.position, separationPos3D);

			Handles.color = Color.black;
			Collider[] AIs = Physics.OverlapSphere(AI.position, separationRadius);
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
			if (separationPos == Vector2.zero || neighbours == 0)
				return;
			Handles.DrawWireDisc(separationPos, Vector3.forward, separationRadius);
			Handles.DrawLine(AI.position, separationPos);

			Handles.color = Color.black;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, separationRadius);
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