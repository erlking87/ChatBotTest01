using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

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
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

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
        //ThreadTest test = new ThreadTest();
        //delegate void del_thread(string str);

        //public static void new_thread(string name)
        //{
        //    Thread cur_thread = Thread.CurrentThread;
        //    Debug.WriteLine("Current {0} Thread = {1}", name, cur_thread.ManagedThreadId);
        //}

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //Thread workerThread = Thread.CurrentThread;
            //Worker workerObject = new Worker();
            //Thread workerThread = new Thread(workerObject.DoWork);

            // welcome message 출력   
            if (activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
            {
                
                WeatherInfo weatherInfo = await GetWeatherInfo();
                //weatherInfo.list[0].weather[0].description
                Debug.WriteLine("weatherInfo :  " + weatherInfo.list[0].weather[0].description);
                Debug.WriteLine("weatherInfo : " + string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.min, 1)));
                Debug.WriteLine("weatherInfo : " + string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.max, 1)));

                DateTime startTime = DateTime.Now;
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = activity.CreateReply("");
                
                reply.Recipient = activity.From;
                reply.Type = "message";
                reply.Attachments = new List<Attachment>();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                // Db
                DbConnect db = new DbConnect();
                List<Dialog> dlg = db.SelectDialog(3);
                Debug.WriteLine("!!!!!!!!!!! : "+ dlg[0].dlgId);
                List<Card> card = db.SelectDialogCard(dlg[0].dlgId);

                VideoCard[] plVideoCard = new VideoCard[card.Count];
                HeroCard[] plHeroCard = new HeroCard[card.Count];
                ReceiptCard[] plReceiptCard = new ReceiptCard[card.Count];
                Attachment[] plAttachment = new Attachment[card.Count];

                  
                for (int i = 0; i < card.Count; i++)
                {
                    List<Button> btn = db.SelectBtn(card[i].dlgId, card[i].cardId);
                    List<Image> img = db.SelectImage(card[i].dlgId, card[i].cardId); 
                    List<Media> media = db.SelectMedia(card[i].dlgId, card[i].cardId);

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction[] plButton = new CardAction[btn.Count];

                    ThumbnailUrl plThumnail = new ThumbnailUrl();

                    List<MediaUrl> mediaURL = new List<MediaUrl>();
                    MediaUrl[] plMediaUrl = new MediaUrl[media.Count];

                    for (int n = 0; n < img.Count; n++)
                    {
                        
                        if (img[n].imgUrl != null)
                        {
                            plThumnail.Url = img[n].imgUrl;
                        }
                    }

                    for (int l = 0; l < media.Count; l++)
                    {
                        if (media[l].mediaUrl != null)
                        {
                            plMediaUrl[l] = new MediaUrl()
                            {
                                Url = media[l].mediaUrl
                            };
                        }
                    }
                    mediaURL = new List<MediaUrl>(plMediaUrl);

                    for (int m = 0; m < btn.Count; m++)
                    {
                        if (btn[m].btnTitle != null)
                        {
                            plButton[m] = new CardAction()
                            {
                                Value = btn[m].btnContext,
                                Type = btn[m].btnType,
                                Title = btn[m].btnTitle
                            };
                        }
                    }
                    cardButtons = new List<CardAction>(plButton);

                    if (card[i].cardType == "videocard")
                    {
                        plVideoCard[i] = new VideoCard()
                        {
                            Title = card[i].cardTitle,
                            Text = card[i].cardText,
                            Subtitle = card[i].cardSubTitle,
                            Media = mediaURL,
                            Image = plThumnail,
                            Buttons = cardButtons,
                            Autostart = true
                        };
                        
                        plAttachment[i] = plVideoCard[i].ToAttachment();
                        reply.Attachments.Add(plAttachment[i]);
                    }
                }
                var reply1 = await connector.Conversations.SendToConversationAsync(reply);

                Debug.WriteLine("activity : " + activity.Id);
                Debug.WriteLine("activity : " + activity.ChannelId);
                Debug.WriteLine("activity : " + activity.Conversation.Id);
                Debug.WriteLine("activity : " + activity.Properties);
                Debug.WriteLine("activity : " + activity.Recipient);
                Debug.WriteLine("activity : " + activity.From.Id);
                Debug.WriteLine("activity : " + activity.Recipient.Id);
                Debug.WriteLine("end activity.Timestamp : " + activity.Timestamp);

                DateTime endTime = DateTime.Now;
                Debug.WriteLine("프로그램 수행시간 : {0}/ms", ((endTime - startTime).Milliseconds));
                //GetWeatherInfo();
                


            }
            else if (activity.Type == ActivityTypes.Message)
            {

                
                //test.resume();

                //Thread.Sleep(3000);
                //test.pause();

                //Thread.Sleep(3000);
                

                //if (workerThread.IsAlive)
                //{

                //    workerObject.RequestStop();
                //    workerThread.Join();
                //    Debug.WriteLine("main thread: Worker thread has terminated.");
                //}

                DateTime startTime = DateTime.Now;

                long unixTime = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
                Debug.WriteLine("startTime : " + startTime);
                Debug.WriteLine("startTime Millisecond : " + unixTime);

                Debug.WriteLine("Debuging : " + activity.Text);
                LUIS Luis = await GetIntentFromLUIS(activity.Text);
                Debug.WriteLine("Debuging :  " + Luis.intents[0].intent);
                Debug.WriteLine("Debuging : " + Luis.entities[0].entity);
                Debug.WriteLine("Debuging : " + Luis.entities[0].type);
                String entitiesStr = "";
                
                for(int i = 0; i < Luis.entities.Count(); i++)
                {

                    Debug.WriteLine("Split : " + Regex.Split(Luis.entities[i].type, "::")[1]);
                    entitiesStr += Regex.Split(Luis.entities[i].type, "::")[1] + ",";
                }

                entitiesStr = entitiesStr.Substring(0, entitiesStr.Length - 1);

                Debug.WriteLine("entitiesStr : " +entitiesStr);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // Db
                DbConnect db = new DbConnect();
                List<Luis> LuisDialogID = db.SelectLuis(Luis.intents[0].intent, entitiesStr);

                List<Dialog> dlg = db.SelectDialog(LuisDialogID[0].dlgId);
                
                if(dlg.Count > 0)
                {
                    if(dlg[0].dlgMent != null)
                    {
                        // return our reply to the user
                        Activity reply = activity.CreateReply(dlg[0].dlgMent);
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }
                
                List<Card> card = db.SelectDialogCard(LuisDialogID[0].dlgId);

                if(card.Count > 0)
                {
                    // HeroCard
                    Activity replyToConversation = activity.CreateReply("");

                    Debug.WriteLine("activity : " + activity.Id);
                    Debug.WriteLine("activity : " + activity.Properties);
                    Debug.WriteLine("activity : " + activity.Recipient);
                    Debug.WriteLine("activity : " + activity.Summary);
                    Debug.WriteLine("activity : " + activity.ReplyToId);
                    Debug.WriteLine("activity : " + activity.Recipient.Id);
                    Debug.WriteLine("activity : " + activity.Conversation.Id);

                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    for (int i = 0; i < card.Count; i++)
                    {
                        List<Button> btn = db.SelectBtn(card[i].dlgId, card[i].cardId);
                        List<Image> img = db.SelectImage(card[i].dlgId, card[i].cardId);
                        List<Media> media = db.SelectMedia(card[i].dlgId, card[i].cardId);

                        List<CardImage> cardImages = new List<CardImage>();
                        CardImage[] plImage = new CardImage[img.Count];

                        ThumbnailUrl plThumnail = new ThumbnailUrl();

                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction[] plButton = new CardAction[btn.Count];

                        List<MediaUrl> mediaURL = new List<MediaUrl>();
                        MediaUrl[] plMediaUrl = new MediaUrl[media.Count];

                        ReceiptCard[] plReceiptCard = new ReceiptCard[card.Count];
                        HeroCard[] plHeroCard = new HeroCard[card.Count];
                        VideoCard[] plVideoCard = new VideoCard[card.Count];
                        Attachment[] plAttachment = new Attachment[card.Count];



                        for (int l = 0; l < img.Count; l++)
                        {
                            if (card[i].cardType == "herocard")
                            {
                                if (img[l].imgUrl != null)
                                {
                                    plImage[l] = new CardImage()
                                    {
                                        Url = img[l].imgUrl
                                    };
                                }
                            }
                            else if (card[i].cardType == "videocard")
                            {
                                if (img[l].imgUrl != null)
                                {
                                    plThumnail.Url = img[l].imgUrl;
                                }
                            }
                        }
                        cardImages = new List<CardImage>(plImage);

                        for (int l = 0; l < media.Count; l++)
                        {
                            if (media[l].mediaUrl != null)
                            {
                                plMediaUrl[l] = new MediaUrl()
                                {
                                    Url = media[l].mediaUrl
                                };
                            }
                        }
                        mediaURL = new List<MediaUrl>(plMediaUrl);

                        for (int m = 0; m < btn.Count; m++)
                        {
                            if (btn[m].btnTitle != null)
                            {
                                plButton[m] = new CardAction()
                                {
                                    Value = btn[m].btnContext,
                                    Type = btn[m].btnType,
                                    Title = btn[m].btnTitle
                                };
                            }
                        }
                        cardButtons = new List<CardAction>(plButton);


                        if (card[i].cardType == "herocard")
                        {
                            plHeroCard[i] = new HeroCard()
                            {
                                Title = card[i].cardTitle,
                                Text = card[i].cardText,
                                Subtitle = card[i].cardSubTitle,
                                Images = cardImages,
                                Buttons = cardButtons
                            };

                            plAttachment[i] = plHeroCard[i].ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment[i]);
                        }
                        else if (card[i].cardType == "videocard")
                        {
                            plVideoCard[i] = new VideoCard()
                            {
                                Title = card[i].cardTitle,
                                Text = card[i].cardText,
                                Subtitle = card[i].cardSubTitle,
                                Image = plThumnail,
                                Media = mediaURL,
                                Buttons = cardButtons,
                                Autostart = true
                            };

                            plAttachment[i] = plVideoCard[i].ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment[i]);
                        }

                    }
                    var reply1 = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                
                DateTime endTime = DateTime.Now;
                Debug.WriteLine("프로그램 수행시간 : {0}/ms", ((endTime - startTime).Milliseconds));

                //Debug.WriteLine("current main thread = {0}",workerThread.ManagedThreadId);

                //del_thread new_th = new del_thread(new_thread);

                //new_th.BeginInvoke("TEST", null, null);
                //Thread.Sleep(3000);

                //// Start the worker thread.
                //workerThread.Start();
                //Debug.WriteLine("ID : "+ workerThread.ManagedThreadId);
                //Debug.WriteLine("main thread: Starting worker thread...");

                //// Loop until worker thread activates.
                //while (!workerThread.IsAlive) ;

                //Thread.Sleep(5);
                //test.resume();

                //Thread.Sleep(30000);
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        //private void GetWeatherInfo()
        //{
        //    throw new NotImplementedException();
        //}

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

        private static async Task<WeatherInfo> GetWeatherInfo()

        {
            WeatherInfo weather = new WeatherInfo();
            Debug.WriteLine("1");
            

            using (HttpClient client = new HttpClient())
            {
                string appId = "0221b2d1d8edb99a011cd7a3f152b756";

                string url = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&units=metric&cnt=1&APPID={1}", "Seoul", appId);

                Debug.WriteLine("2");

                HttpResponseMessage msg = await client.GetAsync(url);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    weather = JsonConvert.DeserializeObject<WeatherInfo>(JsonDataResponse);
                }

                //string json = client.DownloadString(url);

                //Debug.WriteLine("3");

                //WeatherInfo weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);

                //Debug.WriteLine("Weather Info :::::::::: " + weatherInfo.city.name + "," + weatherInfo.city.country);
                //Debug.WriteLine("Weather Info :::::::::: " + weatherInfo.list[0].weather[0].description);
                //Debug.WriteLine("Weather Info :::::::::: " + string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.min, 1)));
                //Debug.WriteLine("Weather Info :::::::::: " + string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.max, 1)));
                //Debug.WriteLine("Weather Info :::::::::: " + string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.day, 1)));
                //Debug.WriteLine("Weather Info :::::::::: " + weatherInfo.list[0].humidity.ToString());
                //lblCity_Country.Text = weatherInfo.city.name + "," + weatherInfo.city.country;

                //imgCountryFlag.ImageUrl = string.Format("http://openweathermap.org/images/flags/{0}.png", weatherInfo.city.country.ToLower());

                //lblDescription.Text = weatherInfo.list[0].weather[0].description;

                //imgWeatherIcon.ImageUrl = string.Format("http://openweathermap.org/img/w/{0}.png", weatherInfo.list[0].weather[0].icon);

                //lblTempMin.Text = string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.min, 1));

                //lblTempMax.Text = string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.max, 1));

                //lblTempDay.Text = string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.day, 1));

                //lblTempNight.Text = string.Format("{0}°С", Math.Round(weatherInfo.list[0].temp.night, 1));

                //lblHumidity.Text = weatherInfo.list[0].humidity.ToString();

                //tblWeather.Visible = true;

                return weather;

            }

        }

    }

    //public class Worker
    //{
    //    // This method will be called when the thread is started.
    //    public void DoWork()
    //    {
    //        while (!_shouldStop)
    //        {
    //            Debug.WriteLine("worker thread: working...");
    //            Thread.Sleep(5);
    //        }
    //        Debug.WriteLine("worker thread: terminating gracefully.");
    //    }
    //    public void RequestStop()
    //    {
    //        _shouldStop = true;
    //    }
    //    // Volatile is used as hint to the compiler that this data
    //    // member will be accessed by multiple threads.
    //    private volatile bool _shouldStop;
    //}


}