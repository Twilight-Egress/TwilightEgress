﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthTree : ModTile
    {
        private Asset<Texture2D> topsTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));

            topsTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Tiles/EnchantedOvergrowth/OvergrowthTree_Tops");
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Framing.GetTileSafely(i, j - 1).TileType == Type)
                return;

            try
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
            catch { }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xOffset = 36 * (i % 4);
            Rectangle sourceRectangle = new Rectangle(xOffset, 0, 36, 70);
            Color paintColor = WorldGen.paintColor(Framing.GetTileSafely(i, j).TileColor);
            spriteBatch.DrawTileTexture(topsTexture.Value, i, j, sourceRectangle, paintColor, 0.0f, new(10, 70));


            WorldGen.GetTreeFrame(Framing.GetTileSafely(i, j));
        }
    }
}