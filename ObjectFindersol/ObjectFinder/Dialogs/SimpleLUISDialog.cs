using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using ObjectFinder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ObjectFinder
{
    [LuisModel("aed3a3e8-9922-495d-8e9e-6d1f1227f6f6", "61d437de2cec4f949ba1c13e4342375b")]
    [Serializable]
    public class SimpleLUISDialog : LuisDialog<object>
    {

        //string name;
        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I am a bot named ObjFinder");
            context.Wait(MessageReceived);
        }
        [LuisIntent("Location")]
        public async Task Location(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            EntityRecommendation entityplace;
            List<string> entitiespresent = new List<string>();
            foreach (var entityList in result.Entities)
            {
                entitiespresent.Add(entityList.Entity);
            }
            if (entitiespresent.Count!=0)
            {
                foreach (var epl in entitiespresent)
                {
                    string Loc = epl;
                    if (result.TryFindEntity("Places", out entityplace))
                    {
                      var heroCard = new HeroCard
                        {
                            Title = "Map for " + Loc,
                            Images = new List<CardImage> {
                    new CardImage("https://maps.googleapis.com/maps/api/staticmap?zoom=13&size=600x300&maptype=roadmap&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyAUT0zYwpPk82Y7BsCL1_VSM5Sohk8FEYQ&center="+Loc) },
                            Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.ShowImage, "View", value: "https://www.google.co.in/maps/place/"+Loc) }
                        };

                        reply.Attachments.Add(heroCard.ToAttachment());
                    }

               }
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync($"Enter valid location");
            }


            
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
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
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
                    Buttons = cardButtons,
                    Tap=plButton
                };


                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("sample")]
        public async Task Sample(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi, Let me help to find a location.");
            PromptDialog.Text(context, this.LocationDialogResumeAfter, "Enter Location", "Try again", 3);
            
            PromptDialog.Text
        }
        private async Task LocationDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            
                string loc = await result;
                var message = context.MakeMessage();
                message.Attachments = new List<Attachment>();
                var heroCard = new HeroCard
                {
                    Title = "Sample location",
                    Images = new List<CardImage> {
                    new CardImage("https://maps.googleapis.com/maps/api/staticmap?zoom=13&size=600x300&maptype=roadmap&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyAUT0zYwpPk82Y7BsCL1_VSM5Sohk8FEYQ&center="+loc) },
                    Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.ShowImage, "View", value: "https://www.google.co.in/maps/place/"+loc) }
                };
                Attachment p2Attachment = heroCard.ToAttachment();
                message.Attachments.Add(p2Attachment);

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
