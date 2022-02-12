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

		Weights = noiseGenerator.Run(transform.position);

		MeshCollider.sharedMesh = new Mesh();
	}


	private void Update() {
		_meshGenerator.RequestMeshData(OnReceiveMeshData, Weights);
		this.enabled = false;
	}

	public void EditWeights(Vector3 globalOrigin, float brushSize) {
		ChunkWeightUpdater.instance.UpdateChunkAt(this, globalOrigin, brushSize);
		RequestUpdate();
	}

	public void RequestUpdate() {
		this.enabled = true;
	}

	void OnReceiveMeshData(MeshData meshData) {
		Mesh mesh = MeshCollider.sharedMesh;
		mesh.Clear();
		mesh.vertices = meshData.vertices;
		mesh.triangles = meshData.triangles;
		mesh.RecalculateNormals();

		MeshCollider.sharedMesh = mesh;
		Display(mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;

		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}
