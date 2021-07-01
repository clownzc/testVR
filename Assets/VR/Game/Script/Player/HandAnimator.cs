// Decompile from assembly: Assembly-CSharp.dll
// ILSpyBased#2
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class HandAnimator : MonoBehaviour
    {
        public enum HandPose
        {
            Open,
            Zap,
            Fist,
            Grab,
            GrabAnticipate,
            Point
        }

        private Animator anim;

        private List<HandPose> allPoses;

        public HandPose currentPose;

        public void Open()
        {
            this.currentPose = HandPose.Open;
        }

        public void Zap()
        {
            this.currentPose = HandPose.Zap;
        }

        public void Close()
        {
            this.currentPose = HandPose.Fist;
        }

        public void Grab()
        {
            this.currentPose = HandPose.Grab;
        }

        public void GrabAnticipate()
        {
            this.currentPose = HandPose.GrabAnticipate;
        }

        public void Point()
        {
            this.currentPose = HandPose.Point;
        }

        private void Start()
        {
            this.anim = base.GetComponent<Animator>();
            this.allPoses = new List<HandPose>();
            IEnumerator enumerator = Enum.GetValues(typeof(HandPose)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this.allPoses.Add((HandPose)current);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (enumerator as IDisposable)) != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < this.allPoses.Count; i++)
            {
                float pose = this.anim.GetFloat(this.allPoses[i].ToString());
                if (this.allPoses[i] == this.currentPose)
                {
                    pose = Mathf.MoveTowards(pose, 1f, Time.deltaTime * 5f);
                    this.anim.SetFloat(this.allPoses[i].ToString(), pose);
                }
                else
                {
                    pose = Mathf.MoveTowards(pose, 0f, Time.deltaTime * 5f);
                    this.anim.SetFloat(this.allPoses[i].ToString(), pose);
                }
            }
        }
    }
}