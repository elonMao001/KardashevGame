using Unity.Mathematics;
using UnityEngine;

namespace PlanetGeneration_OLD {
	public interface ISphereStreams {

		void Setup(
			Mesh.MeshData meshData, int vertexCount, int indexCount
		);

		void SetVertex(int index, Vertex vertex);

		void SetTriangle(int index, int3 triangle);
	}
}