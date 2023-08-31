using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace RLTY.Customisation
{
    [AddComponentMenu("RLTY/Customisable/Processors/Text")]
    public class TextProcessor : Processor
    {
        #region F/P
        #region Serialize

        [SerializeField, Header("Swap Text"), TextArea,
         Tooltip("The text that you want to display. If you want a return you will need to write \\n")]
        string text = "";

        #region Text Style

        [SerializeField, Header("Text Style")] bool textStyle = false;

        [SerializeField, ShowIf("textStyle")] Vector4 textMargin = Vector4.zero;

        [SerializeField, ShowIf("textStyle")] private float fontSize = 36;

        #endregion

        #region Scroll Rect

        [SerializeField, Header("Scroll View")]
        bool doScrollRect = false;

        [SerializeField, ShowIf("doScrollRect")]
        GameObject scrollviewRef = null;

        [SerializeField, ShowIf("doScrollRect"),
         Tooltip("Base at 2. If the text is really long, you can up it or reduce if really short")]
        private float securityTextSize = 2;

        #endregion

        [SerializeField, Header("Page")] 
        private bool doPage = false;

        #region Tabulation

        [SerializeField, Header("Tabulation")] 
        bool doTabulation = false;

        [SerializeField, ShowIf("doTabulation")]
        private bool onlyTabulateOnFirstLine = false;

        #endregion

        #endregion

        #region Private

        // ReSharper disable once InconsistentNaming
        GameObject scrollView = null;
        private GameObject content = null;

        // ReSharper disable once InconsistentNaming
        private TMP_Text textField = null;
        private int pageCount = 0;
        private bool isScrollViewValid => scrollView && doScrollRect;

        #endregion

        public override Component FindComponent()
        {
            TextMeshPro textMeshPro = GetComponentInChildren<TextMeshPro>();
            TextMeshProUGUI textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
            Component target = null;
            if (!textMeshPro && !textMeshProUGUI)
            {
                if (debug)
                    Debug.Log("No TextMeshPro found in children" + commonWarning, this);
            }

            else
            {
                if (textMeshPro)
                    target = textMeshPro;

                if (textMeshProUGUI)
                    target = textMeshProUGUI;
            }
            return target;
        }

        public override void Customize(KeyValueBase keyValue)
        {
            SwapText(keyValue.value);
        }


        private bool IsValid
        {
            get
            {
                if (!textField)
                {
                    if (!TryGetComponent(out TMP_Text tmpText))
                        return false;
                    textField = tmpText;
                }

                return true;
            }
        }

        public bool IsScrollRectValid
        {
            get
            {
                if (GetComponent<ScrollRect>())
                    if (GetComponentInChildren<Scrollbar>())
                        return true;
                return false;
            }
        }
        #endregion


        #region Text Style

        /// <summary>
        /// Set Margin of the TextField
        /// </summary>
        [Button("Set Text Margin"), ShowIf("textStyle"), Title("Text Style"),
         Tooltip("Set the margin of the Text Field")]
        private void SetTextFielMargin()
        {
            if (!IsValid) return;

            textField.margin = textMargin;
        }

        /// <summary>
        /// Get the Margin of the TextField
        /// </summary>
        [Button("Get Text Margin"), ShowIf("textStyle"), Tooltip("Get the margin of the Text Field")]
        private void GetTextFieldMargin()
        {
            if (!IsValid) return;

            textMargin = textField.margin;
        }

        [Button("Set Font Size"), ShowIf("textStyle")]
        private void SetFontSize()
        {
            if (!IsValid) return;

            textField.fontSize = fontSize;
        }

        [Button("Get Font Size"), ShowIf("textStyle")]
        private void GetCurrentFontSize()
        {
            if (!IsValid) return;

            fontSize = textField.fontSize;
        }

        #endregion

        #region Scroll View

        [Button("Create Scroll Rect"), ShowIf("doScrollRect", true), Title("Scroll View"),
         Tooltip("Create a scroll view to scroll the text if it's too long")]
        private void ScrollRect()
        {
            if (!scrollviewRef || scrollView) return;

            scrollView = Instantiate(scrollviewRef, transform.parent, true);
            RectTransform scrollViewRectTransform = scrollView.GetComponent<RectTransform>();
            RectTransform rectTransform = GetComponent<RectTransform>();

            scrollViewRectTransform.offsetMin =
                new Vector2(rectTransform.offsetMin.x, scrollViewRectTransform.offsetMin.y);
            scrollViewRectTransform.offsetMax =
                new Vector2(rectTransform.offsetMax.x, scrollViewRectTransform.offsetMax.y);


            // int textWidth = (int)(Mathf.Abs(rectTransform.offsetMin.x) + Mathf.Abs(rectTransform.offsetMax.x)); 

            Transform viewportTransform = scrollView.transform.Find("Viewport");
            if (!viewportTransform) return;

            RectTransform viewportRect = viewportTransform.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.pivot = new Vector2(0, 1);

            Transform contentTransform = viewportTransform.Find("Content");

            if (!contentTransform) return;
            content = contentTransform.gameObject;

            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0, 1);

            Vector3 position = transform.position;

            transform.SetParent(contentTransform);

            // 

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            scrollView.transform.position = position;

            //TODO Verify with calcul if the text will overflow by dividing content.bottom with text size
            //Add a security to the result because line not full

            ResizeScrollView();
        }

        [Button("Resize Scroll View"), ShowIf("isScrollViewValid", true),
         Tooltip("Will re calculate the size of the content game object to fit to the text")]
        private void ResizeScrollView()
        {
            if (!content || !scrollView) return;

            RectTransform rectTransform = GetComponent<RectTransform>();
            RectTransform scrollRectTransform = scrollView.GetComponent<RectTransform>();
            RectTransform contentRectTransform = content.GetComponent<RectTransform>();

            Vector2 scrollRectOffsetMin = scrollRectTransform.offsetMin;
            Vector2 scrollRectOffsetMax = scrollRectTransform.offsetMax;

            Vector2 contentOffsetMin = contentRectTransform.offsetMin;

            int width = (int) (Mathf.Abs(scrollRectOffsetMin.x) + Mathf.Abs(scrollRectOffsetMax.x));

            int theoricalNumberOfLine = (int) (Mathf.Abs(contentOffsetMin.y) / textField.fontSize);
            int theoricalNumberOfCharacter = (int) (Mathf.Abs(width) / (textField.fontSize / 2));

            int numberOfCharacterNeed = (int) (textField.text.Length + securityTextSize != 0
                ? (textField.text.Length / securityTextSize)
                : 0);

            int line = numberOfCharacterNeed / theoricalNumberOfCharacter;
            int height = (int) (line * textField.fontSize);

            contentRectTransform.offsetMin = new Vector2(0, -height);
            contentRectTransform.offsetMax = Vector2.zero;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private bool IsTextOverflowing()
        {
            // if (!scrollView) return false;
            RectTransform scrollRectTransform = GetComponent<RectTransform>();
            Vector2 scrollRectOffsetMin = scrollRectTransform.offsetMin;
            Vector2 scrollRectOffsetMax = scrollRectTransform.offsetMax;


            int width = (int) (Mathf.Abs(scrollRectOffsetMin.x) + Mathf.Abs(scrollRectOffsetMax.x));

            int theoricalNumberOfCharacter = (int) (Mathf.Abs(width) / (textField.fontSize / 2));

            int numberOfCharacterNeed = (int) (textField.text.Length + securityTextSize != 0
                ? (textField.text.Length / securityTextSize)
                : 0);
            if (numberOfCharacterNeed > theoricalNumberOfCharacter) return false;
            return true;
        }

        #endregion

        #region Tabulation

        /// <summary>
        /// Add a tabulation at the start of the text
        /// </summary>
        [Button("Do Tabulation"), ShowIf("doTabulation", true), Title("Tabulation"),
         Tooltip("Will place a tabulation after each double return and at the first paragraph")]
        private void Tabulation()
        {
            if (!IsValid) return;
            if (!doTabulation) return;

            textField.text = "\t" + textField.text;
        }

        /// <summary>
        /// Add Tabulation after double return
        /// </summary>
        private void MakeTabulationAfterReturn()
        {
            string tmp = textField.text;
            bool asReturn = false;
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] == '\n')
                {
                    if (asReturn)
                    {
                        i++;
                        tmp = tmp.Insert(i, "\t");
                        asReturn = false;
                        continue;
                    }

                    asReturn = true;
                }
            }

            textField.text = tmp;
        }

        #endregion

        #region Public

        #region Swap Text

        /// <summary>
        /// Swap the text of the TextField. If doTabulation then call Tabulation(). If doScrollRect then call ScrollRect().
        /// </summary>
        [Button("Swap Text"), Title("Swap Text"), Tooltip("Swap the current display text with the text you have enter")]
        public void SwapText()
        {
            if (!IsValid) return;
            string replace = text.Replace("\\n", "\n");
            textField.text = replace;
            if (doScrollRect && IsTextOverflowing()) ScrollRect();
            if (doPage)
            {
                textField.overflowMode = TextOverflowModes.Page;
                Canvas canvas = null;
                GameObject baseObejct = gameObject;
                while (!canvas && baseObejct.transform.parent)
                {
                    canvas = baseObejct.transform.parent.GetComponent<Canvas>();
                    if (!canvas) baseObejct = baseObejct.transform.parent.gameObject;
                }

                if (!canvas)
                {
                    Debug.Log("Not Working while de merde");
                    return;
                }
                canvas.worldCamera = Camera.main;
            }
            if (doTabulation)
            {
                Tabulation();
                if (!onlyTabulateOnFirstLine) MakeTabulationAfterReturn();
            }
        }

        /// <summary>
        /// Replace text with param _text and call SwapText()
        /// </summary>
        /// <param name="_text"></param>
        public void SwapText(string _text)
        {
            // if (!IsValid) return;
            // textField.text = _text;
            // if (doScrollRect && IsTextOverflowing()) ScrollRect();
            // if (doTabulation) Tabulation();
            text = _text;
            SwapText();
        }

        #endregion

        public void NextPage()
        {
            if(!IsValid)return;

            textField.pageToDisplay = pageCount++;
        }

        #endregion
    }
}