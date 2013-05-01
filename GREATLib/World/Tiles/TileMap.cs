//
//  TileMap.cs
//
//  Author:
//       Jesse <>
//
//  Copyright (c) 2013 Jesse
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATLib.World.Tiles
{
	/// <summary>
	/// Represents a 2D tile map, to hold the collision data and such of a 
	/// platformer map.
	/// </summary>
    public class TileMap
    {
		/// <summary>
		/// Gets or sets the list of rows (which is a list of tiles).
		/// </summary>
		/// <value>The tile rows.</value>
		public List<List<Tile>> TileRows { get; private set; }

		/// <summary>
		/// Gets or sets the rectangles of the tiles.
		/// </summary>
		/// <value>The tile rectangles.</value>
		private Dictionary<Tile, Rect> TileRectangles { get; set; }

        public TileMap()
        {
			TileRows = GetDummyData();
			InitMap();
        }

		/// <summary>
		/// Returns a dummy testing tilemap.
		/// </summary>
		/// <returns>The dummy data.</returns>
		private List<List<Tile>> GetDummyData()
		{
			// Temporary code to get a quick-and-dirty map.
			List<int> ids = GeneralHelper.MakeList(0, 1);
			List<CollisionType> collisions = GeneralHelper.MakeList(
				CollisionType.Passable, CollisionType.Block);

			List<List<int>> tiles = GeneralHelper.MakeList(
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 1, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 1, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 1, 0, 0, 0, 1, 1),
				GeneralHelper.MakeList(1, 0, 0, 1, 1, 1, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

			return tiles.ConvertAll(
				row => row.ConvertAll(
					id => new Tile(id, collisions[ids.IndexOf(id)])));
		}

		private void InitMap()
		{
			TileRectangles = new Dictionary<Tile, Rect>();
			for (int y = 0; y < GetHeightTiles(); ++y)
			{
				for (int x = 0; x < GetWidthTiles(); ++x)
				{
					TileRectangles.Add(TileRows[y][x],
						new Rect(x * Tile.WIDTH,
					         y * Tile.HEIGHT,
					         Tile.WIDTH,
					         Tile.HEIGHT));
				}
			}
		}

		/// <summary>
		/// Gets the touched tiles by a passed rectangle.
		/// </summary>
		/// <returns>The touched tiles.</returns>
		/// <param name="left">Left side of the rectangle.</param>
		/// <param name="top">Top side of the rectangle.</param>
		/// <param name="width">Width of the rectangle.</param>
		/// <param name="height">Height of the rectangle.</param>
		public List<KeyValuePair<Rect, CollisionType>> GetTouchedTiles(Rect rectangle)
		{
			Debug.Assert(TileRectangles != null, "Map not initialized.");

			// Get the start/end indices of the tiles that our rectangle touches
			int startX = GeneralHelper.Clamp((int)rectangle.Left / Tile.WIDTH,
			                                 0, GetWidthTiles() - 1);
			int startY = GeneralHelper.Clamp((int)rectangle.Top / Tile.HEIGHT,
			                                 0, GetHeightTiles() - 1);
			int endX = GeneralHelper.Clamp((int)Math.Ceiling((double)rectangle.Right / Tile.WIDTH) - 1,
			                               0, GetWidthTiles() - 1);
			int endY = GeneralHelper.Clamp((int)Math.Ceiling((double)rectangle.Bottom / Tile.HEIGHT) - 1,
			                               0, GetHeightTiles() - 1);

			List<KeyValuePair<Rect, CollisionType>> touched = new List<KeyValuePair<Rect, CollisionType>>();

			for (int y = startY; y <= endY; ++y)
			{
				for (int x = startX; x <= endX; ++x)
				{
					CollisionType collision = TileRows[y][x].Collision;
					if (collision != CollisionType.Passable) // we have a collision
					{
						Debug.Assert(TileRectangles.ContainsKey(TileRows[y][x]), "Tile rectangle not created.");

						Rect tileRect = TileRectangles[TileRows[y][x]];
						touched.Add(new KeyValuePair<Rect, CollisionType>(
							tileRect, collision));
					}
				}
			}

			return touched;
		}

		/// <summary>
		/// Gets the width of the tilemap, in tiles.
		/// </summary>
		/// <returns>The width in tiles.</returns>
		public int GetWidthTiles()
		{
			return TileRows.Count > 0 ? TileRows[0].Count : 0;
		}
		/// <summary>
		/// Gets the height of the tilemap, in tiles.
		/// </summary>
		/// <returns>The height in tiles.</returns>
		public int GetHeightTiles()
		{
			return TileRows.Count;
		}

		/// <summary>
		/// Checks if it is a valid x index.
		/// </summary>
		public bool IsValidXIndex(int x)
		{
			return x >= 0 && x < GetWidthTiles();
		}
		/// <summary>
		/// Checks if it is a valid y index.
		/// </summary>
		public bool IsValidYIndex(int y)
		{
			return y >= 0 && y < GetHeightTiles();
		}

		/// <summary>
		/// Gets the x index of the tile (as a rectangle).
		/// </summary>
		public int GetTileXIndex(Rect rect)
		{
			return (int)(rect.Left / Tile.WIDTH);
		}
		/// <summary>
		/// Gets the y index of the tile (as a rectangle).
		/// </summary>
		public int GetTileYIndex(Rect rect)
		{
			return (int)(rect.Top / Tile.HEIGHT);
		}

		/// <summary>
		/// Gets the collision of the given tile.
		/// IF the tile is out of the tilemap, it returns a blocking collision.
		/// </summary>
		public CollisionType GetCollision(int tileX, int tileY)
		{
			return IsValidXIndex(tileX) && IsValidYIndex(tileY) ?
				TileRows[tileY][tileX].Collision : CollisionType.Block;
		}
    }
}

