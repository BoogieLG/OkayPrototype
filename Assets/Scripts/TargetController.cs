using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TargetController : MonoBehaviour
    {
        MeshRenderer _meshRenderer;
        Collider _collider;
        bool _isHitted = false;

        public void GetHit()
        {
            _meshRenderer.enabled = false;
            _collider.enabled = false;
            _isHitted = true;
        }

        public bool GetStatus()
        {
            return _isHitted;
        }

        public void Restart()
        {
            _meshRenderer.enabled = true;
            _collider.enabled = true;
            _isHitted = false;
        }

        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }
    }
}
