using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Range(2, 20)]
    [SerializeField] int renderDistance;

    [SerializeField] Transform player;

	[Header("References")]
    [SerializeField] Chunk chunkPrefab;
	[SerializeField] MeshGenerator MeshGenerator;
	[SerializeField] NoiseGenerator NoiseGenerator;

	List<Chunk> _currentlyEnabledChunks = new List<Chunk>();

    Dictionary<Vector3Int, Chunk> _chunkDictionary = new Dictionary<Vector3Int, Chunk>();
    PriorityQueue<Vector3Int> _buildQueue = new PriorityQueue<Vector3Int>();


	private void Start()
	{
        UpdateChunks();

        int count = _buildQueue.Count;
        for (int i = 0; i < count; i++)
        {
            var chunkPos = _buildQueue.Dequeue();
            CreateChunk(chunkPos);
        }
    }

    private void Update()
    {
        UpdateChunks();

        int count = Mathf.Min(5, _buildQueue.Count);
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

        foreach (var keypair in _chunkDictionary)
        {
            Chunk chunk = keypair.Value;
            if (chunk != null)
            {
                if (!InRenderDistance(ToChunkPosition(chunk.transform.position), relPos, renderDistance))
                {

                    chunk.gameObject.SetActive(false);
                }
            }
        }

        for (int x = -renderDistance + relPos.x; x < renderDistance + relPos.x; x++)
        {
            for (int y = -renderDistance + relPos.y; y < renderDistance + relPos.y; y++)
            {
                for (int z = -renderDistance + relPos.z; z < renderDistance + relPos.z; z++)
                {
                    Vector3Int chunkPos = 
                        new Vector3Int(x * (MeshGenerator.ChunkSize - 1), y * (MeshGenerator.ChunkSize - 1), z * (MeshGenerator.ChunkSize - 1));

                    if (!_chunkDictionary.ContainsKey(chunkPos))
                    {
                        _buildQueue.Enqueue(chunkPos, ManhattanDistance(Vector3Int.FloorToInt(player.transform.position), chunkPos));
                        _chunkDictionary.Add(chunkPos, null);
                    }
                    else
                    {
                        if (_chunkDictionary[chunkPos] != null) _chunkDictionary[chunkPos].gameObject.SetActive(true);
                    }
                }
            }
        }
    }


    private void CreateChunk(Vector3Int chunkPos)
    {
        Chunk chunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, transform);
        chunk.Initialize(MeshGenerator, NoiseGenerator);

        Vector3 boundsCenterPos = chunkPos + (Vector3.one * ((MeshGenerator.ChunkSize - 1) / 2));

        chunk.Bounds = new Bounds(boundsCenterPos, Vector3.one * (MeshGenerator.ChunkSize - 1));
        chunk.name = $"{ToChunkPosition(chunkPos).ToString()}";

        _chunkDictionary[chunkPos] = chunk;
        _currentlyEnabledChunks.Add(chunk);
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

    bool InRenderDistance(Vector3Int a, Vector3Int b, int chunkRenderingDistance)
    {
        return Mathf.Abs(a.x - b.x) <= chunkRenderingDistance && Mathf.Abs(a.y - b.y) <= chunkRenderingDistance && Mathf.Abs(a.z - b.z) <= chunkRenderingDistance;
    }

    private static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }
}
