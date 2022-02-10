using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

	public float[] weights;

	public MeshFilter MeshFilterDisplay;

	public MeshCollider MeshCollider;

	public Chunk other;

	private void Start() {
		weights = NoiseGenerator.instance.Run(transform.position);

	}

	private void Update() {
		InstantUpdateMesh();

		this.enabled = false;
	}

	public void EditWeights(Vector3 globalOrigin) {
		WeightsUpdater.instance.UpdateChunkAt(this, globalOrigin);
	
		RequestUpdate();
	}

	int indexFromCoord(int x, int y, int z) {
		return x + MeshGenerator.ChunkSize * (y + MeshGenerator.ChunkSize * z);
	}

	Vector3Int ToLocalOrigin(Vector3 globalOrigin) {
		return new Vector3Int(
					Mathf.FloorToInt(globalOrigin.x - (transform.position.x * MeshGenerator.ChunkSize - 1)),
					Mathf.FloorToInt(globalOrigin.y),
					Mathf.FloorToInt(globalOrigin.z - (transform.position.z * MeshGenerator.ChunkSize - 1))
				);
	}

	public void RequestUpdate() {
		this.enabled = true;
	}

	void InstantUpdateMesh() {
		Mesh mesh = MeshGenerator.instance.Run(weights);
		MeshCollider.sharedMesh = mesh;
		Display(mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
	}
}
