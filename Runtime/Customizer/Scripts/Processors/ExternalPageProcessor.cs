using RLTY.Customisation;
using RLTY.SessionInfo;
using RLTY.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

namespace RLTY.Customisation
{
    [AddComponentMenu("RLTY/Customisable/Processors/External web page")]
    [RequireComponent(typeof(Customisable))]
    public class ExternalPageProcessor : Processor
    {
        [InfoBox("You need to add either a Button GameObject in a Canvas or a RLTY MouseEvent")]
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private RLTYMouseEvent mouseEvent;
        [SerializeField] private string url = "";

        public override Component FindComponent()
        {
            if (TryGetComponent(out Button _button))
            {
                button = _button;

                if (button.onClick.GetPersistentEventCount() == 0)
                    button.onClick.AddListener(OpenNewInternetPage);
                return button;
            }

            if (TryGetComponent(out RLTYMouseEvent _rltyMouseEvent))
            {
                mouseEvent = _rltyMouseEvent;

                if (mouseEvent.OnClick.GetPersistentEventCount() == 0)
                    mouseEvent.OnClick.AddListener(OpenNewInternetPage);
                return mouseEvent;
            }

            if (Debug.isDebugBuild)
                Debug.LogWarning("No Button or RLTYMouseEvent found" + gameObject.GetComponent<Customisable>().key + ", won't be clickable." + commonWarning, this);
            return null;
        }

        public override void Customize(KeyValueBase keyValue) => SetURL(keyValue.value);

        /// <summary>
        /// Set the url target to open when button clicked
        /// </summary>
        /// <param name="_url">URL target</param>
        public void SetURL(string _url) => url = _url;

        public void OpenNewInternetPage()
        {
            if (url != null && url != "")
                Application.OpenURL(url);

            if (Debug.isDebugBuild)
                Debug.Log("Trying to open external url: " + url, this);
        }
    }
}