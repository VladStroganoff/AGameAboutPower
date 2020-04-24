using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TycoonTerrain.Core.Generation {
	/// <summary>
	/// Generates an example terrain
	/// </summary>
	public struct TerrainGenerationJob : IJob {
		private const int MaxHeight = byte.MaxValue;

		private NativeArray<byte> heightData;
		private readonly int2 size;

		public TerrainGenerationJob(NativeArray<byte> heightMap, int2 dimensions) {
			if (dimensions.x * dimensions.y != heightMap.Length) {
				throw new ArgumentException("Heightmap length does not correspond with specified dimensions");
			}

			heightData = heightMap;
			size = dimensions;
		}

		public void Execute() {

			for (int x = 0; x < size.x; x++) {
				for (int z = 0; z < size.y; z++) {
					int2 pos = new int2(x, z);
					heightData[x + size.x * z] = GenerateHeight(pos);
				}
			}
		}

		/// <summary>
		/// Generates a height for <paramref name="position"/> using multiple octaves of noise.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <returns>The height at position.</returns>
		private byte GenerateHeight(int2 position) {
			float2 scalePosOct1 = position * new float2(0.003f, 0.005f);
			float2 scalePosOct2 = position * new float2(0.03f, 0.05f);
			float lowFreqNoise = noise.snoise(scalePosOct1);
			float highFreqNoise = noise.snoise(scalePosOct2);
			return (byte)math.clamp(10 + 8f * lowFreqNoise + 2f * lowFreqNoise * highFreqNoise + 0.3f * highFreqNoise, 0, 256);
		}
	}
}