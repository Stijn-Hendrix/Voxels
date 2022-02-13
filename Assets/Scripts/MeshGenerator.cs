using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public ComputeShader Shader;

	ComputeBuffer _trianglesBuffer;
	ComputeBuffer _trianglesCountBuffer;
	ComputeBuffer _weightsBuffer;

	public const int ChunkSize = 40;

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

	public MeshData RequestMeshData(float[] weights) {
		Shader.SetBuffer(0, "triangles", _trianglesBuffer);
		Shader.SetBuffer(0, "weights", _weightsBuffer);

		Shader.SetInt("chunkSize", ChunkSize);
		Shader.SetFloat("isoLevel", .5f);

		_weightsBuffer.SetData(weights);
		_trianglesBuffer.SetCounterValue(0);

		Shader.Dispatch(0, ChunkSize / 8, ChunkSize / 8, ChunkSize / 8);

		Triangle[] triangles = new Triangle[ReadTriangleCount()];
		_trianglesBuffer.GetData(triangles);

		return CreateMeshData(triangles);
	}

	// Read triangles count from the buffer
	int ReadTriangleCount() {
		int[] triCount = { 0 };
		ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
		_trianglesCountBuffer.GetData(triCount);
		return triCount[0];
	}

	MeshData CreateMeshData(Triangle[] triangles) {
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
		return new MeshData(tris, verts);
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

public struct MeshData {
	public int[] triangles;
	public Vector3[] vertices;

	public MeshData(int[] triangles, Vector3[] vertices) {
		this.triangles = triangles;
		this.vertices = vertices;
	}
}
