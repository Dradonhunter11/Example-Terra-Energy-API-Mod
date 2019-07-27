using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleTEMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraEnergyLibrary.API;
using TerraEnergyLibrary.API.Enum;
using TerraEnergyLibrary.API.Interface;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ExampleTEMod.Tiles
{
    class Generator : ModTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<GeneratorTileEntity>().Hook_AfterPlacement, -1, 0, false);
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
            GeneratorTileEntity tileEntity = ExampleTEMod.GetTileEntity(i, j) as GeneratorTileEntity;
            if (tileEntity != null)
            {
                Main.NewText(tileEntity.GetEnergyStored(null) + "/" + tileEntity.GetMaxEnergyStorage(null) + " TE");
            }
        }

        

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
            {
                return;
            }
            mod.GetTileEntity("GeneratorTileEntity").Kill(i, j);
        }
    }

    class GeneratorTileEntity : ProviderTileEntity
    {
        /// <summary>
        /// A timer between each synchronization in multiplayer, in this case it will do 20 time per second. 
        /// </summary>
        public int updateTimer = 3;
        /// <summary>
        /// Here we set the capacity of the tile entity, the max transfer rate and the max receive rate of the the tile entity
        /// </summary>

        public GeneratorTileEntity() : base(40000, 16)
        {
        }

        /// <summary>
        /// Generate energy as long the generator is not full.
        /// If it's on server the timer will go on and sync the data every time it reach 0
        /// </summary>
        public override void Update()
        {
            try
            {
                if (storage.GetCurrentEnergy() != storage.MaxEnergy)
                {
                    storage.ModifyEnergy(4);
                }
                
                ModTileEntity up = ExampleTEMod.GetTileEntity(Position.X, Position.Y - 1);
                ModTileEntity down = ExampleTEMod.GetTileEntity(Position.X, Position.Y + 1);
                ModTileEntity left = ExampleTEMod.GetTileEntity(Position.X - 1, Position.Y);
                ModTileEntity right = ExampleTEMod.GetTileEntity(Position.X + 1, Position.Y);

                long usedEnergy = 0;

                ProcessSide(up, Side.up, Side.down, ref usedEnergy);
                ProcessSide(down, Side.down, Side.up, ref usedEnergy);
                ProcessSide(left, Side.left, Side.right, ref usedEnergy);
                ProcessSide(right, Side.right, Side.left, ref usedEnergy);

                storage.TransferEnergy(usedEnergy);

                if (Main.netMode == NetmodeID.Server)
                {
                    updateTimer--;

                    if (updateTimer <= 0)
                    {
                        NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
                        updateTimer = 3;
                    }
                }  
                storage.WriteTagCompound(tag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void ProcessSide(ModTileEntity tileEntity, Side side, Side machineSide, ref long usedEnergy)
        {
            if (tileEntity is EnergyReceiver handler && handler.CanConnect(machineSide))
            {
                usedEnergy += SendEnergy(handler, storage.MaxTransfer, machineSide);
            }
        }

        /// <summary>
        /// Standard tile entity stuff
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<Generator>();
        }

        /// <summary>
        /// Standard Hook_AfterPlacement
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="type"></param>
        /// <param name="style"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
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
