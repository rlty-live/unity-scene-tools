using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Customisation
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif

    [RequireComponent(typeof(DecalProjector)), HideMonoScript]
    [AddComponentMenu("RLTY/Customisable/ScaleDecal")]
    public class ScaleDecalToTexture : RLTYMonoBehaviour
    {
        DecalProjector decal;
        [SerializeField, ReadOnly, ShowIf("showUtilities", true)]
        Material mat;
        [SerializeField, ReadOnly, ShowIf("showUtilities", true)]
        Texture decalTexture;

        [SerializeField, ReadOnly, ShowIf("showUtilities", true)]
        private Vector2 decalMaxSize;
        [SerializeField, ReadOnly, ShowIf("showUtilities", true)]
        private Vector2 textureDimensions;
        [SerializeField, ReadOnly, ShowIf("showUtilities", true)]
        private float decalTextureRatio;

        [SerializeField, ReadOnly]
        public TextureProportions proportions;


        [Button, HorizontalGroup("Buttons")]
        public void ResizeDecal()
        {
            GetDecalTextureDimensions();
            AdaptDecalRatioToTexture();
        }

        private void GetDecalTextureDimensions()
        {
            CheckSetup();

            decalTexture = mat.GetTexture("Base_Map");
            textureDimensions = new Vector2(decalTexture.width, decalTexture.height);
            decalMaxSize = new Vector2(decal.size.x, decal.size.y);

            textureDimensions = new Vector2(decalTexture.width, decalTexture.height);
            decalMaxSize = new Vector2(decal.size.x, decal.size.y);

            if (textureDimensions.x > textureDimensions.y)
                proportions = TextureProportions.Landscape;

            if (textureDimensions.x < textureDimensions.y)
                proportions = TextureProportions.Portrait;

            if (textureDimensions.x == textureDimensions.y)
                proportions = TextureProportions.Square;

            decalTextureRatio = textureDimensions.x / textureDimensions.y;
        }

        private void AdaptDecalRatioToTexture()
        {
            switch (proportions)
            {
                case (TextureProportions.Landscape):
                    decal.size = new Vector3(decal.size.x, decal.size.y / decalTextureRatio, decal.size.z);
                    break;
                case TextureProportions.Portrait:
                    decal.size = new Vector3(decal.size.x * decalTextureRatio, decal.size.y, decal.size.z);
                    break;
                case TextureProportions.Square:
                    //Do Nothing
                    break;
                default:
                    break;
            }
        }

        public override void CheckSetup()
        {
            base.CheckSetup();

            decal = GetComponent<DecalProjector>();

            if (decal.material)
                mat = decal.material;

            else
            {
                if (debug)
                    Debug.Log(this.decal + "is missing a material", this);
            }
        }

#if UNITY_EDITOR
        [Button("Reset"), HorizontalGroup("Buttons")]
        private void Reset()
        {
            decal.size = new Vector3(decalMaxSize.x, decalMaxSize.y, decal.size.z);
        }

        public void OnValidate()
        {
            CheckSetup();
        }
#endif

        public override void EventHandlerRegister() { }
        public override void EventHandlerUnRegister() { }
    }

}
