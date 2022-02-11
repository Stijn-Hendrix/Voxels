using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public float[] Weights { get; set; }

	public MeshFilter MeshFilterDisplay;

	public MeshCollider MeshCollider;

	public Bounds Bounds;

	private void Start() {
		Weights = NoiseGenerator.instance.Run(transform.position);
	}

	private void Update() {
		MeshGenerator.instance.RequestMeshData(OnReceiveMeshData, Weights);
		this.enabled = false;
	}

	public void EditWeights(Vector3 globalOrigin, float brushSize) {
		WeightsUpdater.instance.UpdateChunkAt(this, globalOrigin, brushSize);
		RequestUpdate();
	}

	public void RequestUpdate() {
		this.enabled = true;
	}

	void OnReceiveMeshData(MeshData meshData) {
		Mesh mesh = meshData.Get();

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
