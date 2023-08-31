using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

namespace RLTY.Customisation
{
    [AddComponentMenu("RLTY/Customisable/Processors/Material")]
    public class MaterialProcessor : Processor
    {
        #region Global Variables
        //This component should update everytime the customisable changes customisable type,
        //but OnValidate seems to miss its target, or is not suited to update other components
        [InfoBox("Reset this component if you change the Customisable's CustomisableType")]
        public List<MaterialSpecs> materialsSpecs;
        public bool modifyAllInstances;
        [SerializeField, ReadOnly, ShowIf("showUtilities")]
        private List<Material> materialInstances;

        public static List<ModifiableProperty> modifiedProperties = new List<ModifiableProperty>();
        [ShowIf("showUtilities"), SerializeField]
        private List<ModifiableProperty> propertiesToModify = new List<ModifiableProperty>();

        #endregion

        #region EditorOnly Logic
#if UNITY_EDITOR

        [Button("Get Materials")]
        public void GetMaterialsProperties()
        {
            materialsSpecs = new List<MaterialSpecs>();
            Renderer rdr = null;
            Material sharedMaterial;
            MaterialSpecs specs;

            if (TryGetComponent(out Renderer _rdr))
            {
                rdr = _rdr;

                if (rdr.sharedMaterials.Length > 0)
                    foreach (Material _sharedMat in rdr.sharedMaterials)
                    {
                        sharedMaterial = _sharedMat;
                        specs = new MaterialSpecs(sharedMaterial, GetComponent<Customisable>().type);
                        materialsSpecs.Add(new MaterialSpecs(sharedMaterial, GetComponent<Customisable>().type));
                    }
                else
                {
                    if (debug)
                        Debug.Log("This Renderer does not have a material, please assign one or remove this component and the customisable", this);
                }

            }

            if (TryGetComponent(out DecalProjector projector))
            {
                if (projector.material)
                    materialsSpecs.Add(new MaterialSpecs(projector.material, GetComponent<Customisable>().type));
            }
            else
            {
                if (!rdr)
                    if (debug) Debug.Log("Trying to modify on material on a GameObject with no renderer nor Decal Projector, please assign one or remove this customisable");
            }
        }

        public void Reset()
        {
            GetMaterialsProperties();
            GetPropertiesToModify();
        }
#endif
        #endregion

        #region Common Logic

        public override Component FindComponent()
        {
            TryGetComponent(out Renderer rd);
            Component target = rd;

            if (target == null)
            {
                if (!TryGetComponent(out DecalProjector proj))
                {
                    if (debug)
                       JLogBase.LogWarning("No Renderer or DecalProjector found in children" + commonWarning, this);
                }
                else
                    target = proj;
            }
            return target;
        }

        public override void Customize(KeyValueBase keyValue)
        {
            GetPropertiesToModify();

            if (propertiesToModify == null || propertiesToModify.Count == 0)
            {
                JLogBase.LogWarning("No properties to modify", this);
                return;
            }

            switch (keyValue.Type)
            {
                case CustomisableType.Texture:
                    Texture t = keyValue.data as Texture;
                    if (t != null)
                        SwapTextures(t);
                    else
                        JLogBase.LogError("Couldn't process texture: " + keyValue, this);
                    break;
                case CustomisableType.Color:
                    if (CustomisableUtility.TryParseColor(keyValue, out Color color))
                        SwapColors(color);
                    else
                        JLogBase.LogError("Couldn't parse color: " + keyValue, this);
                    break;
                default:
                    JLogBase.LogError("Unhandled type " + keyValue.Type, this);
                    break;
            }
        }

        public void GetPropertiesToModify()
        {
            if (materialsSpecs.Any())
                foreach (MaterialSpecs matSpecs in materialsSpecs)
                {
                    if (matSpecs.shaderProperties.Any() && matSpecs.customize)
                        foreach (ModifiableProperty property in matSpecs.shaderProperties)
                        {
                            if (!propertiesToModify.Contains(property) && property.modifyThis)
                                propertiesToModify.Add(property);
                        }
                }
        }
        #endregion

