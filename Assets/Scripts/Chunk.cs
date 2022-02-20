using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public float[] Weights { get; set; }

	public MeshFilter MeshFilterDisplay;
	public MeshCollider MeshCollider;
	public Bounds Bounds;

	MeshGenerator _meshGenerator;

	public void Initialize(MeshGenerator meshGenerator, NoiseGenerator noiseGenerator) {
		_meshGenerator = meshGenerator;

		Weights = noiseGenerator.GetNoise(transform.position);

		MeshCollider.sharedMesh = new Mesh();

		UpdateMesh(_meshGenerator.GetMeshData(Weights, this));
	}

	public void EditWeights(Vector3 globalOrigin, float brushSize) {
		ChunkWeightUpdater.instance.UpdateChunkAt(this, globalOrigin, brushSize);
		UpdateMesh(_meshGenerator.GetMeshData(Weights, this));
	}

	void UpdateMesh(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
		MeshCollider.sharedMesh = mesh;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}
