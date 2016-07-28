using System;
using System.Collections.Generic;
using System.Text;

namespace Shopping.DemoApp.Helpers
{
    public class RatingMsgHelper
    {
        public static string GetInitialRatingTitle()
        {
            return "Smile if you like our app! Align your face to the silhouette and take a photo.";
        }

        public static string GetRecognizingRatingTitle()
        {
            return "Please wait a few seconds for us to run facial recognition.";
        }

        public static string GetRecognizingRatingDescription()
        {
            return "Perfect! Processing now...";
        }

        public static string GetNoFaceDetectedTitle()
        {
            return "No face detected.";
        }

        public static string GetNoFaceDetectedMsg()
        {
            return "No face detected. Please, try again.";
        }
    }
}
