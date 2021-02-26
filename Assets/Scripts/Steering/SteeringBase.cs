using UnityEngine;

public abstract class SteeringBase
{
	public abstract void Update();
	public abstract Vector2 GetForce();
	public abstract Vector3 GetForce3D();
	public abstract float GetWeight();
	public abstract void OnDrawGizmos();
}