using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleTEMod.Items
{
    class ExampleWire : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Wire");
            Tooltip.SetDefault("This is a wire that can Transfer/Receive Terra Energy (TE).\nTransfer/Receive 16 TE per tick.");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = 2;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.createTile = mod.TileType("ExampleWire");
            
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
