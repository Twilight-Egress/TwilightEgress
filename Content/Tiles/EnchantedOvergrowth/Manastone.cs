﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Systems;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class Manastone : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override string LocalizationCategory => "Tiles.EnchantedOvergrowth";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[ModContent.TileType<OvergrowthDirt>()][Type] = true;

            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            RegisterItemDrop(ModContent.ItemType<ManastoneItem>());
            AddMapEntry(new Color(49, 42, 146));

            glowTexture = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/Manastone_Glow");
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
            => WorldGen.TileMergeAttempt(-2, ModContent.TileType<OvergrowthDirt>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            float randomForTile = TwilightEgressUtilities.RandomFromVector(new Vector2(i, j));

            if (randomForTile <= 0.12f)
                tile.TileFrameY += 270;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;

            Vector2 drawPosition = new Vector2(i * 16, j * 16) - Main.screenPosition;
            //drawPosition += new Vector2(randomForTile, TwilightEgressUtilities.RandomFromVector(new Vector2(j, i))) * 16f;
            if (!Main.drawToScreen)
                drawPosition += new Vector2(Main.offScreenRange);

            Color drawColor = WorldGen.paintColor(Framing.GetTileSafely(i, j).TileColor) * (float)(1 + NoiseSystem.cellular.GetNoise(i * 5, j * 5, Main.GameUpdateCount));

            Color lightColor = Lighting.GetColor(i, j);
            drawColor.R = (byte)Math.Clamp(drawColor.R + lightColor.R, 0, 255);
            drawColor.G = (byte)Math.Clamp(drawColor.G + lightColor.G, 0, 255);
            drawColor.B = (byte)Math.Clamp(drawColor.B + lightColor.B, 0, 255);

            spriteBatch.Draw(glowTexture.Value, drawPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
    }

    public class ManastoneItem : ModItem
    {
        public override string LocalizationCategory => "Tiles.EnchantedOvergrowth.Manastone.Items";

        public override string Texture => "TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/Manastone_Item";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Manastone>());
            Item.width = 16;
            Item.height = 16;
        }
    }
}