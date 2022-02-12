using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] Chunk chunkPrefab;


	[SerializeField] int viewDistance;

	List<Chunk> _currentlyEnabledChunks;

	Dictionary<Vector3Int, Chunk> _chunkDictionary;

	public static ChunkManager instance;

	private void Awake() {
		instance = this;

		_chunkDictionary = new Dictionary<Vector3Int, Chunk>();
		_currentlyEnabledChunks = new List<Chunk>();

	}

	private void Start() {
		for (int x = -viewDistance; x < viewDistance; x++) {
			for (int z = -viewDistance; z < viewDistance; z++) {

				Vector3 pos = new Vector3(x * (MeshGenerator.ChunkSize - 1), 0, z * (MeshGenerator.ChunkSize - 1));

				var chunk = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);

				var boundsCenter = pos + (Vector3.one * ((MeshGenerator.ChunkSize - 1) / 2));

				chunk.Bounds = new Bounds(boundsCenter, Vector3.one * (MeshGenerator.ChunkSize - 1));

				chunk.name = $"{ToChunkPosition(pos).ToString()}";

				_chunkDictionary.Add(ToChunkPosition(pos), chunk);
				_currentlyEnabledChunks.Add(chunk);
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
					0,
					Mathf.FloorToInt((globalOrigin.z) / (MeshGenerator.ChunkSize - 1))
				);
	}

}
