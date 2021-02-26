using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
	[SerializeField] private float moveSpeed;

	private Vector2 startPos, target = Vector2.zero;
	private Vector3 startPos3D, target3D = Vector3.zero;

	[HideInInspector] public bool threeD;

	void Start()
	{
		startPos = (Vector2)transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (threeD)
		{
			if (target3D == Vector3.zero || Vector3.Distance(transform.position, target3D) < 0.5f)
			{
				target3D = startPos3D + Random.insideUnitSphere * 5f;
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, target3D, moveSpeed * Time.deltaTime);
			}
		}
		else
		{
			if (target == Vector2.zero || Vector2.Distance(transform.position, target) < 0.5f)
			{
				target = startPos + Random.insideUnitCircle * 5f;
			}
			else
			{
				transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
			}
		}
	}
}
