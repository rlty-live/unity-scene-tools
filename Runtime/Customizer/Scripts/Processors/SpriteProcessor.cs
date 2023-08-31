using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using JetBrains.Annotations;
using RLTY.SessionInfo;

namespace RLTY.Customisation
{
    [AddComponentMenu("RLTY/Customisable/Processors/Sprite"), HideMonoScript]
    public class SpriteProcessor : Processor
    {
        #region Global Variables
        private SpriteRenderer SpriteRenderer { get { return GetComponent<SpriteRenderer>(); } }
        private Image Image { get { return GetComponent<Image>(); } }

        [Title("Parameters")]
        [SerializeField, ShowIf("equiBordersWidth", true)]
        public float equiBordersWidth = 1f;
        [SerializeField, ShowIf("showUtilities")]
        private bool addBorders = true;
        [SerializeField, Range(10, 400)]
        private float minPixelPerUnit = 100;
        [SerializeField, Tooltip("If new Sprite is narrower than placeholder it will be aligned according to selection.")]
        private HorizontalAlignment horizontalAlignment;
        [SerializeField, Tooltip("If new Sprite is smaller than placeholder it will be aligned according to selection.")]
        private VerticalAlignment verticalAlignment;


        [Title("Placeholder")]
        [InfoBox("Cannot have any children for now.", InfoMessageType.Warning)]
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("World dimensions")]
        private Vector2 placeholderSpriteWorldDimensions;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("world Sprite ratio")]
        public float placeHolderWorldRatio;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("Proportions")]
        private TextureProportions placeholderProportions;
        [SerializeField, ShowIf("showUtilities")]
        private bool displayBounds = true;

        private Vector2 placeHolderSpriteDimensions;
        private float placeHolderRatio;
        private float placeholderPPU;

        [Title("New sprite")]
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("Sprite dimensions")]
        private Vector2 newSpriteDimensions;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("Ratio")]
        private float newTextureRatio;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("Proportions")]
        private TextureProportions newSpriteProportions;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        [LabelText("World dimensions")]
        public Vector2 newSpriteWorldDimensions;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        float newRatio;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        bool isOffset = false;
        Vector3 PositionOffset;
        float offset;

        #endregion

        public override Component FindComponent()
        {
            Component target = null;

            if (SpriteRenderer==null && Image==null)
            {
                if (debug)
                    Debug.LogWarning("No SpriteRenderer found in children" + commonWarning, this);
            }
            else
            {
                if (SpriteRenderer!=null)
                    target = SpriteRenderer;

                if (Image)
                    target = Image;
            }
            return target;
        }

        public override void Customize(KeyValueBase keyValue)
        {
            Texture t = keyValue.data as Texture;
            if (t == null)
                return;
            if (SpriteRenderer!=null)
                SpriteRendererSpriteSwap(t);
            else
            if (Image!=null)
                ImageSpriteSwap(t);
        }

        #region Runtime Logic
        //[Button("Measure")]
        public void GetSpriteDimensions()
        {
            if (SpriteRenderer == null || SpriteRenderer.sprite == null)
                return;

            placeholderPPU = SpriteRenderer.sprite.pixelsPerUnit;
            placeHolderRatio = SpriteRenderer.sprite.texture.width / SpriteRenderer.sprite.texture.height;
            placeHolderSpriteDimensions = new Vector2(SpriteRenderer.sprite.texture.width / placeholderPPU, SpriteRenderer.sprite.texture.height / placeholderPPU);
            placeholderSpriteWorldDimensions = placeHolderSpriteDimensions * transform.localScale;
            placeHolderWorldRatio = placeholderSpriteWorldDimensions.x / placeholderSpriteWorldDimensions.y;

            //Local bounds seems to only use the original sprite size, at least in editor (check the Gizmos in editor)
            //spriteWorldDimensions = new Vector2(spriteRenderer.localBounds.size.x, spriteRenderer.localBounds.size.y);

            float spriteRatio = placeholderSpriteWorldDimensions.x / placeholderSpriteWorldDimensions.y;

            switch (spriteRatio)
            {
                case > 1:
                    placeholderProportions = TextureProportions.Landscape;
                    break;
                case < 1:
                    placeholderProportions = TextureProportions.Portrait;
                    break;
                case 1:
                    placeholderProportions = TextureProportions.Square;
                    break;
                default:
                    Debug.Log("Invalid dimensions for " + SpriteRenderer + " verify scale and Sprite asset.", this);
                    break;
            }
        }

        public Sprite SetUpSprite(Texture2D tex)
        {
            Rect texturePart = new Rect(0.0f, 0.0f, tex.width, tex.height);

            Vector4 borders;
            if (addBorders)
                borders = new Vector4(equiBordersWidth, equiBordersWidth, equiBordersWidth, equiBordersWidth);
            else
                borders = Vector4.zero;

            Vector2 pivotPosition = new Vector2(0.5f, 0.5f);

            if (SpriteRenderer.sprite!=null)
            {
                newSpriteDimensions = new Vector2(
                tex.width / SpriteRenderer.sprite.pixelsPerUnit,
                tex.height / SpriteRenderer.sprite.pixelsPerUnit);
                newTextureRatio = newSpriteDimensions.x / newSpriteDimensions.y;

                switch (newTextureRatio)
                {
                    case (> 1):
                        newSpriteProportions = TextureProportions.Landscape;
                        break;
                    case (< 1):
                        newSpriteProportions = TextureProportions.Portrait;
                        break;
                    case (1):
                        newSpriteProportions = TextureProportions.Square;
                        break;
                    default:
                        Debug.Log("Invalid texture dimensions for " + tex + " verify source asset.", this);
                        break;
                }
            }
            

            return Sprite.Create(tex, texturePart, pivotPosition, minPixelPerUnit, 0, SpriteMeshType.FullRect, borders);
            // Borders could be used to add padding, but it might be better if it's done on the web API
            // Each customisable will have its own set of dimensions to adjust the sprite to
        }

        public void ImageSpriteSwap(Texture tex)
        {
            if (Image)
                Image.sprite = SetUpSprite((Texture2D)tex);
        }

        public void SwapSprite(Texture tex)
        {
            if (SpriteRenderer == null)
                return;
            if (tex==null)
            {
                Debug.Break();
            }
            GetSpriteDimensions();
            Sprite newSprite = SetUpSprite((Texture2D)tex);
            SpriteRenderer.sprite = newSprite;
            
            float spriteScaleFactor = 1;
            newRatio = 1;

            if (SpriteRenderer.drawMode == SpriteDrawMode.Simple)
            {
                switch (placeHolderWorldRatio - newTextureRatio)
                {
                    //New Sprite is narrower than Placeholder
                    case > 0:
                        spriteScaleFactor = placeHolderSpriteDimensions.y / newSpriteDimensions.y;
                        transform.localScale = new Vector3(
                            transform.localScale.y * spriteScaleFactor,
                            transform.localScale.y * spriteScaleFactor,
                            1);
                        break;

                    //New Sprite is wider than Placeholder
                    case < 0:
                        spriteScaleFactor = placeHolderSpriteDimensions.x / newSpriteDimensions.x;
                        transform.localScale = new Vector3(
                            transform.localScale.x * spriteScaleFactor,
                            transform.localScale.x * spriteScaleFactor,
                            1);
                        break;

                    //Both sprites have same ratio
                    case 0:
                        //No need to adujst, just scale
                        spriteScaleFactor = placeholderSpriteWorldDimensions.x / newSpriteDimensions.x;
                        transform.localScale = new Vector3(
                            transform.localScale.x * spriteScaleFactor,
                            transform.localScale.y * spriteScaleFactor,
                            1);
                        break;
                }

                newSpriteWorldDimensions = new Vector2(
                    newSpriteDimensions.x * transform.localScale.x,
                    newSpriteDimensions.y * transform.localScale.y);

                newRatio = newSpriteWorldDimensions.x / newSpriteWorldDimensions.y;

                SetPositionToAnchor();
            }
        }

        public void SetPositionToAnchor()
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    isOffset = false;
                    break;

                case HorizontalAlignment.Left:
                    offset = (placeholderSpriteWorldDimensions.x - newSpriteWorldDimensions.x) / 2;
                    PositionOffset = new Vector3(-offset, 0, 0);
                    transform.position += PositionOffset;
                    isOffset = true;
                    break;

                case HorizontalAlignment.Right:
                    offset = (placeholderSpriteWorldDimensions.x - newSpriteWorldDimensions.x) / 2;
                    PositionOffset = new Vector3(+offset, 0, 0);
                    transform.position += PositionOffset;
                    isOffset = true;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Up:

                    offset = (placeholderSpriteWorldDimensions.y - newSpriteWorldDimensions.y) / 2;
                    PositionOffset = new Vector3(0, offset, 0);
                    transform.position += PositionOffset;
                    isOffset = true;
                    break;

                case VerticalAlignment.Down:
                    offset = (placeholderSpriteWorldDimensions.y - newSpriteWorldDimensions.y) / 2;
                    PositionOffset = new Vector3(0, -offset, 0);
                    transform.position += PositionOffset;
                    isOffset = true;
                    break;
            }
        }

        public static Transform[] GetTopLevelChildren(Transform Parent)
        {
            Transform[] children = new Transform[Parent.childCount];
            for (int ID = 0; ID < Parent.childCount; ID++)
            {
                children[ID] = Parent.GetChild(ID);
            }
            return children;
        }

        public override void CheckSetup()
        {
            if (gameObject.scene == null)
                return;
            base.CheckSetup();
            correctSetup = SpriteRenderer!=null || Image!=null;

            if (!correctSetup)
                JLogBase.LogWarning("Customisable doesn't have neither a Sprite Renderer nor Image component, please add one or remove this customisable", this);
        }

        #endregion

        #region EditorOnly Logic
        [Button("Test")]
        public void SpriteRendererSpriteSwap(Texture tex)
        {
            SwapSprite(tex);
        }

        public void OnValidate()
        {
            CheckSetup();
            if (SpriteRenderer)
                GetSpriteDimensions();
        }

        public void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            if (displayBounds && SpriteRenderer)
            {
                switch (SpriteRenderer.drawMode)
                {
                    case SpriteDrawMode.Simple:
                        Gizmos.DrawWireCube(Vector3.zero, new Vector3(
                            placeholderSpriteWorldDimensions.x / transform.localScale.x,
                            placeholderSpriteWorldDimensions.y / transform.localScale.y,
                            0.1f));
                        break;
                    case SpriteDrawMode.Sliced:
                        Gizmos.DrawWireCube(Vector3.zero, new Vector3(SpriteRenderer.size.x, SpriteRenderer.size.y, 0.1f));
                        break;
                    case SpriteDrawMode.Tiled:
                        Gizmos.DrawWireCube(Vector3.zero, new Vector3(SpriteRenderer.size.x, SpriteRenderer.size.y, 0.1f));
                        break;
                }
            }
        }

        public void Reset() => isOffset = false;
        #endregion
    }
}
