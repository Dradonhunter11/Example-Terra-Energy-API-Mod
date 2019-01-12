using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleTEMod.Items
{
	//Standard ModItem class
	public class Generator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Generator");
			Tooltip.SetDefault("This is a block that can produce terra energy (TE).\nProduce 4 TE per tick.");
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
			item.createTile = mod.TileType("Generator");
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