        #region Runtime Logic
        public void SwapTextures(Texture tex)
        {
            if (modifyAllInstances)
            {
                foreach (ModifiableProperty property in propertiesToModify)
                {
                    if (property.mat && !modifiedProperties.Contains(property))
                    {
                        property.mat.SetTexture(property.propertyName, tex);
                        modifiedProperties.Add(property);
                        //JLogBase.Log("Switched " + property.mat + "shared material property " + property.propertyName + " texture to " + tex, this);
                    }
                }
            }

            //Modify to have only on instance of this material be applied to all meshrenderer
            //Instead of an instance for each meshrenderer
            else
            {
                GetComponent<Renderer>().GetMaterials(materialInstances);

                foreach (ModifiableProperty property in propertiesToModify)
                {
                    if (property.mat)
                        foreach (Material mat in materialInstances)
                        {
                            if (mat.name.Contains(property.mat.name))
                            {
                                mat.SetTexture(property.propertyName, tex);
                                //JLogBase.Log("Switched " + property.mat + "instanced material property " + property.propertyName + " texture to " + tex, this);
                            }
                        }
                    else
                        JLogBase.LogWarning("Missing material in for " + property, this);
                }
            }

            if (TryGetComponent(out ScaleDecalToTexture scaler))
                scaler.ResizeDecal();
        }

        public void SwapColors(Color color)
        {
            if (modifyAllInstances)
            {
                foreach (ModifiableProperty property in propertiesToModify)
                {
                    if (!modifiedProperties.Contains(property) && property.mat)
                    {
                        property.mat.SetColor(property.propertyName, color);
                        modifiedProperties.Add(property);
                        //JLogBase.Log("Switched " + property.mat + "shared material property " + property.propertyName + " color to " + color, this);
                    }
                    else
                    {
                        JLogBase.Log("Either property has already been modified or material is missing", this);
                    }
                }
            }
            else
            {
                GetComponent<Renderer>().GetMaterials(materialInstances);
                
                foreach (ModifiableProperty property in propertiesToModify)
                {
                    if(property.mat)
                        foreach (Material mat in materialInstances)
                        {
                            if (mat.name.Contains(property.mat.name))
                            {
                                mat.SetColor(property.propertyName, color);
                                //JLogBase.Log("Switched " + property.mat + "instanced material property " + property.propertyName + " color to " + color, this);
                            }
                            else
                                JLogBase.Log(property.propertyName + " is not featured in " + mat.name, this);
                        }
                    else
                        JLogBase.Log("Either property has already been modified or material is missing", this);
                }
            }
        }

        #endregion
    }

    #region Data
    [System.Serializable]
    public class MaterialSpecs
    {
        private CustomisableType customisableType;
        [ReadOnly]
        public Material sharedMaterial;
        [SerializeField]
        public bool customize;

        [SerializeField, ShowIf("customize", true)]
        public List<ModifiableProperty> shaderProperties;

        public MaterialSpecs(Material _sharedMat, CustomisableType _customisableType)
        {
            customisableType = _customisableType;
            sharedMaterial = _sharedMat;

            switch (_customisableType)
            {
                case CustomisableType.Texture:
                    GetAllTextureProperties();
                    break;
                case CustomisableType.Sprite:
                    GetAllTextureProperties();
                    break;
                case CustomisableType.Color:
                    GetAllColorProperties();
                    break;
                default:
                    break;
            }
        }

        public void GetAllTextureProperties()
        {
            float nProperties = sharedMaterial.shader.GetPropertyCount();
            shaderProperties = new List<ModifiableProperty>();

            for (int i = 0; i < nProperties; i++)
            {
                if (sharedMaterial.shader.GetPropertyType(i) == ShaderPropertyType.Texture)
                    shaderProperties.Add(new ModifiableProperty(sharedMaterial, i));
            }
        }

        public void GetAllColorProperties()
        {
            float nProperties = sharedMaterial.shader.GetPropertyCount();
            shaderProperties = new List<ModifiableProperty>();

            for (int i = 0; i < nProperties; i++)
            {
                if (sharedMaterial.shader.GetPropertyType(i) == ShaderPropertyType.Color)
                    shaderProperties.Add(new ModifiableProperty(sharedMaterial, i));
            }
        }
    }

    [System.Serializable]
    public class ModifiableProperty
    {
        [ReadOnly]
        public Material mat;
        [ReadOnly]
        public string propertyName;
        public bool modifyThis;

        public ModifiableProperty(Material _mat, int propertyIndex)
        {
            mat = _mat;
            propertyName = mat.shader.GetPropertyName(propertyIndex);
        }
    }
    #endregion
}