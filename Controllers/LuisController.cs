using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;

namespace Bot_Application1
{
    [LuisModel("28745440-fd03-4658-b190-9c154fe89d89", "7efb093087dd48918b903885b944740c")]
    [Serializable]
    public class LuisController : LuisDialog<object>
    {

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"None";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = $"greeting";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}