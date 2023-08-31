using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace RLTY.Customisation
{
    [AddComponentMenu("RLTY/Customisable/Processors/Image")]
    public class ImageProcessor : Processor
    {
        #region Global Variables
        public Vector3 spriteDimensions;
        public Texture2D texture2D;

        public bool addEquiBordersToSprite;
        public float equiBordersWidth = 1f;
        #endregion

        #region Logic
        [Button("Test")]
        public void ConstrainedSpriteSwap()
        {
            if (TryGetComponent<Image>(out Image img))
            {
                if (img.sprite)
                    GetSpriteDimensions();
                SwapSprite();
            }
        }

        [Button("Get spriteRenderer dimensions")]
        public void GetSpriteDimensions()
        {
            Image image;

            if (TryGetComponent<Image>(out Image img))
            {
                image = img;
                Bounds spriteBounds = image.sprite.bounds;
                spriteDimensions = spriteBounds.size;
            }
        }

        public Sprite SetUpSprite(Texture2D tex)
        {
            Rect texturePart = new Rect(0.0f, 0.0f, tex.width, tex.height);

            Vector4 borders;
            if (addEquiBordersToSprite)
                borders = new Vector4(equiBordersWidth, equiBordersWidth, equiBordersWidth, equiBordersWidth);
            else
                borders = Vector4.zero;

            Vector2 pivotPosition = new Vector2(0.5f, 0.5f);
            int pixelsToUnitsRatio = 100;

            return Sprite.Create(tex, texturePart, pivotPosition, pixelsToUnitsRatio, 0, SpriteMeshType.FullRect, borders);
            // Borders could be used to add padding, but it might be better if it's done on the web API
            // Each customisable will have its own set of dimensions to adjust the sprite to.
        }

        public void SwapSprite()
        {
            Image image = GetComponent<Image>();
            image.sprite = SetUpSprite(texture2D);
        }
        #endregion

    }
}
