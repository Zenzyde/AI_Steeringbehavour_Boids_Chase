using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlySwatter : MonoBehaviour
{
	[SerializeField] private float swatRadius, swatRange;
	[SerializeField] private GameObject visualizer3D;

	private List<SteeringController> flies = new List<SteeringController>();
	private float simulatedRadius;
	private Vector2 position = new Vector2(1000f, 1000f);
	private bool canSwat = true, isSwatting;
	private Round round;
	private Vector3 position3D = new Vector3(1000f, 1000f, 1000f);
	
	[HideInInspector] public bool threeD;

	void Start()
    {
		flies.AddRange(FindObjectsOfType<SteeringController>());
		round = FindObjectOfType<Round>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSwat && !threeD || Input.GetKeyDown(KeyCode.F) && canSwat && threeD)
		    SwatFlies();
        if (Input.GetKeyDown(KeyCode.Space) && round.IsGameOver())//AllFliesSwatted())
			round.Restart();
		// foreach (SteeringController fly in flies)
		// {
		// 	fly.gameObject.SetActive(true);
		// }
		if (!isSwatting)
		{
			if (threeD)
			{
				position3D = new Vector3(1000f, 1000f, 1000f);
			}
			else
			{
				position = new Vector2(1000f, 1000f);
			}
		}
	}

    bool AllFliesSwatted()
    {
		int totalFlies = 0;
        foreach (SteeringController fly in flies)
        {
            if (!fly.gameObject.activeSelf)
				totalFlies++;
		}
        if (totalFlies == flies.Count)
			return true;
		return false;
	}

    void SwatFlies()
    {
		canSwat = false;
		isSwatting = true;
		if (threeD)
		{
			StartCoroutine(DoSwat3D());
		}
		else
		{
			StartCoroutine(DoSwat());
		}
	}

    IEnumerator DoSwat()
    {
		List<Collider2D> AIs = new List<Collider2D>();
		position = GetMouseInput();
		while (simulatedRadius < swatRadius)
        {
			Collider2D[] newAIs = Physics2D.OverlapCircleAll(position, simulatedRadius);
			simulatedRadius += 0.1f;
			yield return null;
            if (newAIs.Length == 0)
				continue;
            if (AIs.Count == 0)
            {
				for (int i = 0; i < newAIs.Length; i++)
				{
					AIs.Add(newAIs[i]);
				}
            }
            else
            {
				foreach (Collider2D ai in newAIs)
                {
					bool skip = false;
					for (int i = 0; i < AIs.Count; i++)
                    {
                        if (ai == AIs[i])
							skip = true;
					}
                    if (!skip)
						AIs.Add(ai);
				}
            }
		}
        if (AIs.Count > 0)
        {
			foreach (var ai in AIs)
			{
				if (ai.GetComponent<SteeringController>())
				{
					ai.gameObject.SetActive(false);
				}
			}
        }
		simulatedRadius = 0f;
		canSwat = true;
		isSwatting = false;
	}

	IEnumerator DoSwat3D()
    {
		List<Collider> AIs = new List<Collider>();
		position3D = transform.position + transform.forward * swatRange;
		visualizer3D.transform.position = position3D;
		visualizer3D.transform.localScale = Vector3.zero;
		while (simulatedRadius < swatRadius)
        {
			Collider[] newAIs = Physics.OverlapSphere(position3D, simulatedRadius);
			simulatedRadius += .1f;
			visualizer3D.transform.localScale += Vector3.one * .1f;
			yield return null;
            if (newAIs.Length == 0)
				continue;
            if (AIs.Count == 0)
            {
				for (int i = 0; i < newAIs.Length; i++)
				{
					AIs.Add(newAIs[i]);
				}
            }
            else
            {
				foreach (Collider ai in newAIs)
                {
					bool skip = false;
					for (int i = 0; i < AIs.Count; i++)
                    {
                        if (ai == AIs[i])
							skip = true;
					}
                    if (!skip)
						AIs.Add(ai);
				}
            }
		}
        if (AIs.Count > 0)
        {
			foreach (var ai in AIs)
			{
				if (ai.GetComponent<SteeringController>())
				{
					ai.gameObject.SetActive(false);
				}
			}
        }
		simulatedRadius = 0f;
		visualizer3D.transform.localScale = Vector3.zero;
		canSwat = true;
		isSwatting = false;
	}

    Vector2 GetMouseInput()
    {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

    public Vector2 GetPosition()
    {
		return position;
	}

	public Vector3 GetPosition3D()
    {
		return position3D;
	}

	public Vector3 GetVisualizerPosition()
	{
		return transform.position + transform.forward * swatRange;
	}

    public Vector2 GetRadiusPosition(SteeringController AI)
    {
		foreach (SteeringController ai in flies)
        {
            if (ai == AI)
				return position + ((Vector2)AI.transform.position - position).normalized * simulatedRadius;
		}
		return new Vector2(1000f, 1000f);
	}

	public Vector3 GetRadiusPosition3D(SteeringController AI)
    {
		foreach (SteeringController ai in flies)
        {
            if (ai == AI)
			{
				if (position3D == Vector3.one * 1000f)
				{
					return transform.position + (AI.transform.position - transform.position).normalized * simulatedRadius;
				}
				else
				{
					return position3D + (AI.transform.position - position3D).normalized * simulatedRadius;
				}
			}
		}
		return new Vector3(1000f, 1000f, 1000f);
	}

    void OnDrawGizmos()
    {
		Handles.color = Color.green;
		Gizmos.color = Color.green;
		if (EditorApplication.isPlaying)
		{
			if (threeD)
			{
				if (isSwatting)
					return;
				visualizer3D.transform.localScale = Vector3.one * swatRadius;
				visualizer3D.transform.position = transform.position + transform.forward * swatRange;
			}
			else
			{
				Handles.DrawSolidDisc(position, Vector3.forward, simulatedRadius);
			}
		}
	}
}
