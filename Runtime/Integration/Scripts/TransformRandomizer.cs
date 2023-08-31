using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Tools
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [AddComponentMenu("RLTY/Integration/Transform randomizer")]
    [HideMonoScript]
#endif
    public class TransformRandomizer : RLTYMonoBehaviourBase
    {
#if UNITY_EDITOR
        #region Global Variables   
        [Title("Scale")]
        [SerializeField]
        bool separateScaleAxis;
        [SerializeField, HideIf("separateScaleAxis"), MinMaxSlider(0.5f, 2f, true)]
        Vector2 scaleRange = new Vector2(0.5f, 2f);

        [SerializeField]
        [ShowIf("separateScaleAxis", true)]
        [MinMaxSlider(0.5f, 2f, true)]
        Vector2 xScaleRange = new Vector2(0.5f, 2f), 
            yScaleRange = new Vector2(0.5f, 2f), 
            zScaleRange = new Vector2(0.5f, 2f);

        [SerializeField, Space(5)]
        bool freezeScaleAxis;
        [SerializeField]
        [HorizontalGroup("Frozen scale axis", VisibleIf = "freezeScaleAxis"), LabelWidth(10), LabelText("x")]
        bool frozenXScale;
        [SerializeField]
        [HorizontalGroup("Frozen scale axis"), LabelText("y"), LabelWidth(10)]
        bool frozenYScale;
        [SerializeField]
        [HorizontalGroup("Frozen scale axis"), LabelText("z"), LabelWidth(10)]
        bool frozenZScale;

        [Title("Rotation")]
        [SerializeField]
        bool separateRotationAxis;
        [SerializeField]
        [HideIf("separateRotationAxis")]
        [MinMaxSlider(-180, 180, true)]
        Vector2 rotationRange = new Vector2(-180,180);

        [SerializeField]
        [ShowIf("separateRotationAxis")]
        [MinMaxSlider(-180, 180, true)]
        Vector2 xRotationRange = new Vector2(-180, 180),
            yRotationRange = new Vector2(-180, 180),
            zRotationRange = new Vector2(-180, 180);

        [SerializeField, Space(5)]
        bool freezeRotationAxis;
        [SerializeField]
        [HorizontalGroup("Frozen rotation axis", VisibleIf = "freezeRotationAxis"), LabelText("x"), LabelWidth(10)]
        bool frozenXRotation;
        [SerializeField]
        [HorizontalGroup("Frozen rotation axis"), LabelText("y"), LabelWidth(10)]
        bool frozenYRotation;
        [SerializeField]
        [HorizontalGroup("Frozen rotation axis"), LabelText("z"), LabelWidth(10)]
        bool frozenZRotation;
        #endregion

        [HorizontalGroup("Buttons", Title = "Randomize"), Button("Scale")]
        private void RandomizeScale()
        {
            Undo.RecordObject(transform, "Randomize Scale");

            float xScaling = transform.localScale.x;
            float yScaling = transform.localScale.y;
            float zScaling = transform.localScale.z;

            if (separateScaleAxis)
            {
                xScaling *= frozenXScale ? 1 : Random.Range(xScaleRange.x, xScaleRange.y);
                yScaling *= frozenYScale ? 1 : Random.Range(yScaleRange.x, yScaleRange.y);
                zScaling *= frozenZScale ? 1 : Random.Range(zScaleRange.x, zScaleRange.y);
            }

            else
            {
                float commonScaleFactor = Random.Range(scaleRange.x, scaleRange.y);

                xScaling *= frozenXScale ? 1 : commonScaleFactor;
                yScaling *= frozenYScale ? 1 : commonScaleFactor;
                zScaling *= frozenZScale ? 1 : commonScaleFactor;
            }

            transform.localScale = new Vector3(xScaling, yScaling, zScaling);
        }

        [HorizontalGroup("Buttons"), Button("Rotation")]
        private void RandomizeRotation()
        {
            Undo.RecordObject(transform, "Randomize Rotation");

            float xRotation = transform.eulerAngles.x;
            float yRotation = transform.eulerAngles.y;
            float zRotation = transform.eulerAngles.z;

            if (separateRotationAxis)
            {
                xRotation *= frozenXRotation ? 1 : Random.Range(xRotationRange.x, xRotationRange.y);
                yRotation *= frozenXRotation ? 1 : Random.Range(yRotationRange.x, yRotationRange.y);
                zRotation *= frozenXRotation ? 1 : Random.Range(zRotationRange.x, zRotationRange.y);
            }

            else
            {
                float commonrRotationFactor = Random.Range(rotationRange.x, rotationRange.y);

                xRotation *= frozenXScale ? 1 : commonrRotationFactor;
                yRotation *= frozenYScale ? 1 : commonrRotationFactor;
                zRotation *= frozenZScale ? 1 : commonrRotationFactor;
            }

            transform.Rotate(new Vector3(xRotation, yRotation, zRotation), Space.Self);
        }

        //TBC
        //[HorizontalGroup("Buttons"), Button("Position")]
        //private void RandomPositionOffset()
        //{
        //
        //}

        [HorizontalGroup("Buttons"), Button("Both")]
        private void RandomizeBoth()
        {
            Undo.RecordObject(transform, "Randomized position and rotation");

            RandomizeScale();
            RandomizeRotation();
        }
#endif
    }

}
