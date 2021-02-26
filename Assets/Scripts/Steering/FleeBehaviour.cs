using UnityEngine;
using UnityEditor;

public class FleeBehaviour : SteeringBase
{
	private Transform player, AI;
	private float fleeStrength, fleeRadius = 2.25f;
	private Vector2 fleeForce;
	private FlySwatter swatter;
	private bool threeD;
	private Vector3 fleeForce3D;
	private Camera cam;

	public FleeBehaviour(Transform player, Transform AI, float fleeStrength, float radius, bool threeD)
	{
		this.player = player;
		this.AI = AI;
		this.fleeStrength = fleeStrength;
		this.swatter = player.GetComponent<FlySwatter>();
		this.fleeRadius = radius;
		this.threeD = threeD;
		cam = Camera.main;
	}

	void CalculateForce()
	{
		if (threeD)
		{
			Vector3 fleeDirection = (AI.position - swatter.GetVisualizerPosition()).normalized;
			float swatDist = (AI.position - swatter.GetRadiusPosition3D(AI.GetComponent<SteeringController>())).magnitude;
			if (swatDist > fleeRadius)
			{
				fleeForce3D = Vector3.zero;
				return;
			}

			//FlySwatter
			fleeForce3D = fleeDirection * fleeStrength;

			fleeForce3D *= swatDist > .1f ? 1f / swatDist : 1f;
		}
		else
		{
			Vector2 fleeDirection = ((Vector2)AI.position - swatter.GetPosition()).normalized;
			float swatDist = ((Vector2)AI.position - swatter.GetRadiusPosition(AI.GetComponent<SteeringController>())).magnitude;
			if (swatDist > fleeRadius)
			{
				fleeForce = Vector3.zero;
				return;
			}

			//FlySwatter
			fleeForce = fleeDirection * fleeStrength;

			fleeForce *= swatDist > .1f ? 1f / swatDist : 1f;
		}
	}

	public override void Update()
	{
		CalculateForce();
	}

	public override Vector2 GetForce()
	{
		return fleeForce;
	}

	public override Vector3 GetForce3D()
	{
		return fleeForce3D;
	}

	public override float GetWeight()
	{
		return fleeStrength;
	}

	public override void OnDrawGizmos()
	{
		Handles.color = Color.blue;
		if (threeD)
		{
			if (Vector3.Distance(swatter.GetVisualizerPosition(), AI.position) < fleeRadius)
				Handles.color = Color.red;
			Handles.DrawWireDisc(AI.position, cam.transform.forward, fleeRadius);

			Handles.DrawLine(AI.position, AI.position + fleeForce3D);
		}
		else
		{
			if (Vector2.Distance(swatter.GetPosition(), AI.position) < fleeRadius)
				Handles.color = Color.red;
			Handles.DrawWireDisc(AI.position, Vector3.forward, fleeRadius);

			Handles.DrawLine(AI.position, (Vector2)AI.position + fleeForce);
		}
	}
}