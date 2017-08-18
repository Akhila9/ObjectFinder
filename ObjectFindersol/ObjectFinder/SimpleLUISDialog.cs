using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ObjectFinder
{
    [LuisModel("Luis Application ID", "Subscription Key")]
    [Serializable]
    public class SimpleLUISDialog : LuisDialog<object>
    {


        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I am a bot named ObjFinder");
            context.Wait(MessageReceived);
        }
        [LuisIntent("Location")]
        public async Task Location(IDialogContext context, LuisResult result)
        {
            context.Call(new jamesloc(), ResumeAfterjameslocDialog);
        }

        public async Task ResumeAfterjameslocDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageHandled = await result;


            await context.PostAsync($"We hope we helped to find the desired object");

            context.Wait(MessageReceived);
        }


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I have no idea what you are talking about.");
            context.Wait(MessageReceived);
        }
        [LuisIntent("Cards")]
        public async Task Cards(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();
            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("Hyderabad", "http://www.culturalindia.net/iliimages/Charminar-ili-45-img-4.jpg");
            cardContentList.Add("Chennai", "https://msdnholidays.files.wordpress.com/2011/03/chennai-central.jpg");
            cardContentList.Add("Bangalore", "https://storage.googleapis.com/ehimages/2017/4/25/img_4e75a976a5b5e1a3fd486d2d7716d1e1_1493091778253_original.jpg");

            foreach (KeyValuePair<string, string> cardContent in cardContentList)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: cardContent.Value));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                    Type = "openUrl",
                    Title = "WikiPedia Page"
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = $"About {cardContent.Key}",
                    Subtitle = $"{cardContent.Key} Wikipedia Page",
                    Images = cardImages,
                    Buttons = cardButtons
                };


                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
       
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();
            var thumbnailCard = new ThumbnailCard
            {
                Title = "Object Finder Help",
                Subtitle = "Support",
                Text = "We do help you to find the location of a partciular object",
                Images = new List<CardImage> {
                    new CardImage("http://blogs.studentlife.utoronto.ca/lifeatuoft/files/2017/03/help-007.jpg") },
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.OpenUrl, "View More", value: "https://github.com/Microsoft/BotBuilder-Location") }
            };
            Attachment plAttachment = thumbnailCard.ToAttachment();
            message.Attachments.Add(plAttachment);
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);

        }
    }
}
