using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoiseType {
	FLAT,
	SPHERICAL
}

public class NoiseGenerator : MonoBehaviour
{
   
    [Header("Shaders")]
	public ComputeShader FlatTerrainShader;
	public ComputeShader SphericalTerrainShader;

	public NoiseType NoiseType;


	[Header("Noise Parameters")]

	[SerializeField] float noiseScale = 0.08f;
	[SerializeField] float amplitude = 200;
	[SerializeField] float frequency = 0.004f;
	[SerializeField] int octaves = 6;
	[SerializeField, Range(0f,1f)] float groundPercent = 0.2f;


    ComputeShader _noiseConstructionCompute;
	ComputeBuffer _weightsBuffer;

	private void Awake() {
		switch (NoiseType) {
			case NoiseType.FLAT:
				_noiseConstructionCompute = FlatTerrainShader;
				break;
			case NoiseType.SPHERICAL:
				_noiseConstructionCompute = SphericalTerrainShader;
				break;
		}

		CreateBuffers();
	}

	private void OnDestroy() {
		ReleaseBuffers();
	}

	public float[] GetNoise(Vector3 position) {
		float[] heights = new float[MeshGenerator.ChunkSize * MeshGenerator.ChunkSize * MeshGenerator.ChunkSize];

		_noiseConstructionCompute.SetBuffer(0, "_Weights", _weightsBuffer);
		_noiseConstructionCompute.SetInt("_ChunkSize", MeshGenerator.ChunkSize);
		_noiseConstructionCompute.SetVector("_Position", position);
		_noiseConstructionCompute.SetFloat("_NoiseScale", noiseScale);
		_noiseConstructionCompute.SetFloat("_Amplitude", amplitude);
		_noiseConstructionCompute.SetFloat("_Frequency", frequency);
		_noiseConstructionCompute.SetInt("_Octaves", octaves);
		_noiseConstructionCompute.SetFloat("_GroundPercent", groundPercent);

		_noiseConstructionCompute.Dispatch(0, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8, MeshGenerator.ChunkSize / 8);

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
