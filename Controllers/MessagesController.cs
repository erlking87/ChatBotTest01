using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Bot_Application1.Lib;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Diagnostics;


namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //Console.WriteLine("aaa : "+ activity.Text);
                Debug.WriteLine("Debuging : " + activity.Text);
                LUIS Luis = await GetIntentFromLUIS(activity.Text);
                Debug.WriteLine("Debuging : " + Luis.intents[0].intent);
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                //await Conversation.SendAsync(activity, () => new LuisController());

                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                await connector.Conversations.ReplyToActivityAsync(reply);

                // Db
                DbConnect db = new DbConnect();
                List<Car> car = db.SelectDb(Luis.intents[0].intent);

                // HeroCard
                Activity replyToConversation = activity.CreateReply("Should go to conversation, with a hero card");

                replyToConversation.Recipient = activity.From;
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();
                replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                List<CardImage>[] cardImages = new List<CardImage>[car.Count];
                List<CardAction>[] cardButtons = new List<CardAction>[car.Count];
                CardAction[] plButton = new CardAction[car.Count];
                HeroCard[] plCard = new HeroCard[car.Count];
                Attachment[] plAttachment = new Attachment[car.Count];

                for (int i = 0; i < car.Count; i++)
                {
                    if(car[i].erase == "F")
                    {
                        cardImages[i] = new List<CardImage>();
                        cardImages[i].Add(new CardImage(url: car[i].carImage));

                        cardButtons[i] = new List<CardAction>();                        
                        plButton[i] = new CardAction()
                        {
                            Value = car[i].carButtonContent,
                            Type = "imBack",
                            Title = car[i].carButton
                        };
                        cardButtons[i].Add(plButton[i]);

                        plCard[i] = new HeroCard()
                        {
                            Title = car[i].carTitle,
                            Images = cardImages[i],
                            Buttons = cardButtons[i]
                        };

                        plAttachment[i] = plCard[i].ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment[i]);
                    }
                }

                var reply1 = await connector.Conversations.SendToConversationAsync(replyToConversation);
                //await Conversation.SendAsync(activity, () => new LuisController());
            }
            else
            {
                //HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private static async Task<LUIS> GetIntentFromLUIS(string Query)
    {
        Query = Uri.EscapeDataString(Query);
        LUIS Data = new LUIS();
        using (HttpClient client = new HttpClient())
        {
            string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=28745440-fd03-4658-b190-9c154fe89d89&subscription-key=7efb093087dd48918b903885b944740c&q=" + Query;
            HttpResponseMessage msg = await client.GetAsync(RequestURI);

            if (msg.IsSuccessStatusCode)
            {
                var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                Data = JsonConvert.DeserializeObject<LUIS>(JsonDataResponse);
            }
        }
        return Data;
    }

    }

}