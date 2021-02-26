using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraMover : MonoBehaviour
{
	[SerializeField] private Transform bounds;
	[SerializeField] private float moveSpeed, rotateSpeed;
	[SerializeField] private Vector2 camRadius;
	[SerializeField] private bool doThreeD;
	[SerializeField] private GameObject[] twoDObjects, threeDObjects;

	private bool camLocked;

	void OnValidate()
	{
		foreach (GameObject obj in twoDObjects)
		{
			obj.SetActive(!doThreeD);
		}
		foreach (GameObject obj in threeDObjects)
		{
			obj.SetActive(doThreeD);
		}
	}

	void Awake()
	{
		GetComponent<FlySwatter>().threeD = doThreeD;
		foreach (SteeringController steering in FindObjectsOfType<SteeringController>())
		{
			steering.doThreeD = doThreeD;
		}
		GetComponent<Camera>().orthographic = !doThreeD;
		FindObjectOfType<ObstacleMover>().threeD = doThreeD;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
			camLocked = !camLocked;
		if (camLocked)
			return;
		Move();
	}

	void OnDrawGizmos()
	{
		if (doThreeD)
			return;
		Handles.color = Color.green;
		Handles.DrawAAPolyLine(4f, (Vector2)transform.position - Vector2.right * camRadius - Vector2.up * camRadius,
			(Vector2)transform.position + Vector2.right * camRadius - Vector2.up * camRadius);

		Handles.DrawAAPolyLine(4f, (Vector2)transform.position - Vector2.right * camRadius + Vector2.up * camRadius,
			(Vector2)transform.position + Vector2.right * camRadius + Vector2.up * camRadius);

		Handles.DrawAAPolyLine(4f, (Vector2)transform.position - Vector2.right * camRadius + Vector2.up * camRadius,
			(Vector2)transform.position - Vector2.right * camRadius - Vector2.up * camRadius);

		Handles.DrawAAPolyLine(4f, (Vector2)transform.position + Vector2.right * camRadius + Vector2.up * camRadius,
			(Vector2)transform.position + Vector2.right * camRadius - Vector2.up * camRadius);
	}

	void Move()
	{
		if (doThreeD)
		{
			if (Input.GetKey(KeyCode.W) && CanMove3D(transform.forward))
				transform.position += transform.forward * moveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.S) && CanMove3D(-transform.forward))
				transform.position -= transform.forward * moveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.A) && CanMove3D(-transform.right))
				transform.position -= transform.right * moveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.D) && CanMove3D(transform.right))
				transform.position += transform.right * moveSpeed * Time.deltaTime;

			if (Input.GetKey(KeyCode.UpArrow))
				transform.Rotate(Vector3.right, -rotateSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.DownArrow))
				transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.LeftArrow))
				transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.RightArrow))
				transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}
		else
		{
			Vector3 moveVector = MouseVector();
			if (MoveToMouse())
				transform.position += moveVector * moveSpeed * Time.deltaTime;
		}
	}

	bool CanMove(Vector2 position)
	{
		position = (Vector2)transform.position + position;
		return position.x + camRadius.x < bounds.GetComponent<Collider>().bounds.max.x &&
			position.x - camRadius.x > bounds.GetComponent<Collider>().bounds.min.x &&
			position.y + camRadius.y < bounds.GetComponent<Collider>().bounds.max.y &&
			position.y - camRadius.y > bounds.GetComponent<Collider>().bounds.min.y;
	}

	bool CanMove3D(Vector3 position)
	{
		position = transform.position + position;
		return position.x < bounds.GetComponent<Collider>().bounds.max.x &&
			position.x > bounds.GetComponent<Collider>().bounds.min.x &&
			position.y < bounds.GetComponent<Collider>().bounds.max.y &&
			position.y > bounds.GetComponent<Collider>().bounds.min.y &&
			position.z < bounds.GetComponent<Collider>().bounds.max.z &&
			position.z > bounds.GetComponent<Collider>().bounds.min.z;
	}

	Vector3 MouseVector()
	{
		if (MoveToMouse())
			return (GetMouseInput() - transform.position).normalized;
		return transform.position;
	}

	bool MoveToMouse()
	{
		Vector3 mouseVector = GetMouseInput();
		return mouseVector.x < transform.position.x - camRadius.x || mouseVector.x > transform.position.x + camRadius.x ||
			mouseVector.y < transform.position.y - camRadius.y || mouseVector.y > transform.position.y + camRadius.y;
	}

	public Vector3 GetMouseInput()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
