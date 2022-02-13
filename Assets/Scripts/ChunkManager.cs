using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	[SerializeField] int horizontalViewDistance;
	[SerializeField] int verticalViewDistance;

	[Header("References")]
    [SerializeField] Chunk chunkPrefab;
	[SerializeField] MeshGenerator MeshGenerator;
	[SerializeField] NoiseGenerator NoiseGenerator;

	List<Chunk> _currentlyEnabledChunks = new List<Chunk>();
	Dictionary<Vector3Int, Chunk> _chunkDictionary = new Dictionary<Vector3Int, Chunk>();

	private void Start() {
		for (int x = -horizontalViewDistance; x < horizontalViewDistance; x++) {
			for (int y = -verticalViewDistance; y < verticalViewDistance; y++) {
				for (int z = -horizontalViewDistance; z < horizontalViewDistance; z++) {

					Vector3 chunkPos = new Vector3(x * (MeshGenerator.ChunkSize - 1), y * (MeshGenerator.ChunkSize - 1), z * (MeshGenerator.ChunkSize - 1));

					Chunk chunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, transform);
					chunk.Initialize(MeshGenerator, NoiseGenerator);

					Vector3 boundsCenterPos = chunkPos + (Vector3.one * ((MeshGenerator.ChunkSize - 1) / 2));

					chunk.Bounds = new Bounds(boundsCenterPos, Vector3.one * (MeshGenerator.ChunkSize - 1));
					chunk.name = $"{ToChunkPosition(chunkPos).ToString()}";

					_chunkDictionary.Add(ToChunkPosition(chunkPos), chunk);
					_currentlyEnabledChunks.Add(chunk);
				}
			}
		}
	}

	public void EditWeights(Vector3 globalPosition, float brushSize) {
		// Add 5 to make sure you check a slightly larger range than brush size
		// avoids cracks when editing with brush size just on the edge between chunks
		float brushSizeSqrd = brushSize * brushSize + 5;

		foreach (var chunk in _currentlyEnabledChunks) {
			if (chunk.Bounds.SqrDistance(globalPosition) < brushSizeSqrd) {
				chunk.EditWeights(globalPosition, brushSize);
			}
		}
	}

	Vector3Int ToChunkPosition(Vector3 globalOrigin) {
		return new Vector3Int(
					Mathf.FloorToInt((globalOrigin.x) / (MeshGenerator.ChunkSize - 1)),
					Mathf.FloorToInt((globalOrigin.y) / (MeshGenerator.ChunkSize - 1)),
					Mathf.FloorToInt((globalOrigin.z) / (MeshGenerator.ChunkSize - 1))
				);
	}

}
