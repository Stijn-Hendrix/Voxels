using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	public ComputeShader Shader;

	ComputeBuffer _weightsBuffer;

	public static NoiseGenerator instance;

	private void Awake() {
		instance = this;
		CreateBuffers();
	}

	private void OnDestroy() {
		DisposeBuffers();
	}

	public float[] Run(Vector3 position) {
		CreateBuffers();

		float[] heights = new float[MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize];

		Shader.SetBuffer(0, "weights", _weightsBuffer);

		Shader.SetInt("numPointsPerAxis", MeshGenerator.ChunkSize);
		Shader.SetVector("position", position);
		Shader.SetFloat("noiseScale", 0.08f);
		Shader.SetFloat("amplitude", 200);

		Shader.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

		_weightsBuffer.GetData(heights);

		return heights;
	}

	void CreateBuffers() {
		if (_weightsBuffer != null) {
			_weightsBuffer.Release();
		}
		_weightsBuffer = new ComputeBuffer(MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize, sizeof(float));
	}

	void DisposeBuffers() {
		_weightsBuffer.Dispose();
	}

}
