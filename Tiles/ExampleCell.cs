using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraEnergyLibrary.API;
using TerraEnergyLibrary.API.Interface;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleTEMod.Tiles
{
    class ExampleCell : ModTile
    {
        /// <summary>
        /// A standard set default.
        /// Do note that setting the origin is REALLY important
        /// </summary>
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<ExampleCellTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
        }

        /// <summary>
        /// Standard right click funtion, it get the top left corner of the tile and see if there is a generator tile entity.
        /// If there is one, it will show how much energy it has generated, otherwise it will return an error.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public override void RightClick(int i, int j)
        {

            Tile tile = Main.tile[i, j];

            int left = i - (tile.frameX / 18);
            int top = j - (tile.frameY / 18);

            int index = mod.GetTileEntity<ExampleCellTileEntity>().Find(left, top);

            if (index == -1)
            {
                Main.NewText("An error happened");
                return;
            }

            ExampleCellTileEntity tileEntity = TileEntity.ByID[index] as ExampleCellTileEntity;
            Main.NewText("Example cell energy storage", Color.Green);
            Main.NewText(tileEntity.EnergyContainer.GetCurrentEnergy() + "/" + tileEntity.EnergyContainer.MaxEnergy + " TE");
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
            {
                return;
            }
            mod.GetTileEntity("ExampleCellTileEntity").Kill(i, j);
        }
    }

    class ExampleCellTileEntity : TileEntityEnergyHandler
    {
        private int updateTimer = 3;
        public ExampleCellTileEntity() : base(1000000, 128)
        {
        }

        public override void Update()
        {
            ModTileEntity up = ExampleTEMod.GetTileEntity(Position.X, Position.Y - 1);
            ModTileEntity down = ExampleTEMod.GetTileEntity(Position.X, Position.Y + 1);
            ModTileEntity left = ExampleTEMod.GetTileEntity(Position.X - 1, Position.Y);
            ModTileEntity right = ExampleTEMod.GetTileEntity(Position.X + 1, Position.Y);


            if (Main.netMode == NetmodeID.Server)
            {
                updateTimer--;
                if (updateTimer == 0)
                {
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
                    updateTimer = 3;
                }
            }
            EnergyContainer.WriteTagCompound(tag);
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<ExampleCell>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
    }
    
}
