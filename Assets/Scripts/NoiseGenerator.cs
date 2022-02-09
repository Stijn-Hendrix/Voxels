using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	public ComputeShader Shader;

	ComputeBuffer _weightsBuffer;

	public MeshGenerator MeshGenerator;

	private void Start() {
		var heights = Run();

		MeshGenerator.Run(heights);
	}

	float[] Run() {
		CreateBuffers();

		float[] heights = new float[MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize];

		Shader.SetBuffer(0, "weights", _weightsBuffer);

		Shader.SetInt("numPointsPerAxis", MeshGenerator.ChunkSize);
		Shader.SetVector("position", transform.position);

		Shader.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

		_weightsBuffer.GetData(heights);

		DisposeBuffers();

		return heights;
	}

	void CreateBuffers() {
		_weightsBuffer = new ComputeBuffer(MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize, sizeof(float));
	}

	void DisposeBuffers() {
		_weightsBuffer.Dispose();
	}

}
