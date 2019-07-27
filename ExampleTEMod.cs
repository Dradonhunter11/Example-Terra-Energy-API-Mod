using Microsoft.Xna.Framework;
using TerraEnergyLibrary.API.Enum;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleTEMod
{
	class ExampleTEMod : Mod
	{
		public ExampleTEMod()
		{
		}

		public static ModTileEntity GetTileEntity(float i, float j)
		{
			return GetTileEntity((int) i, (int) j);
		}

		public static ModTileEntity GetTileEntity(short i, short j)
		{
			return GetTileEntity((int)i, (int)j);
		}

		public static ModTileEntity GetTileEntity(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int top = j;
			int left = i;

			int index = -1;

			Point16 key = new Point16(left, top);
			if (TileEntity.ByPosition.ContainsKey(key))
			{
				
				TileEntity tileEntity = TileEntity.ByPosition[key];
				if (tileEntity != null)
				{
					index = tileEntity.ID;
				}
			}

			if (index == -1)
			{
				//Main.NewText("Not a tile entity");
				return null;
			}

			return TileEntity.ByID[index] as ModTileEntity;
		}

		public static Side GetBlockSide(int i, int j)
		{
			Vector2 position = Main.MouseWorld / 16;
			Tile tile = Main.tile[i, j];
			Vector2 tileWantedSide = new Vector2(i + TileObjectData.GetTileData(tile).Width / 2, j + TileObjectData.GetTileData(tile).Height / 2);
			float tileUp = (float)(j + TileObjectData.GetTileData(tile).Height - 0.2f);
			float tileDown = (float)(j + TileObjectData.GetTileData(tile).Height - TileObjectData.GetTileData(tile).Height + 0.2f);
			float tileLeft = (float)(i + TileObjectData.GetTileData(tile).Width - 0.2f);
			float tileRight = (float)(i + TileObjectData.GetTileData(tile).Width - TileObjectData.GetTileData(tile).Width + 0.2f);

			if (position.Y > tileUp)
			{
				Main.NewText("Down");
				return TerraEnergyLibrary.API.Enum.Side.down;
			}
			if (position.Y < tileDown)
			{
				Main.NewText("Up");
				return TerraEnergyLibrary.API.Enum.Side.up;
			}

			if (position.X > tileLeft)
			{
				Main.NewText("Right");
				return TerraEnergyLibrary.API.Enum.Side.right;
			}

			if (position.X < tileRight)
			{
				Main.NewText("Left");
				return TerraEnergyLibrary.API.Enum.Side.left;
			}

			return TerraEnergyLibrary.API.Enum.Side.none;
		}
	}
}
