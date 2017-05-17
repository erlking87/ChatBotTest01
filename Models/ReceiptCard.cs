namespace Microsoft.Bot.Connector
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class ReceiptCard
    {

        public ReceiptCard() { }


        public ReceiptCard(string title = default(string), IList<ReceiptItem> items = default(IList<ReceiptItem>), IList<Fact> facts = default(IList<Fact>), CardAction tap = default(CardAction), string total = default(string), string tax = default(string), string vat = default(string), IList<CardAction> buttons = default(IList<CardAction>))
        {
            Title = title;
            Items = items;
            Facts = facts;
            Tap = tap;
            Total = total;
            Tax = tax;
            Vat = vat;
            Buttons = buttons;
        }
        

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }


        [JsonProperty(PropertyName = "items")]
        public IList<ReceiptItem> Items { get; set; }



        [JsonProperty(PropertyName = "facts")]
        public IList<Fact> Facts { get; set; }


        [JsonProperty(PropertyName = "tap")]
        public CardAction Tap { get; set; }


        [JsonProperty(PropertyName = "total")]
        public string Total { get; set; }


        [JsonProperty(PropertyName = "tax")]
        public string Tax { get; set; }


        [JsonProperty(PropertyName = "vat")]
        public string Vat { get; set; }


        [JsonProperty(PropertyName = "buttons")]
        public IList<CardAction> Buttons { get; set; }
    }
}

