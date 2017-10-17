//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Utils
{
    public class Levers : MonoBehaviour
    {
        /*
        //Code for non-VR
        bool on = false;
        public float timer;

        private void OnTriggerStay(Collider col)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                on = true;
                Debug.Log("Lever on");
                StartCoroutine("leverOff");
            }
        }

        IEnumerator leverOff()
        {
            yield return new WaitForSeconds(timer);
            on = false;
            Debug.Log("Lever off");
        }
        */

        [SerializeField] private Material m_NormalMaterial;
        [SerializeField] private Material m_OverMaterial;
        [SerializeField] private Material m_ClickedMaterial;
        [SerializeField] private Material m_DoubleClickMaterial;
        [SerializeField] private ItemInteract item;
        [SerializeField] private Renderer m_Renderer;

        private void Awake()
        {
            m_Renderer.material = m_NormalMaterial;
        }

        private void OnEnable()
        {
            item.OnOver += HandleOver;
            item.OnOut += HandleOut;
            item.OnClick += HandleClick;
            item.OnDoubleClick += HandleDoubleClick;
        }

        private void OnDisable()
        {
            item.OnOver -= HandleOver;
            item.OnOut -= HandleOut;
            item.OnClick -= HandleClick;
            item.OnDoubleClick -= HandleDoubleClick;
        }

        private void HandleOver()
        {
            Debug.Log("Over Item");
            m_Renderer.material = m_NormalMaterial;
        }

        private void HandleOut()
        {
            Debug.Log("Out Item");
            m_Renderer.material = m_NormalMaterial;
        }

        private void HandleClick()
        {
            Debug.Log("Click Item");
            m_Renderer.material = m_ClickedMaterial;
        }

        private void HandleDoubleClick()
        {
            Debug.Log("Double Click Item");
            m_Renderer.material = m_DoubleClickMaterial;
        }
    } //end class
} //end namespace
