using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

	float[] heights;

	public MeshFilter MeshFilterDisplay;

	public MeshCollider MeshCollider;

	private void Start() {
		heights = NoiseGenerator.instance.Run(transform.position);

	}

	private void Update() {
		InstantUpdateMesh();

		this.enabled = false;
	}

	public void EditHeights(Vector3 globalOrigin) {
		Vector3Int localOrigin = ToLocalOrigin(globalOrigin);

		for (int x = localOrigin.x - 2; x < localOrigin.x + 2; x++) {

			for (int y = localOrigin.y - 2; y < localOrigin.y + 2; y++) {

				for (int z = localOrigin.z - 2; z < localOrigin.z + 2; z++) {
					heights[indexFromCoord(x, y, z)] += 1;
				}
			}
		}

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
		Mesh mesh = MeshGenerator.instance.Run(heights);
		MeshCollider.sharedMesh = mesh;
		Display(mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
	}
}
