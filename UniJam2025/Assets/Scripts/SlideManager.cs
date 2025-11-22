using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets
{
    public class SlideManager : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float redTextProbabilty = 0.25f;
        [SerializeField, Range(0f, 1f)] private float blueTextProbabilty = 0.25f;

        [SerializeField, Range(0f, 1f)] private float boldTextProbabilty = 0.5f;
        [SerializeField, Range(0f, 1f)] private float italicTextProbabilty = 0.25f;
        [SerializeField, Range(0f, 1f)] private float underlinedTextProbabilty = 0.25f;
        [SerializeField, Range(0f, 1f)] private float crossedTextProbabilty = 0.05f;

        [SerializeField] private Textdata textData;
        [SerializeField] private Textdata titleData;

        [SerializeField] private GameObject testSlide;
        [SerializeField] private TMP_Text slideText; 
        [SerializeField] private TMP_Text slideTitle;
        [SerializeField] private Image image;

        [SerializeField] private GameObject ruleSlide;
        [SerializeField] private TMP_Text ruleSlideText;

        private Texture2D[] allImages;

        private int shownTextureIndex = -1;

        private void Awake()
        {
            allImages = Resources.LoadAll<Texture2D>("SlideImages");
        }

        public void GenerateSlide()
        {
            testSlide.SetActive(true);
            ruleSlide.SetActive(false);

            //pull this shit from a json
            string genText = StyleString(textData.text[UnityEngine.Random.Range(0, textData.text.Length)]);
            string genTitle = StyleString("<im>" + titleData.text[UnityEngine.Random.Range(0, titleData.text.Length)] + "</im>");

            shownTextureIndex = UnityEngine.Random.Range(0, allImages.Length);
            //Texture2D loadedTexture = allImages[shownTextureIndex];
            Texture2D loadedTexture = allImages[shownTextureIndex];

            slideText.text = genText;
            slideTitle.text = genTitle;
            image.sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Rule description should be of the format "Si X alors Y"
        /// </summary>
        /// <param name="ruleDescription"></param>
        public void GenerateRuleSlide(string ruleDescription)
        {
            testSlide.SetActive(false);
            ruleSlide.SetActive(true);

            ruleSlideText.text = ruleDescription;
        }

        public bool IsRuleActive(Rule rule)
        {
            switch (rule.conditionType)
            {
                case Rule.ConditionType.TextRed:
                    return slideTitle.text.Contains("<color=ff0000>") || slideText.text.Contains("<color=ff0000>");
                case Rule.ConditionType.TitleUnderlined:
                    return slideTitle.text.Contains("<u>");
                case Rule.ConditionType.ImgDiagram:
                    return shownTextureIndex == 0;
                default:
                    return false;
            }
        }

        private string StyleString(string s)
        {
            int openIndex = s.IndexOf("<im>");
            if (openIndex == -1)
                return s;

            int endIndex = s.IndexOf("</im>");

            string start = "";
            string end = "";

            if (UnityEngine.Random.Range(0f, 1f) <= boldTextProbabilty)
            {
                start += "<b>";
                end += "</b>";
            }
            if (UnityEngine.Random.Range(0f, 1f) <= italicTextProbabilty)
            {
                start += "<i>";
                end += "</i>";
            }
            if (UnityEngine.Random.Range(0f, 1f) <= crossedTextProbabilty)
            {
                start += "<s>";
                end += "</s>";
            }
            if (UnityEngine.Random.Range(0f, 1f) <= underlinedTextProbabilty)
            {
                start += "<u>";
                end += "</u>";
            }

            {
                //color selection
                float colorProb = UnityEngine.Random.Range(0f, 1f);
                if (colorProb <= redTextProbabilty)
                {
                    start += "<color=#ff0000>";
                    end += "</color>";
                }
                else if (colorProb - redTextProbabilty <= blueTextProbabilty)
                {
                    start += "<color=#0000ff>";
                    end += "</color>";
                }
            }

            s = s.Substring(0, openIndex) + start + s.Substring(openIndex + 4, endIndex - openIndex - 4) + end + s.Substring(endIndex + 5);

            return StyleString(s);
        }
    }
}
