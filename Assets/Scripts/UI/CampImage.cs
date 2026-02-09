using System;
using System.Collections.Generic;
using TableStructure;
using UnityEngine;
using UnityEngine.UI;

namespace Takeover
{
    public class CampImage : MonoBehaviour
    {
        [SerializeField]
        private List<CampImageData> images;
        [SerializeField]
        private bool resetNativeSize = false;

        private ECamp m_camp;
        public ECamp Camp
        {
            get => m_camp;
            set
            {
                m_camp = value;
                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            if (images != null && images.Count > 0)
            {
                var image = GetComponent<Image>();
                foreach (var data in images)
                {
                    if (data.camp == m_camp)
                    {
                        image.sprite = data.sprite;
                        if (resetNativeSize)
                            image.SetNativeSize();
                        break;
                    }
                }
            }

            // 同时更新子类
            foreach (var child in transform.GetComponentsInChildren<CampImage>())
                if (child != this)
                    child.Camp = m_camp;
        }


        [Serializable]
        private class CampImageData
        {
            public ECamp camp;
            public Sprite sprite;
        }
    }
}