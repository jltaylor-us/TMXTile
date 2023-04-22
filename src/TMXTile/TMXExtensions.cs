using System;
using System.Globalization;
using System.Linq;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace TMXTile
{
    public struct DrawInstructions
    {
        public float Rotation;
        public int Effect;
        public Location Offset;
        public TMXColor Color;
        public float Opacity;
    }

    public static class TMXExtensions
    {

        public static bool IsImageLayer(this Layer layer)
        {
            if (layer.Properties.TryGetParsed("@ImageLayer", out bool value))
                return value;

            return false;
        }

        public static void MakeImageLayer(this Layer layer)
        {
            layer.Properties["@ImageLayer"] = true.ToString();
        }

        public static TileSheet GetTileSheetForImageLayer(this Layer layer)
        {
            if (!layer.IsImageLayer())
                return null;

            if (layer.Properties.TryGetValue("@ImageLayerTileSheet", out string value) && layer.Map.TileSheets.FirstOrDefault(t => t.Id == value) is TileSheet ts)
                return ts;

            return null;
        }

        public static void SetTileSheetForImageLayer(this Layer layer, TileSheet tileSheet)
        {
            layer.Properties["@ImageLayerTileSheet"] = tileSheet.Id;
        }

        public static Location GetOffset(this Layer layer)
        {
            return layer.Properties.GetOffsetProperties();
        }

        public static Location GetOffset(this Tile tile)
        {
            return tile.Properties.GetOffsetProperties();
        }


        public static void SetOffset(this Layer layer, Location offset)
        {
            layer.Properties.SetOffsetProperties(offset);
        }

        public static void SetOffset(this Tile tile, Location offset)
        {
            tile.Properties.SetOffsetProperties(offset);
        }

        public static float GetOpacity(this Layer layer)
        {
            if (layer.Properties.TryGetParsed("@Opacity", out float value))
                return value;

            return 1f;
        }

        public static float GetOpacity(this Tile tile)
        {
            if (tile.Properties.TryGetParsed("@Opacity", out float value))
                return value;

            return 1f;
        }

        public static void SetOpacity(this Layer layer, float opacity)
        {
            layer.Properties["@Opacity"] = opacity.ToString(CultureInfo.InvariantCulture);
        }

        public static void SetOpacity(this Tile tile, float opacity)
        {
            tile.Properties["@Opacity"] = opacity.ToString(CultureInfo.InvariantCulture);
        }

        public static TMXColor GetColor(this Layer layer)
        {
            return layer.Properties.GetColorFromProperty("@Color");
        }

        public static TMXColor GetColor(this Tile tile)
        {
            return tile.Properties.GetColorFromProperty("@Color");
        }

        public static TMXColor GetColor(this Map map)
        {
            return map.Properties.GetColorFromProperty("@Color");
        }

        public static void SetColor(this Layer layer, TMXColor color)
        {
            layer.Properties["@Color"] = color.ToString();
        }

        public static void SetColor(this Tile tile, TMXColor color)
        {
            tile.Properties["@Color"] = color.ToString();
        }

        public static void SetColor(this Map map, TMXColor color)
        {
            map.Properties["@Color"] = color.ToString();
        }

        public static int GetRotationValue(this Tile tile)
        {
            if (tile.Properties.TryGetParsed("@Rotation", out int value))
                return value;

            return 0;
        }

        public static void SetRotationValue(this Tile tile, int rotation)
        {
            tile.Properties["@Rotation"] = rotation.ToString();
        }

        public static float GetRotation(this Tile tile)
        {
            float value = tile.GetRotationValue();

            value %= 360;
            if (value == 0)
                return 0;

            return (float)(Math.PI / (180.0 / value));
        }

        public static int GetFlip(this Tile tile)
        {
            if (tile.Properties.TryGetParsed("@Flip", out int value))
                return value;

            return 0;
        }

        public static void SetFlip(this Tile tile, int flip)
        {
            tile.Properties["@Flip"] = flip.ToString();
        }


        public static TMXColor GetBackgroundColor(this Map map)
        {
            return map.Properties.GetColorFromProperty("@BackgroundColor");
        }

        public static TMXColor SetBackgroundColor(this Map map, TMXColor color)
        {
            map.Properties["@BackgroundColor"] = color.ToString();

            return null;
        }

        public static TMXColor GetTransparentColor(this TileSheet tilesheet)
        {
            return tilesheet.Properties.GetColorFromProperty("@TColor");
        }

        public static TMXColor GetColorFromProperty(this IPropertyCollection collection, string key)
        {
            if (collection.TryGetValue(key, out string value))
                return TMXColor.FromString(value);

            return null;
        }

        public static Location GetOffsetProperties(this IPropertyCollection collection)
        {
            int x = 0;
            int y = 0;

            if (collection.TryGetParsed("@OffsetX", out int xValue))
                x = xValue;

            if (collection.TryGetParsed("@OffsetY", out int yValue))
                y = yValue;

            return new Location(
                x,
                y
                );
        }

        public static void SetOffsetProperties(this IPropertyCollection collection, Location offset)
        {
            collection["@OffsetX"] = offset.X.ToString();
            collection["@OffsetY"] = offset.Y.ToString();
        }

        public static bool TryGetValue<T>(this IPropertyCollection collection, string key, Func<string, T> parse, out T value)
        {
            if (collection.TryGetValue(key, out string raw))
            {
                value = parse(raw);
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetParsed(this IPropertyCollection collection, string key, out bool value)
        {
            return collection.TryGetValue(key, bool.Parse, out value);
        }

        public static bool TryGetParsed(this IPropertyCollection collection, string key, out float value)
        {
            return collection.TryGetValue(key, float.Parse, out value);
        }

        public static bool TryGetParsed(this IPropertyCollection collection, string key, out int value)
        {
            return collection.TryGetValue(key, int.Parse, out value);
        }

        public static DrawInstructions GetDrawInstructions(this Tile tile)
        {
            return new DrawInstructions()
            {
                Rotation = tile.GetRotation(),
                Effect = tile.GetFlip(),
                Offset = tile.Layer.GetOffset() + tile.GetOffset(),
                Color = tile.GetColor() ?? tile.Layer.GetColor() ?? tile.Layer.Map.GetColor(),
                Opacity = tile.Layer.GetOpacity() * tile.GetOpacity()
            };
        }

        public static void SetupImageLayer(this Layer layer)
        {
            if (!layer.IsImageLayer())
                return;

            if (xTile.Format.FormatManager.Instance?.GetMapFormatByExtension("tmx") is TMXFormat tmxFormat)
            {
                layer.AfterDraw -= tmxFormat.ImageLayer_AfterDraw;
                layer.AfterDraw += tmxFormat.ImageLayer_AfterDraw;
            }
        }
    }
}
