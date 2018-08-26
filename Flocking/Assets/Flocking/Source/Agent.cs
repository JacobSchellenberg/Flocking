using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Agent : MonoBehaviour {

	// Alignment
	// Cohesion
	// Separation

	public float speed = 1;
	public float alignmentWeight = 1;
	public float cohesionWeight = 1;
	public float separationWeight = 1;

	Vector3 alignment;
	Vector3 cohesion;
	Vector3 separation;
	Vector3 wallAvoidance;
	List<Agent> agentArray = new List<Agent>();

	LineRenderer lineRenderer;

	void Start(){
		lineRenderer = this.GetComponent<LineRenderer>();
		agentArray = GameObject.FindObjectsOfType<Agent>().ToList();
		this.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
		this.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));

		alignmentWeight = Random.Range(0.0f,50.0f);
		cohesionWeight = Random.Range(0.0f,50.0f);
		separationWeight = Random.Range(0.0f,50.0f);
	}

	void Update(){
		alignment = ComputeAlignment();
		cohesion = ComputeCohesion();
		separation = ComputeSeparation();
		DrawLineToNeighbors();
	
		this.GetComponent<Rigidbody>().velocity += new Vector3(alignment.x * alignmentWeight + cohesion.x * cohesionWeight + separation.x * separationWeight, alignment.y * alignmentWeight + cohesion.y * cohesionWeight + separation.y * separationWeight , alignment.z * alignmentWeight + cohesion.y * cohesionWeight + separation.z * separationWeight) * speed * Time.deltaTime;
		this.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(this.GetComponent<Rigidbody>().velocity.x, -5, 5), Mathf.Clamp(this.GetComponent<Rigidbody>().velocity.y, -5, 5), Mathf.Clamp(this.GetComponent<Rigidbody>().velocity.z, -5, 5));
		this.GetComponent<Rigidbody>().velocity.Normalize();

		ComputeWall();
	}

	void ComputeWall(){
		if(this.transform.position.x >= 15 || this.transform.position.x <= -15 || this.transform.position.y >= 15 || this.transform.position.y <= -15 ||this.transform.position.z >= 15 || this.transform.position.z <= -20){
			this.GetComponent<Rigidbody>().velocity += -Vector3.MoveTowards(this.transform.position, Vector3.zero, 0.1f).normalized; 
			this.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1); //red
		}
		else
			this.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1); // white
	}

	void DrawLineToNeighbors(){
		int index = 1;

		lineRenderer.SetVertexCount(index); // Reset vertex count each time to clear old lines

		foreach(Agent agent in agentArray){
			if(agent != this){
				if(Vector3.Distance(this.transform.position, agent.transform.position) < 5){
					lineRenderer.SetVertexCount(index + 1);
					lineRenderer.SetPosition(index, agent.transform.position);
					index++;
				}
			}
		}

		lineRenderer.SetPosition(0, this.transform.position); // Line should start from this object
	}

	Vector3 ComputeAlignment(){
		Vector3 v = new Vector3();
		int neighborCount = 0;

		foreach(Agent agent in agentArray){
			if(agent != this){
				if(Vector3.Distance(this.transform.position, agent.transform.position) < 5){
					v.x += agent.GetComponent<Rigidbody>().velocity.x;
					v.y += agent.GetComponent<Rigidbody>().velocity.y;
					v.z += agent.GetComponent<Rigidbody>().velocity.z;
					neighborCount++;
				}
			}
		}

		if(neighborCount == 0)
			return v;

		v.x /= neighborCount;
		v.y /= neighborCount;
		v.z /= neighborCount;
		return v.normalized;
	}

	Vector3 ComputeCohesion(){
		Vector3 v = new Vector3();
		int neighborCount = 0;
		
		foreach(Agent agent in agentArray){
			if(agent != this){
				if(Vector3.Distance(this.transform.position, agent.transform.position) < 5){
					v.x += agent.transform.position.x;
					v.y += agent.transform.position.y;
					v.z += agent.transform.position.z;
					neighborCount++;
				}
			}
		}
		
		if(neighborCount == 0)
			return v;
		
		v.x /= neighborCount;
		v.y /= neighborCount;
		v.z /= neighborCount;
		v = new Vector3(v.x - this.transform.position.x, v.y - this.transform.position.y, v.z - this.transform.position.z);
		return v.normalized;
	}

	Vector3 ComputeSeparation(){
		Vector3 v = new Vector3();
		int neighborCount = 0;
		
		foreach(Agent agent in agentArray){
			if(agent != this){
				if(Vector3.Distance(this.transform.position, agent.transform.position) < 5){
					v.x += agent.transform.position.x - this.transform.position.x;
					v.y += agent.transform.position.y - this.transform.position.y;
					v.z += agent.transform.position.z - this.transform.position.z;
					neighborCount++;
				}
			}
		}
		
		if(neighborCount == 0)
			return v;
		
		v.x *= -1;
		v.y *= -1;
		v.z *= -1;
		return v.normalized;
	}
}
