using System.Collections.Generic;
using TycoonTerrain.Core;
using TycoonTerrain.Core.Generation;
using TycoonTerrain.Core.PaintOperations;
using TycoonTerrain.Core.TerrainOperations;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This is an example script that generates a few different terrains at startup and when pressing some of the number keys.
/// </summary>
[RequireComponent(typeof(TycoonTileMap))]
public class TestTerrains : MonoBehaviour {
	private TycoonTileMap terrain;

	private JobHandle terrainGenerationJob;
	private NativeArray<byte> heightmap;
	private int2 heightMapDimensions;

    // Start is called before the first frame update
    void Awake() {
	    terrain = GetComponent<TycoonTileMap>();

	    
    }

    void Start() {
		ExampleTerrain();
    }

    // Update is called once per frame
    void Update()
    {
		//Execute terrain modifications based on user input.
	    if (Input.GetKeyUp(KeyCode.Alpha1)) {
			ExampleTerrain();
	    }

	    if (Input.GetKeyUp(KeyCode.Alpha2)) {
			FlatTerrain();
	    }

	    if (Input.GetKeyUp(KeyCode.Alpha2)) {
		    WaterTestTerrain();
	    }
    }

	/// <summary>
	/// Generates an example terrain.
	/// </summary>
    private void ExampleTerrain() {
		heightMapDimensions = new int2(terrain.Width, terrain.Length) + new int2(1);
		heightmap = new NativeArray<byte>(heightMapDimensions.x * heightMapDimensions.y, Allocator.TempJob);
		terrainGenerationJob = new TerrainGenerationJob(heightmap, heightMapDimensions).Schedule();

	    terrainGenerationJob.Complete();
	    terrain.ScheduleOperation(new CopyHeightMapOperation(heightmap, heightMapDimensions));
	    heightmap.Dispose();

	    var positions = new NativeList<int2>(Allocator.TempJob);
	    terrain.ScheduleOperation(new BeachCheckOperation(7, positions));

	    var result = new NativeList<int2>(Allocator.Temp);
	    TrimBeach(positions, result);
	    positions.Dispose();
	    terrain.SchedulePaintOperation(new BeachPaintOperation(result, 2));

	    int2 waterSetPosition = new int2(81, 89);
	    terrain.ScheduleOperation(new CreateWaterBodyFloodOperation(waterSetPosition, 26, terrain.WaterHeightStepsPerTileHeight));
    }

	/// <summary>
	/// Generates a flat terrain.
	/// </summary>
    private void FlatTerrain() {
	    byte height = (byte)(terrain.MaxHeight / 2);

		terrain.ScheduleOperation(new ExecuteTileJobOperation<SetHeightOperation>(new SetHeightOperation(height), terrain.Bounds));
    }

	/// <summary>
	/// Creates a test terrain with 4 mountains in a square.
	/// </summary>
    private void WaterTestTerrain() {
	    byte height = (byte) (terrain.MaxHeight * 3 / 4);

	    int a = 8;
	    int b = 16;

		terrain.ScheduleOperation(new MaxHeightSmooth(new int2(a,a), height));
		terrain.ScheduleOperation(new MaxHeightSmooth(new int2(a,b), height));
		terrain.ScheduleOperation(new MaxHeightSmooth(new int2(b,a), height));
		terrain.ScheduleOperation(new MaxHeightSmooth(new int2(b,b), height));

		terrain.SchedulePaintOperation(new PaintTerrainBoundsOperation(new IntBound(new int2(a), new int2(b)), 1));
    }

	/// <summary>
	/// Removes the outer beach tile positions.
	/// </summary>
    private void TrimBeach(NativeArray<int2> beach, NativeList<int2> result) {
	    HashSet<int2> positions = new HashSet<int2>(beach);
	    NativeArray<int2> offsets = new NativeArray<int2>(4, Allocator.Temp);
	    offsets[0] = CardinalDirection.North.ToVector();
		offsets[1] = CardinalDirection.East.ToVector();
		offsets[2] = CardinalDirection.South.ToVector();
		offsets[3] = CardinalDirection.West.ToVector();

	    for (int i = 0; i < beach.Length; i++) {
		    int2 pos = beach[i];

		    int count = 0;
		    for (int j = 0; j < 4; j++) {
			    int2 neighbourPos = pos + offsets[j];

			    if (positions.Contains(neighbourPos) || !terrain.IsInBounds(neighbourPos)) {
				    count++;
			    }
		    }

			if(count == 4)
				result.Add(pos);
	    }
    }

	/// <summary>
	/// Generates tile positions for the beach based on height
	/// </summary>
	private struct BeachCheckOperation : ITerrainJob {
		private readonly byte maxHeight;
		private NativeList<int2> positions;

		public BeachCheckOperation(byte height, NativeList<int2> result) {
			maxHeight = height;
			positions = result;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			for (int x = 0; x < grid.Width; x++) {
				for (int z = 0; z < grid.Length; z++) {
					int2 pos = new int2(x, z);

					if(grid.GetTile(pos).GetData().GetHighestPoint() <= maxHeight)
						positions.Add(pos);
				}
			}
		}
	}

	/// <summary>
	/// Changes the terrain types based on previously found beach positions.
	/// </summary>
	private struct BeachPaintOperation : ITerrainPaintJob {
		private readonly ushort sandType;
		private readonly NativeArray<int2> positions;

		public BeachPaintOperation(NativeArray<int2> positions, ushort terrainType) {
			this.positions = positions;
			sandType = terrainType;
		}

		public void Execute(ref TerrainTypeTable terrainTypeTable, ref ChunkSet dirtyChunks) {
			for (int i = 0; i < positions.Length; i++) {
				int2 pos = positions[i];
				terrainTypeTable.SetTerrainType(pos, sandType);
			}
		}
	}
}
