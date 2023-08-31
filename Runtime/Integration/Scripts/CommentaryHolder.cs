using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Tools
{
    [AddComponentMenu("RLTY/Integration/Commentary holder")]
    public class CommentaryHolder : MonoBehaviour
    {
        [Title("New comment")]
        [SerializeField, TextArea]
        string comment;
        [SerializeField]
        string url;
        [SerializeField]
        string author;

        [Title("Comments")]
        [SerializeField, ReadOnly, PropertyOrder(4)]
        public List<Commentary> commentaries;

        [Button(ButtonSizes.Medium)]
        public void PublishCommentary()
        {
            commentaries.Add(new Commentary(comment, author));
            comment = string.Empty;
            url = string.Empty;

            if (!CompareTag("EditorOnly"))
                gameObject.tag = "EditorOnly";
        }

        [SerializeField, InfoBox("Wrong saving path, saving to default: Assets/Scenes/Commentaries/", "wrongSavingPath")]
        private string customSavingFolder = "Assets/Scenes/Commentaries/";
        private static string defaultSavingPath = "Assets/Scenes";
        private bool wrongSavingPath;

        //Add helpboxes and debugs for overwrites
        //maybe a folder selector
        //Refactor the saving function for clarity and security
#if UNITY_EDITOR
        [MenuItem("CONTEXT/CommentaryHolder/Save commentaries")]
        static void SaveCommentariesAsAssets(MenuCommand command)
        {
            CommentaryHolder cH = (CommentaryHolder)command.context;
            CommentaryHolderSO newCommentaryHolderSO = ScriptableObject.CreateInstance<CommentaryHolderSO>();
            newCommentaryHolderSO._commentaries = cH.commentaries.ToArray();
            newCommentaryHolderSO.sOName = "S_" + cH.gameObject.scene.name + ", GO_" + cH.gameObject.name;

            newCommentaryHolderSO.name = newCommentaryHolderSO.sOName + ".asset";

            if (AssetDatabase.IsValidFolder(cH.customSavingFolder))
            {
                if (AssetDatabase.LoadAssetAtPath(cH.customSavingFolder + newCommentaryHolderSO.name, typeof(CommentaryHolderSO)))
                    AssetDatabase.DeleteAsset(cH.customSavingFolder + newCommentaryHolderSO.name);

                AssetDatabase.CreateAsset(newCommentaryHolderSO, cH.customSavingFolder + newCommentaryHolderSO.name);
            }

            else
            {
                AssetDatabase.CreateFolder(defaultSavingPath, "/Commentaries");

                if (AssetDatabase.LoadAssetAtPath(cH.customSavingFolder + newCommentaryHolderSO.name, typeof(CommentaryHolderSO)))
                    AssetDatabase.DeleteAsset(cH.customSavingFolder + newCommentaryHolderSO.name);

                AssetDatabase.CreateAsset(newCommentaryHolderSO, defaultSavingPath + "/Commentaries/" + newCommentaryHolderSO.name);
            }
        }


        [Button("Create Asset")]
        private void SaveCommentariesAsAssets()
        {
            CommentaryHolderSO newCommentaryHolderSO = ScriptableObject.CreateInstance<CommentaryHolderSO>();
            newCommentaryHolderSO._commentaries = commentaries.ToArray();
            newCommentaryHolderSO.sOName = "S_" + gameObject.scene.name + ", GO_" + gameObject.name;

            newCommentaryHolderSO.name = newCommentaryHolderSO.sOName + ".asset";

            if (AssetDatabase.IsValidFolder(customSavingFolder))
            {
                if (AssetDatabase.LoadAssetAtPath(customSavingFolder + newCommentaryHolderSO.name, typeof(CommentaryHolderSO)))
                    AssetDatabase.DeleteAsset(customSavingFolder + newCommentaryHolderSO.name);

                AssetDatabase.CreateAsset(newCommentaryHolderSO, customSavingFolder + newCommentaryHolderSO.name);
            }

            else
            {
                AssetDatabase.CreateFolder(defaultSavingPath, "Commentaries");

                if (AssetDatabase.LoadAssetAtPath(customSavingFolder + newCommentaryHolderSO.name, typeof(CommentaryHolderSO)))
                    AssetDatabase.DeleteAsset(customSavingFolder + newCommentaryHolderSO.name);

                AssetDatabase.CreateAsset(newCommentaryHolderSO, defaultSavingPath + "/Commentaries/" + newCommentaryHolderSO.name);
            }
        }
#endif
    }

    [System.Serializable]
    public class Commentary
    {
        [SerializeField]
        string dateTime;
        [SerializeField]
        string author;
        [SerializeField, ShowIf("hasURL", true)]
        string url;
        [SerializeField, TextArea, HideLabel]
        string commentary;

        private bool hasURL;

        public Commentary(string _comment, string _author)
        {
            author = _author;
            commentary = _comment;
            dateTime = System.DateTime.Now.ToShortDateString() + ": " + System.DateTime.Now.Hour + " hours";
        }

        public Commentary(string _comment, string _url, string _author)
        {
            author = _author;

            if (url != null && url != string.Empty)
                url = _url;

            commentary = _comment;
            dateTime = System.DateTime.Now.ToShortDateString() + ": " + System.DateTime.Now.Hour + " hours";
        }
    }

    public class CommentaryHolderSO : ScriptableObject
    {
        public string sOName;
        public Commentary[] _commentaries;
    }
}
