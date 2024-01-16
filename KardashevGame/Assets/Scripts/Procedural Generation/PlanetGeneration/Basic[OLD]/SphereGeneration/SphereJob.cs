using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PlanetGeneration_OLD {

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct SphereJob<G, S> : IJobFor
		where G : struct, ISphereGenerator
		where S : struct, ISphereStreams {
		
		[ReadOnly]
		G generator;

		[WriteOnly]
		S streams;

		public void Execute (int i) => generator.Execute(i, streams);

		public static JobHandle ScheduleParallel (
			Mesh.MeshData meshData, int resolution, JobHandle dependency
		) {
			var job = new SphereJob<G, S>();
			job.generator.Resolution = resolution;
			job.streams.Setup(
				meshData,
				job.generator.VertexCount,
				job.generator.IndexCount
			);
			return job.ScheduleParallel(
				job.generator.JobLength, 1, dependency
			);
		}
	}

	public delegate JobHandle MeshJobScheduleDelegate (
		Mesh.MeshData meshData, int resolution, JobHandle dependency
	);
}