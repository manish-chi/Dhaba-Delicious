using Daba_Delicious.Cards;
using Dhaba_Delicious.Interfaces;
using Dhaba_Delicious.Serializables;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class ResponseManager
    {
        private IConfiguration _configuration;
        private IResponseService _responseService;
        private CardManager _cardManager;

        public ResponseManager(IConfiguration configuration,CardManager cardManager,IResponseService responseService)
        {
            _configuration = configuration;
            _responseService = responseService;
            _cardManager = cardManager;
        }

        public async Task<IMessageActivity> GetWelcomeReponseAsync(string token,ITurnContext context)
        {
            var llmResponse = await _responseService.GetResponseAsync<ResponseSerializer>(token, _configuration["GetWelcomeReponseURL"],context.Activity.Conversation.Id);

            var reply = context.Activity.CreateReply();

            reply.Text = llmResponse.data;

            return reply;
        }

        public async Task<List<IActivity>> GetDefaultResponseAsync(ITurnContext context,string userQuery,string token)
        {
            var messages = new List<IActivity>();

            var llmDefaultResponse = await _responseService.GetResponseWithBody<NodeTemplateSerializer>(userQuery, token, _configuration["LLMChatURL"],context.Activity.Conversation.Id);

            var reply = context.Activity.CreateReply();

            reply.Text = llmDefaultResponse.data;

            messages.Add(reply);

            return messages;
        }
    }
}
