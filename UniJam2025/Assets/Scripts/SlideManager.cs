using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets
{
    public class SlideManager : MonoBehaviour
    {
        [SerializeField] private GameObject testSlide;
        [SerializeField] private TMP_Text slideText; 
        [SerializeField] private TMP_Text slideTitle;
        [SerializeField] private Image image;

        [SerializeField] private GameObject ruleSlide;

        private Texture2D[] allImages;

        private void Start()
        {
            allImages = Resources.LoadAll<Texture2D>("SlideImages");

            GenerateSlide();
        }

        public void GenerateSlide()
        {
            //pull this shit from a json
            string genText = "aaa text";
            string genTitle = "Default title";
            Texture2D loadedTexture = allImages[UnityEngine.Random.Range(0, allImages.Length - 1)];

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
            //not implemented yet
        }

        public bool IsRuleActive(Rule rule)
        {
            return false;
        }
    }
}
