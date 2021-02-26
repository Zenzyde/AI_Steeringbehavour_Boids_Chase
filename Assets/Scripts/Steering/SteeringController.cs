using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SteeringController : MonoBehaviour
{
	[SerializeField] private Transform player, bounds;
	[SerializeField] private float maxVelocity;
	[SerializeField]
	private BehaviourSettings fleeSettings, arrivalSettings, wanderSettings, avoidanceSettings, alignSettings,
	cohesionSettings, separationSettings;

	[HideInInspector] public bool doThreeD;

	// Steering inspirations:
	// https://gamedevelopment.tutsplus.com/series/understanding-steering-behaviors--gamedev-12732
	// https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
	private SteeringBase fleeBehaviour, arrivalBehaviour, wanderBehaviour, avoidanceBehaviour, alignBehaviour, cohesionBehaviour,
	separationBehaviour;
	private List<SteeringBase> steeringBehaviours = new List<SteeringBase>();

	private Vector2 moveForce, velocity;
	private Vector3 moveForce3D, velocity3D;

	// Start is called before the first frame update
	void Start()
	{
		fleeBehaviour = new FleeBehaviour(player, transform, doThreeD ? fleeSettings.strength * 0.8f : fleeSettings.strength, fleeSettings.radius, doThreeD);
		arrivalBehaviour = new ArrivalBehaviour(transform, bounds.GetComponent<Renderer>().bounds, arrivalSettings.strength,
			arrivalSettings.radius, doThreeD);
		wanderBehaviour = new WanderBehaviour(transform, bounds.GetComponent<Renderer>().bounds, wanderSettings.strength,
			wanderSettings.radius, doThreeD);
		avoidanceBehaviour = new AvoidanceBehaviour(player, transform, avoidanceSettings.strength, avoidanceSettings.radius, doThreeD);
		alignBehaviour = new AlignBehaviour(player, transform, alignSettings.strength, alignSettings.radius, doThreeD);
		cohesionBehaviour = new CohesionBehaviour(player, transform, cohesionSettings.strength, cohesionSettings.radius, doThreeD);
		separationBehaviour = new SeparationBehaviour(player, transform, separationSettings.strength, separationSettings.radius, doThreeD);
		steeringBehaviours.Add(fleeBehaviour);
		steeringBehaviours.Add(arrivalBehaviour);
		steeringBehaviours.Add(wanderBehaviour);
		steeringBehaviours.Add(avoidanceBehaviour);
		steeringBehaviours.Add(alignBehaviour);
		steeringBehaviours.Add(cohesionBehaviour);
		steeringBehaviours.Add(separationBehaviour);
	}

	void OnEnable()
	{
		Bounds bound = bounds.GetComponent<Renderer>().bounds;
		if (doThreeD)
		{
			transform.position = new Vector3(
				Random.Range(bound.min.x + 5, bound.max.x - 5),
				Random.Range(bound.min.y + 5, bound.max.y - 5),
				Random.Range(bound.min.z + 5, bound.max.z - 5)
			);
		}
		else
		{
			transform.position = new Vector2(
				Random.Range(bound.min.x + 5, bound.max.x - 5),
				Random.Range(bound.min.y + 5, bound.max.y - 5)
			);
		}
	}

	// Update is called once per frame
	void Update()
	{
		foreach (SteeringBase steering in steeringBehaviours)
		{
			steering.Update();
		}
		AddForces();
	}

	//Slight inspiration from "weighted prioritized truncated sum"
	// https://alastaira.wordpress.com/2013/03/13/methods-for-combining-autonomous-steering-behaviours/
	void FixedUpdate()
	{
		if (doThreeD)
		{
			velocity3D += moveForce3D * Time.fixedDeltaTime;
			velocity3D = velocity3D.magnitude > maxVelocity ? velocity3D.normalized * maxVelocity : velocity3D;
			transform.position += velocity3D * Time.fixedDeltaTime;
		}
		else
		{
			velocity += moveForce * Time.fixedDeltaTime;
			velocity = velocity.magnitude > maxVelocity ? velocity.normalized * maxVelocity : velocity;
			transform.position = (Vector2)transform.position + velocity * Time.fixedDeltaTime;
		}
	}

	void OnDrawGizmos()
	{
		if (!EditorApplication.isPlaying)
			return;
		if (fleeSettings.showGizmo)
		{
			fleeBehaviour.OnDrawGizmos();
		}
		if (alignSettings.showGizmo)
		{
			alignBehaviour.OnDrawGizmos();
		}
		if (arrivalSettings.showGizmo)
		{
			arrivalBehaviour.OnDrawGizmos();
		}
		if (avoidanceSettings.showGizmo)
		{
			avoidanceBehaviour.OnDrawGizmos();
		}
		if (cohesionSettings.showGizmo)
		{
			cohesionBehaviour.OnDrawGizmos();
		}
		if (separationSettings.showGizmo)
		{
			separationBehaviour.OnDrawGizmos();
		}
		if (wanderSettings.showGizmo)
		{
			wanderBehaviour.OnDrawGizmos();
		}
	}

	//Slight hybrid solution of "weighted blending" & "priority arbitration"
	//Weighted blending: just add all forces to a total and act with that force
	//Priority arbitration: give each behaviour a priority number & only act with the current highest priority behaviour's force
	//Others not used:
	// *Weighted prioritized truncated sum: check highest priority & add to total, move to next, add to total & truncate if > max
	// *Prioritized dithering: use random chance, check current highest priority behaviour, add if current > chance else check next
	// https://alastaira.wordpress.com/2013/03/13/methods-for-combining-autonomous-steering-behaviours/
	void AddForces()
	{
		//Nullify instead of normalizing to be able to add multiple behavioral forces together
		//Normalization keeps the directional vector which interferes with the overall targeted behaviour
		if (doThreeD)
		{
			moveForce3D = Vector3.zero;
			moveForce3D += arrivalBehaviour.GetForce3D() - velocity3D;
			if (arrivalBehaviour.GetWeight() < arrivalSettings.strength)
				moveForce3D += wanderBehaviour.GetForce3D() - velocity3D;
			moveForce3D += fleeBehaviour.GetForce3D() - velocity3D;
			moveForce3D += avoidanceBehaviour.GetForce3D() - velocity3D;
			moveForce3D += alignBehaviour.GetForce3D() - velocity3D;
			moveForce3D += cohesionBehaviour.GetForce3D() - velocity3D;
			moveForce3D += separationBehaviour.GetForce3D() - velocity3D;
		}
		else
		{
			moveForce = Vector2.zero;
			moveForce += arrivalBehaviour.GetForce() - velocity;
			if (arrivalBehaviour.GetWeight() < arrivalSettings.strength)
				moveForce += wanderBehaviour.GetForce() - velocity;
			moveForce += fleeBehaviour.GetForce() - velocity;
			moveForce += avoidanceBehaviour.GetForce() - velocity;
			moveForce += alignBehaviour.GetForce() - velocity;
			moveForce += cohesionBehaviour.GetForce() - velocity;
			moveForce += separationBehaviour.GetForce() - velocity;
		}
	}

	public Vector2 GetVelocity()
	{
		return velocity;
	}

	public Vector3 GetVelocity3D()
	{
		return velocity3D;
	}
}
