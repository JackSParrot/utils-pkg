using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services.AppReview
{
    [CreateAssetMenu(fileName = "EmptyAppReviewService", menuName = "JackSParrot/Services/EmptyAppReviewService")]
    public class EmptyAppReviewService : AAppReviewService
    {
        public override bool CanShowReview()
        {
            UnityEngine.Debug.Log("D: CanShowReview");
            return true;
        }

        public override void ShowReview()
        {
            UnityEngine.Debug.Log("D: ShowReview");
            Application.OpenURL("www.google.com");
        }

        public override void Cleanup()
        {
            
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            Status = EServiceStatus.Initialized;
            yield return null;
        }
    }
}