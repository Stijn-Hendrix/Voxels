using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	public ComputeShader Shader;

	ComputeBuffer _weightsBuffer;

	private void Awake() {
		CreateBuffers();
	}

	private void OnDestroy() {
		ReleaseBuffers();
	}

	public float[] GetNoise(Vector3 position) {
		float[] heights = new float[MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize];

		Shader.SetBuffer(0, "_Weights", _weightsBuffer);
		Shader.SetInt("_ChunkSize", MeshGenerator.ChunkSize);
		Shader.SetVector("_Position", position);
		Shader.SetFloat("_NoiseScale", 0.08f);
		Shader.SetFloat("_Amplitude", 200);

		Shader.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

		_weightsBuffer.GetData(heights);

		return heights;
	}

	void CreateBuffers() {
		_weightsBuffer = new ComputeBuffer(MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize, sizeof(float));
	}

	void ReleaseBuffers() {
		_weightsBuffer.Release();
	}

}
