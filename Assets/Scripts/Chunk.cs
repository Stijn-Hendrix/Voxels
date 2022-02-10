using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

	float[] heights;

	public MeshFilter MeshFilterDisplay;

	private void Start() {
		heights = NoiseGenerator.instance.Run(transform.position);

		UpdateMesh();
	}

	public void UpdateMesh() {
		Mesh mesh = MeshGenerator.instance.Run(heights);
		Display(mesh);
	}

	void Display(Mesh mesh) {
		MeshFilterDisplay.sharedMesh = mesh;
	}
}
