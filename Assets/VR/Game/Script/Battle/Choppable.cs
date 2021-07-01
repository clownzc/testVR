
using System;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
    public class Choppable : MonoBehaviour
    {
        public bool canBeChopped;
        public bool hasBeenChopped;
        public Transform[] bones;
        public Choppable[] children;
        public Transform parentBoneTransform;
        public float chopThreshold = 20f;
        public Transform chopPoint;

        private SkinnedMeshRenderer _meshRenderer;

        public SkinnedMeshRenderer meshRenderer
        {
            get { return _meshRenderer; }
            set { _meshRenderer = value; }
        }

        private void Start()
        {
            var enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                meshRenderer = enemy.skinnedMeshRenderer;
            }
        }
    }
}
