using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkWeightUpdater : MonoBehaviour
{
	public ComputeShader Shader;

	public static ChunkWeightUpdater instance;

	ComputeBuffer _weightsBuffer;

	private void Awake() {
		instance = this;
		CreateBuffers();
	}

	private void OnDestroy() {
		DisposeBuffers();
	}

	public void UpdateChunkAt(Chunk chunk, Vector3 globalPosition, float brushSize) {
		Shader.SetBuffer(0, "weights", _weightsBuffer);
		_weightsBuffer.SetData(chunk.Weights);

		Shader.SetInt("numPointsPerAxis", MeshGenerator.ChunkSize);
		Shader.SetVector("hitPosition", globalPosition);
		Shader.SetVector("myPosition", chunk.transform.position);
		Shader.SetFloat("brushSize", brushSize);

		Shader.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

		_weightsBuffer.GetData(chunk.Weights);
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
