using UnityEngine;
using System.Collections;

public class BuildingSmoke : MonoBehaviour {

	public ParticleSystem particleSystem;

	public void StartSmoke(GridObject grid_object)
	{
		StartSmoke(grid_object.size);
	}

	public void StartSmoke(Vector2 size)
	{	
		// create four particle systems
		// two horizontal
		SetupParticleSystem(new Vector2(size.x,1), new Vector2(0f,((size.y / 2) + 1)));
		SetupParticleSystem(new Vector2(size.x,1), new Vector2(0f,-((size.y / 2) - 1)));
		// two vertical
		SetupParticleSystem(new Vector2(1, size.y), new Vector2((size.x / 2) + 1,0f));
		SetupParticleSystem(new Vector2(1, size.y), new Vector2(-(size.x / 2) - 1,0f));

		// one in the center
		SetupParticleSystem(new Vector2(size.x, size.y), new Vector2(0,0));

	}

	void SetupParticleSystem(Vector2 size, Vector2 offset)
	{
		// isntantiate and set position
		GameObject new_particle_go = GameObject.Instantiate(particleSystem.gameObject);
		new_particle_go.transform.position = particleSystem.transform.position;
		new_particle_go.transform.position += new Vector3(offset.x * GridHelper.GetGridCellSize(), 0,
														  offset.y * GridHelper.GetGridCellSize ());

		// set parent and local scale
		new_particle_go.transform.parent = particleSystem.transform.parent.parent;

		new_particle_go.transform.localScale = new Vector3 (size.x * GridHelper.GetGridCellSize (), 
			                                    			size.y * GridHelper.GetGridCellSize (),
			                                    			new_particle_go.transform.localScale.z);

		// grab component and set emission rate
		ParticleSystem new_component = new_particle_go.GetComponent<ParticleSystem>();
		new_component.emissionRate *= Mathf.Max(size.x, size.y); 
		// play and destroy system in future
		new_component.Play();
		Destroy(new_particle_go, new_component.duration * 30f);

	}
}
