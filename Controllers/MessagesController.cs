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
            // welcome message 출력   
            if (activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
            {

                //비디오카드 선언
                VideoCard vdCard = new VideoCard()
                {
                    Title = "그랜다이저",
                    Autostart = true,
                    Subtitle = "Grandizer",
                    Text = "안녕하세요. 저는 현대자동차의 그랜저 ig를 소개하는 그랜다이저예요. \n\n 대화중 언제든지'그랜다이저' 라고 입력하면 초기 화면으로 돌아가요.            \n\n Hi. My name is Grandizer. \n\n At any time, type 'Grandizer' to return to the initial screen. ",
                    Image = new ThumbnailUrl
                    {
                        Url = "http://webbot02.azurewebsites.net/hyundai/images/img_car01.jpg"
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://webbot02.azurewebsites.net/openning.wav"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Title = "가격",
                            Type = ActionTypes.ImBack,
                            Value = "가격을 보여줘"
                        },
                        new CardAction()
                        {
                            Title = "누르지마",
                            Type = ActionTypes.ImBack,
                            Value = "NO!NO!NO!"
                        }
                    }
                };

                Attachment plAttachment = new Attachment();
                plAttachment = vdCard.ToAttachment();

                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = activity.CreateReply("");

                reply.Recipient = activity.From;
                reply.Type = "message";
                reply.Attachments = new List<Attachment>();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                plAttachment = vdCard.ToAttachment();
                reply.Attachments.Add(plAttachment);

                var reply1 = await connector.Conversations.SendToConversationAsync(reply);



            }
            else if (activity.Type == ActivityTypes.Message)
            {
                Debug.WriteLine("Debuging : " + activity.Text);
                LUIS Luis = await GetIntentFromLUIS(activity.Text);
                Debug.WriteLine("Debuging :  " + Luis.intents[0].intent);
                Debug.WriteLine("Debuging : " + Luis.entities[0].entity);
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // Db
                DbConnect db = new DbConnect();
                List<Car> card = db.SelectDb(Luis.intents[0].intent);

                // return our reply to the user
                Activity reply = activity.CreateReply(card[0].cardMent);
                await connector.Conversations.ReplyToActivityAsync(reply);

                // HeroCard
                Activity replyToConversation = activity.CreateReply("");

                replyToConversation.Recipient = activity.From;
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();
                replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                List<CardImage>[] cardImages = new List<CardImage>[card.Count];
                List<CardAction>[] cardButtons = new List<CardAction>[card.Count];
                CardAction[] plButton = new CardAction[card.Count];
                HeroCard[] plCard = new HeroCard[card.Count];
                Attachment[] plAttachment = new Attachment[card.Count];

                for (int i = 0; i < card.Count; i++)
                {
                    if(card[i].erase == "F")
                    {
                        cardImages[i] = new List<CardImage>();
                        if (card[i].cardImage != null)
                        {
                            cardImages[i].Add(new CardImage(url: card[i].cardImage));
                        }
                        
                        cardButtons[i] = new List<CardAction>();
                        if (card[i].cardButton != null)
                        {
                            Debug.WriteLine("???? : " + card[i].cardButton);

                            plButton[i] = new CardAction()
                            {
                                Value = card[i].cardButtonContent,
                                Type = "imBack",
                                Title = card[i].cardButton
                            };
                            cardButtons[i].Add(plButton[i]);
                        }
                        //else
                        //{
                            
                        //}

                        if(card[i].cardType == "HEROCARD")
                        {
                            plCard[i] = new HeroCard()
                            {
                                Title = card[i].cardTitle,
                                Text = card[i].cardText,
                                Subtitle = card[i].cardSubTitle,
                                Images = cardImages[i],
                                Buttons = cardButtons[i]
                            };
                        }
                        
                        plAttachment[i] = plCard[i].ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment[i]);
                    }
                }

                var reply1 = await connector.Conversations.SendToConversationAsync(replyToConversation);
            }
            else
            {
                HandleSystemMessage(activity);
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
            //string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=28745440-fd03-4658-b190-9c154fe89d89&subscription-key=7efb093087dd48918b903885b944740c&q=" + Query;
              string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=1adab70c-f7a6-4d5c-9809-c27672653896&subscription-key=7489b95cf3fb4797939ea70ce94a4b11&q=" + Query;
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