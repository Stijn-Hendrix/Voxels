using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public ComputeShader Shader;

	ComputeBuffer _trianglesBuffer;
	ComputeBuffer _trianglesCountBuffer;
	ComputeBuffer _weightsBuffer;

	public const int ChunkSize = 32;

	private void Awake() {
		CreateBuffers();
	}

	private void OnDestroy() {
		ReleaseBuffers();
	}

	struct Triangle {
		public Vector3 a;
		public Vector3 b;
		public Vector3 c;

		public static int SizeOf => sizeof(float) * 3 * 3;
	}

	public Mesh GetMeshData(float[] weights, Chunk from) {
		Shader.SetBuffer(0, "_Triangles", _trianglesBuffer);
		Shader.SetBuffer(0, "_Weights", _weightsBuffer);

		Shader.SetInt("_ChunkSize", ChunkSize);
		Shader.SetFloat("_IsoLevel", .5f);

		_weightsBuffer.SetData(weights);
		_trianglesBuffer.SetCounterValue(0);

		Shader.Dispatch(0, ChunkSize / 8, ChunkSize / 8, ChunkSize / 8);

		Triangle[] triangles = new Triangle[ReadTriangleCount()];
		_trianglesBuffer.GetData(triangles);

		return CreateMesh(triangles, from);
	}

	// Read triangles count from the buffer
	int ReadTriangleCount() {
		int[] triCount = { 0 };
		ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
		_trianglesCountBuffer.GetData(triCount);
		return triCount[0];
	}

	Mesh CreateMesh(Triangle[] triangles, Chunk from) {
		Vector3[] verts = new Vector3[triangles.Length * 3];
		int[] tris = new int[triangles.Length * 3];

		for (int i = 0; i < triangles.Length; i++) {
			int startIndex = i * 3;

			verts[startIndex] = triangles[i].a;
			verts[startIndex + 1] = triangles[i].b;
			verts[startIndex + 2] = triangles[i].c;

			tris[startIndex] = startIndex;
			tris[startIndex + 1] = startIndex + 1;
			tris[startIndex + 2] = startIndex + 2;
		}

		Mesh mesh = from.MeshCollider.sharedMesh;
		mesh.Clear();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.RecalculateNormals();
		return mesh;
	}

	void CreateBuffers() {
		_trianglesBuffer = new ComputeBuffer(5 * (ChunkSize * ChunkSize * ChunkSize), Triangle.SizeOf, ComputeBufferType.Append);
		_trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
		_weightsBuffer = new ComputeBuffer(ChunkSize * ChunkSize * ChunkSize, sizeof(float));
	}

	void ReleaseBuffers() {
		_trianglesBuffer.Release();
		_trianglesCountBuffer.Release();
		_weightsBuffer.Release();
	}
}