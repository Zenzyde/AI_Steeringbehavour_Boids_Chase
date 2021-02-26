using UnityEngine;
using UnityEditor;

public class AlignBehaviour : SteeringBase
{
	private Transform player, AI;
	private float alignStrength, alignRadius = 2.75f;
	private Vector2 alignForce;
	private int neighbours;
	private bool threeD;
	private Vector3 alignForce3D;
	private Camera cam;

	public AlignBehaviour(Transform player, Transform AI, float alignStrength, float radius, bool threeD)
	{
		this.player = player;
		this.AI = AI;
		this.alignStrength = alignStrength;
		this.alignRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			neighbours = 0;
			alignForce3D = Vector3.zero;
			Collider[] AIs = Physics.OverlapSphere(AI.position, alignRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				alignForce3D += ai.GetComponent<SteeringController>().GetVelocity3D();
				neighbours++;
			}
			alignForce3D /= neighbours;
			alignForce3D = alignForce3D.normalized * alignStrength;
		}
		else
		{
			neighbours = 0;
			alignForce = Vector2.zero;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, alignRadius);
			if (AIs.Length == 0)
				return;
			foreach (Collider2D ai in AIs)
			{
				if (ai.transform == AI.transform || !ai.GetComponent<SteeringController>())
					continue;
				alignForce += ai.GetComponent<SteeringController>().GetVelocity();
				neighbours++;
			}
			alignForce /= neighbours;
			alignForce = alignForce.normalized * alignStrength;
		}
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return alignForce;
	}

	public override Vector3 GetForce3D()
	{
		return alignForce3D;
	}

	public override float GetWeight()
	{
		return alignStrength;
	}

	public override void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		if (threeD)
		{
			Handles.DrawLine(AI.position, AI.position + alignForce3D);
			Handles.DrawWireDisc(AI.position, cam.transform.forward, alignRadius);

			Handles.color = Color.green;
			Collider[] AIs = Physics.OverlapSphere(AI.position, alignRadius);
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
			Handles.DrawLine(AI.position, (Vector2)AI.position + alignForce);
			Handles.DrawWireDisc(AI.position, Vector3.forward, alignRadius);

			Handles.color = Color.green;
			Collider2D[] AIs = Physics2D.OverlapCircleAll(AI.position, alignRadius);
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