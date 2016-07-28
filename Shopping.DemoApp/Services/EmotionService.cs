namespace Shopping.DemoApp.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.ProjectOxford.Common;
    using Microsoft.ProjectOxford.Emotion.Contract;
    using Models;
    public class EmotionService : ServiceClient
    {
        public EmotionService()
        {
            this.ApiRoot = "https://api.projectoxford.ai/emotion/v1.0";
            this.AuthKey = "Ocp-Apim-Subscription-Key";
            this.AuthValue = AppSettings.EmotionApiKey;
        }

        public async Task<Emotion[]> RecognizeAsync(Stream imageStream)
        {
            return await this.PostAsync<Stream, Emotion[]>("/recognize", imageStream);
        }

        public RatingText GetTextsFor(float rating)
        {
            var texts = new RatingText();
            if (rating > 0.7)
            {
                texts.Top = "Great! Glad you like the app!";
                texts.Message = "Oh, beautiful smile! Thank you!";
            }
            else if (rating > 0.4)
            {
                texts.Top = "The app is good for you, but could be better.";
                texts.Message = "OK. Maybe not good enough :-|";
            }
            else
            {
                texts.Top = "You don't like the app after all.";
                texts.Message = "Ooops, it seems that you are sad :-(";
            }

            return texts;
        }
    }
}