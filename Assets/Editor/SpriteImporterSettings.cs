using UnityEngine;
using UnityEditor;

public class SpriteImporterSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        if (importer.assetPath.Contains("/Sprites/")) // só aplica se estiver na pasta "Sprites"
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32; // Mude aqui para o seu padrão, ex: 16, 32, 100...
            importer.spritePivot = new Vector2(0.5f, 0.5f); // Pivot central
            importer.filterMode = FilterMode.Point; // Pixel art
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
        }
    }
}
