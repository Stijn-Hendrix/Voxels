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
		InstantUpdateMesh();
		this.enabled = false;
	}

	public void EditWeights(Vector3 globalOrigin, float brushSize) {
		WeightsUpdater.instance.UpdateChunkAt(this, globalOrigin, brushSize);
		RequestUpdate();
	}

	public void RequestUpdate() {
		this.enabled = true;
	}

	void InstantUpdateMesh() {
		Mesh _mesh = MeshGenerator.instance.Run(Weights);
		MeshCollider.sharedMesh = _mesh;
		Display(_mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;

		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}
