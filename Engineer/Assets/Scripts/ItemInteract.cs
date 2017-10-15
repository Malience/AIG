using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    public class ItemInteract : MonoBehaviour
    {
        public event Action OnOver; //gaze on object
        public event Action OnOut; //gave leaving object
        public event Action OnClick;
        public event Action OnDoubleClick;
        public event Action OnUp; //fire1 release
        public event Action OnDown; //fire1 press

        protected bool m_IsOver;

        public bool IsOver
        {
            get
            {
                return m_IsOver;
            }
        }

        public void Over()
        {
            m_IsOver = true;
            if (OnOver != null)
                OnOver();
        }

        public void Out()
        {
            m_IsOver = false;
            if (OnOut != null)
                OnOut();
        }

        public void Click()
        {
            if (OnClick != null)
                OnClick();
        }

        public void DoubleClick()
        {
            if (OnDoubleClick != null)
                OnDoubleClick();
        }

        public void Up()
        {
            if (OnUp != null)
                OnUp();
        }

        public void Down()
        {
            if (OnDown != null)
                OnDown();
        }
    } //end class
} //end namespace
