using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Rendering")]
    [Range(1, 20), SerializeField] int renderDistanceHorizontal;
    [Range(1, 20), SerializeField] int renderDistanceVertical;
    [Range(1, 10), SerializeField] int maxChunkBuildPerFrame;
    [SerializeField] Transform player;
    private Vector3Int _lastPlayerChunk;

	[Header("References")]
    [SerializeField] Chunk chunkPrefab;
	[SerializeField] MeshGenerator MeshGenerator;
	[SerializeField] NoiseGenerator NoiseGenerator;

    Dictionary<Vector3Int, Chunk> _chunkDictionary = new Dictionary<Vector3Int, Chunk>();
    PriorityQueue<Vector3Int> _buildQueue = new PriorityQueue<Vector3Int>();
	List<Chunk> _currentlyEnabledChunks = new List<Chunk>();

	private void Start()
	{
        Vector3 playerPos = player.transform.position;
        Vector3Int relPos = ToChunkPosition(playerPos);
        _lastPlayerChunk = relPos + Vector3Int.one; // Set to something different than current player pos for initial chunk creation

        UpdateChunks();
        ConstructBuildQueue(_buildQueue.Count);
    }

    private void Update()
    {
        UpdateChunks();
        ConstructBuildQueue(maxChunkBuildPerFrame);
    }

    void ConstructBuildQueue(int size)
    {
        int count = Mathf.Min(size, _buildQueue.Count);
        for (int i = 0; i < count; i++)
        {
            var chunkPos = _buildQueue.Dequeue();
            CreateChunk(chunkPos);
        }
    }

	void UpdateChunks()
	{
        Vector3 playerPos = player.transform.position;
        Vector3Int relPos = ToChunkPosition(playerPos);

        if (_lastPlayerChunk == relPos)
        {
            return;
        }

        _currentlyEnabledChunks.ForEach(chunk => chunk.gameObject.SetActive(false) );
        _currentlyEnabledChunks.Clear();

        for (int x = -renderDistanceHorizontal + relPos.x; x < renderDistanceHorizontal + relPos.x; x++)
        {
            for (int y = -renderDistanceVertical + relPos.y; y < renderDistanceVertical + relPos.y; y++)
            {
                for (int z = -renderDistanceHorizontal + relPos.z; z < renderDistanceHorizontal + relPos.z; z++)
                {
                    Vector3Int chunkglobalPos = 
                        new Vector3Int(x * (MeshGenerator.ChunkSize - 1), y * (MeshGenerator.ChunkSize - 1), z * (MeshGenerator.ChunkSize - 1));

                    if (!_chunkDictionary.ContainsKey(chunkglobalPos))
                    {
                        _buildQueue.Enqueue(chunkglobalPos, ManhattanDistance(Vector3Int.FloorToInt(player.transform.position), chunkglobalPos));
                        _chunkDictionary.Add(chunkglobalPos, null);
                    }
                    else
                    {
                        Chunk chunk = _chunkDictionary[chunkglobalPos];
                        if (chunk != null)
                        {
                            chunk.gameObject.SetActive(true);
                            _currentlyEnabledChunks.Add(chunk);
                        }
                    }
                }
            }
        }

        _lastPlayerChunk = relPos;
    }


    private void CreateChunk(Vector3Int chunkglobalPos)
    {
        Chunk chunk = Instantiate(chunkPrefab, chunkglobalPos, Quaternion.identity, transform);
        chunk.Initialize(MeshGenerator, NoiseGenerator);

        Vector3 boundsCenterPos = chunkglobalPos + (Vector3.one * ((MeshGenerator.ChunkSize - 1) / 2));

        chunk.Bounds = new Bounds(boundsCenterPos, Vector3.one * (MeshGenerator.ChunkSize - 1));
        chunk.name = $"{ToChunkPosition(chunkglobalPos).ToString()}";

        _chunkDictionary[chunkglobalPos] = chunk;

        bool visible = InRenderDistance(_lastPlayerChunk, ToChunkPosition(chunkglobalPos), renderDistanceHorizontal, renderDistanceVertical);
        chunk.gameObject.SetActive(visible);
        if (visible) _currentlyEnabledChunks.Add(chunk);
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

	Vector3Int ToChunkPosition(Vector3 globalPosition) {
		return new Vector3Int(
					Mathf.FloorToInt((globalPosition.x) / (MeshGenerator.ChunkSize - 1)),
					Mathf.FloorToInt((globalPosition.y) / (MeshGenerator.ChunkSize - 1)),
					Mathf.FloorToInt((globalPosition.z) / (MeshGenerator.ChunkSize - 1))
				);
	}

    bool InRenderDistance(Vector3Int a, Vector3Int b, int horizontal, int vertical)
    {
        return Mathf.Abs(a.x - b.x) <= horizontal && Mathf.Abs(a.y - b.y) <= vertical && Mathf.Abs(a.z - b.z) <= horizontal;
    }

    private static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }
}
