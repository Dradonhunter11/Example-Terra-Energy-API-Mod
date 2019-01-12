using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraEnergyLibrary.API;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
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
            TileObjectData.newTile.Origin = new Point16(1, 1);
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

            Tile tile = Main.tile[i, j];

            int left = i - (tile.frameX / 18);
            int top = j - (tile.frameY / 18);

            int index = mod.GetTileEntity<GeneratorTileEntity>().Find(left, top);

            if (index == -1)
            {
                Main.NewText("An error happened");
                return;
            }

            GeneratorTileEntity tileEntity = TileEntity.ByID[index] as GeneratorTileEntity;
            Main.NewText(tileEntity.EnergyContainer.GetCurrentEnergy() + "/" + tileEntity.EnergyContainer.MaxEnergy + " TE");
        }
    }

    class GeneratorTileEntity : TileEntityEnergyHandler
    {
        /// <summary>
        /// A timer between each synchronization in multiplayer, in this case it will do 20 time per second. 
        /// </summary>
        public int updateTimer = 3;
        /// <summary>
        /// Here we set the capacity of the tile entity, the max transfer rate and the max receive rate of the the tile entity
        /// </summary>
        public GeneratorTileEntity() : base(40000, 4, 16)
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
                if (EnergyContainer.GetCurrentEnergy() != EnergyContainer.MaxEnergy)
                {
                    this.EnergyContainer.ReceiveEnergy(4);
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    updateTimer--;

                    if (updateTimer <= 0)
                    {
                        NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
                        updateTimer = 3;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
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
