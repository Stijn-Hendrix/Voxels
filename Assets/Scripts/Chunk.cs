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

	public void RequestUpdate() {
		this.enabled = true;
	}

	private void Update() {
		UpdateMesh(_meshGenerator.RequestMeshData(Weights));
		this.enabled = false;
	}

	public void EditWeights(Vector3 globalOrigin, float brushSize) {
		ChunkWeightUpdater.instance.UpdateChunkAt(this, globalOrigin, brushSize);
		RequestUpdate();
	}

	void UpdateMesh(MeshData meshData) {
		Mesh mesh = MeshCollider.sharedMesh;
		mesh.Clear();
		mesh.vertices = meshData.vertices;
		mesh.triangles = meshData.triangles;
		mesh.RecalculateNormals();

		Display(mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
		MeshCollider.sharedMesh = mesh;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}
