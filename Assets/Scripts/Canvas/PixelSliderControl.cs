using System;
using UnityEngine;

namespace Canvas
{
    public abstract class PixelSliderControl : MonoBehaviour
    {
        public GameObject[] sections;
        public int maxValue;
        public int minValue;
        public int countNow;
    
        private int _count;
        private int _addValue;
    
        protected int value;
        
        private void Awake()
        {
            _count = sections.Length;
            _addValue = (Math.Abs(maxValue) + Math.Abs(minValue)) / _count;
            StartCount();
        }

        protected virtual void StartCount()
        {
            SetCount(countNow);
        }

        public void AddCount()
        {
            if (countNow == _count-1)
            {
                countNow = _count-2;
            }
            countNow++;
            sections[countNow].SetActive(true);
            AddEvent();
        }
    
        public void DeleteCount()
        {
            if (countNow < 0)
            {
                return;
            }
            sections[countNow].SetActive(false);
            countNow--;
            DeleteEvent();
        }

        protected void SetCount(int count)
        {
            countNow = count;
            for (var i = 0; i < _count; i++)
            {
                sections[i].SetActive(countNow >= i);
            }
            SetValue(minValue + count * _addValue);
        }

        protected virtual void AddEvent()
        {
            value += _addValue;
        }

        protected virtual void DeleteEvent()
        {
            value -= _addValue;
        }

        protected virtual void SetValue(int value)
        {
            this.value = value;
        }
    }
}
