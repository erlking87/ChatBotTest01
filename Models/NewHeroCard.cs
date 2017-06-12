using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Models
{
    public class NewHeroCard : Microsoft.Bot.Connector.HeroCard
    {
        String str;

        public NewHeroCard() : base() { }

        public NewHeroCard(String a, String b, String c, IList<CardImage> d, IList<CardAction> e, CardAction f) : base(a, b, c, d, e, f)
        {

        }
        [Newtonsoft.Json.JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }
        public void setStr(String str)
        {
            this.str = str;
        }
    }
}