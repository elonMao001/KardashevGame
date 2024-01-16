
using UnityEngine;

namespace PlanetGeneration_OLD {

	public interface ISphereGenerator {

		int VertexCount { get; }

		int IndexCount { get; }

		int JobLength { get; }

		int Resolution { get; set; }

		void Execute<S> (int i, S streams) where S : struct, ISphereStreams;
	}
}