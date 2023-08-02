using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
		ReleaseBuffers();
	}

	public void UpdateChunkAt(Chunk chunk, Vector3 globalPosition, float brushSize) {
		Shader.SetBuffer(0, "_Weights", _weightsBuffer);

		_weightsBuffer.SetData(chunk.Weights);

		Shader.SetInt("_ChunkSize", MeshGenerator.ChunkSize);
		Shader.SetVector("_HitPosition", globalPosition);
		Shader.SetVector("_MyPosition", chunk.transform.position);
		Shader.SetFloat("_BrushSize", brushSize);

		Shader.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

		_weightsBuffer.GetData(chunk.Weights);
    }

    void CreateBuffers() {
		_weightsBuffer = new ComputeBuffer(MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize, sizeof(float));
	}

	void ReleaseBuffers() {
		_weightsBuffer.Release();
	}
}
