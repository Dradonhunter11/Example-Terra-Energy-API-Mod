using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TerraEnergyLibrary.API;
using TerraEnergyLibrary.API.Enum;
using TerraEnergyLibrary.API.Interface;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleTEMod.TileEntities
{
    public abstract class ProviderTileEntity : ModTileEntity, EnergyProvider, BasicDataStorage
    {
        public readonly EnergyContainer storage;
        public readonly Dictionary<Side, bool> IO;


        public ProviderTileEntity(long capacity) : this(capacity, capacity)
        {
        }

        public ProviderTileEntity(long capacity, long maxTransfer)
        {
            tag = new TagCompound();
            storage = new EnergyContainer(capacity, maxTransfer, 0);
            IO = new Dictionary<Side, bool>();
            IO.Add(Side.down, true);
            IO.Add(Side.up, true);
            IO.Add(Side.left, true);
            IO.Add(Side.right, true);
        }

        public virtual bool CanConnect(Side side)
        {
            return true;
        }

        public long GetEnergyStored(Tile tile)
        {
            return storage.GetCurrentEnergy();
        }

        public long GetMaxEnergyStorage(Tile tile)
        {
            return storage.MaxEnergy;
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
        public long TransferEnergy(long maxTransfer, Side side)
        {
            if (!IO[side])
            {
                return storage.TransferEnergy(maxTransfer);
            }
            return 0;
        }

        public bool canProvide(long maxTransfer)
        {
            return true;
        }

        public long SendEnergy(EnergyReceiver receiver, long amount, Side side)
        {
            return receiver.ReceiveEnergy(amount, side);
        }
    }
}
