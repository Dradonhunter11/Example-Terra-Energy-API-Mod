using System.Collections.Generic;
using System.IO;
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
using Terraria.World.Generation;

namespace ExampleTEMod.Tiles
{
    public class ExampleWire : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<ExampleWireTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            drop = mod.ItemType("ExampleWire");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            ModTileEntity up = ExampleTEMod.GetTileEntity(i, j - 1);
            ModTileEntity down = ExampleTEMod.GetTileEntity(i, j + 1);
            ModTileEntity left = ExampleTEMod.GetTileEntity(i - 1, j);
            ModTileEntity right = ExampleTEMod.GetTileEntity(i + 1, j);

            int frameX = 0;
            int frameY = 0;
            if (WorldGen.InWorld(i - 1, j) && Main.tile[i - 1, j].active() && /*left is TileEntityEnergyHandler*/ Main.tile[i - 1, j].type == Type)
            {
                frameX += 18;
            }
            if (WorldGen.InWorld(i + 1, j) && Main.tile[i + 1, j].active() && /*right is TileEntityEnergyHandler*/ Main.tile[i + 1, j].type == Type)
            {
                frameX += 36;
            }
            if (WorldGen.InWorld(i, j - 1) && Main.tile[i, j - 1].active() && /*up is TileEntityEnergyHandler*/  Main.tile[i, j - 1].type == Type)
            {
                frameY += 18;
            }
            if (WorldGen.InWorld(i, j + 1) && Main.tile[i, j + 1].active() && /*down is TileEntityEnergyHandler*/ Main.tile[i, j + 1].type == Type)
            {
                frameY += 36;
            }
            Main.tile[i, j].frameX = (short)frameX;
            Main.tile[i, j].frameY = (short)frameY;
            return false;
        }

        public override void RightClick(int i, int j)
        {
            ModTileEntity tileEntity = ExampleTEMod.GetTileEntity(i, j);
            if (tileEntity is ExampleWireTileEntity wire)
            {
                Side side = ExampleTEMod.GetBlockSide(i, j);
                if (side != Side.none)
                {
                    wire.IO[side] = !wire.IO[side];
                }
                Main.NewText(wire.storage.GetCurrentEnergy() + "/" + wire.storage.MaxEnergy + " TE");
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D input = mod.GetTexture("Tiles/ExampleWireConnectorInput");
            Texture2D output = mod.GetTexture("Tiles/ExampleWireConnectorOutput");
            ModTileEntity up = ExampleTEMod.GetTileEntity(i, j - 1);
            ModTileEntity down = ExampleTEMod.GetTileEntity(i, j + 1);
            ModTileEntity left = ExampleTEMod.GetTileEntity(i - 1, j);
            ModTileEntity right = ExampleTEMod.GetTileEntity(i + 1, j);
            ExampleWireTileEntity itself = ExampleTEMod.GetTileEntity(i, j) as ExampleWireTileEntity;

            

            if (up is EnergyHandler handlerUp && !(up is ExampleWireTileEntity) && handlerUp.CanConnect(Side.down))
            {
                if (itself.IO[Side.up])
                    spriteBatch.Draw(input, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(0, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
                 else
                    spriteBatch.Draw(output, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(0, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
            }
            if (down is EnergyHandler handlerDown && !(down is ExampleWireTileEntity) && handlerDown.CanConnect(Side.up))
            {
                if (itself.IO[Side.down])
                    spriteBatch.Draw(input, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(18, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(output, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(18, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
            }            
            if (right is EnergyHandler handlerRight && !(right is ExampleWireTileEntity) && handlerRight.CanConnect(Side.left))
            {
                if (itself.IO[Side.right])
                    spriteBatch.Draw(input, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(36, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(output, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(36, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
            }
            if (left is EnergyHandler handlerLeft && !(left is ExampleWireTileEntity) && handlerLeft.CanConnect(Side.right))
            {
                if (itself.IO[Side.left])
                    spriteBatch.Draw(input, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(54, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(output, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y), new Rectangle(54, 0, 16, 16), Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
            {
                return;
            }
            mod.GetTileEntity("ExampleWireTileEntity").Kill(i, j);
        }
    }

    public class ExampleWireTileEntity : ModTileEntity, EnergyReceiver, BasicDataStorage
    {
        private int updateTimer = 3;
        public readonly Dictionary<Side, bool> IO;
        public EnergyContainer storage;

        private long maxTransferRate = 0; 

        public ExampleWireTileEntity()
        {
            storage = new EnergyContainer(1024, 32, 16);
            IO = new Dictionary<Side, bool>();
            IO.Add(Side.up, true);
            IO.Add(Side.left, true);
            IO.Add(Side.right, true);
            IO.Add(Side.down, true);
            maxTransferRate = 16;
            tag = new TagCompound();
        }

        public override void Update()
        {
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

            storage.WriteTagCompound(tag);
            WriteDataToTagCompound();

            if (Main.netMode == NetmodeID.Server)
            {
                updateTimer--;
                if (updateTimer == 0)
                {
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
                    updateTimer = 3;
                }
            }
        }

        private void ProcessSide(ModTileEntity tileEntity, Side side, Side machineSide, ref long usedEnergy)
        {
            EnergyReceiver energyReceiver;
            if (tileEntity is EnergyReceiver)
            {
                energyReceiver = tileEntity as EnergyReceiver;
                if ((energyReceiver.CanConnect(machineSide) && !IO[side]) || energyReceiver is ExampleWireTileEntity)
                {
                    usedEnergy += SendEnergy(energyReceiver, storage.MaxTransfer - storage.GetCurrentEnergy(), side);
                }
            }
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

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<ExampleWire>();
        }

        public void WriteDataToTagCompound()
        {
            tag["Up"] = IO[Side.up];
            tag["Down"] = IO[Side.down];
            tag["Left"] = IO[Side.left];
            tag["Right"] = IO[Side.right];
        }

        public void ReadDataToTagCompound()
        {
            IO[Side.up] = tag.GetBool("Up");
            IO[Side.down] = tag.GetBool("Down");
            IO[Side.left] = tag.GetBool("Left");
            IO[Side.right] = tag.GetBool("Right");
        }

        public bool CanConnect(Side side)
        {
            return IO[side];
        }

        public long GetEnergyStored(Tile tile)
        {
            return storage.GetCurrentEnergy();
        }

        public long GetMaxEnergyStorage(Tile tile)
        {
            return storage.MaxEnergy;
        }

        public long ReceiveEnergy(long maxTransfer, Side side)
        {
            return storage.ReceiveEnergy(maxTransfer);
        }

        public bool canReceive(long maxTransfer)
        {
            return true;
        }

        public long SendEnergy(EnergyReceiver energyReceiver, long maxReceive, Side side)
        {
            return energyReceiver.ReceiveEnergy(maxReceive, side);
        }

        public bool HasTagCompound()
        {
            return tag != null;
        }

        public void SetTagCompound(TagCompound tag)
        {
            this.tag = tag;
        }

        public TagCompound tag { get; internal set; }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            MemoryStream memoryStream = new MemoryStream(65536);
            WriteDataToTagCompound();
            storage.WriteTagCompound(this.tag);
            TagIO.ToStream(this.tag, (Stream)memoryStream, true);
            writer.Write((ushort)memoryStream.Length);
            writer.Write(memoryStream.ToArray());

        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            int count = (int)reader.ReadUInt16();
            this.tag = TagIO.FromStream((Stream)new MemoryStream(reader.ReadBytes(count)), true);
            ErrorLogger.Log((object)this.tag.Count, false);
            storage.ReadTagCompound(this.tag);
            ReadDataToTagCompound();
        }
    }
}
