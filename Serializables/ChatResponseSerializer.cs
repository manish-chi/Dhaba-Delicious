using Microsoft.AspNetCore.Builder;

namespace Dhaba_Delicious.Serializables
{

    public class ChatResponseSerializer
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string input { get; set; }
        public object[] chat_history { get; set; }
        public string output { get; set; }
        public Intermediatestep[] intermediateSteps { get; set; }
    }

    public class Intermediatestep
    {
        public Action action { get; set; }
        public string observation { get; set; }
    }

    public class Action
    {
        public string tool { get; set; }
        public Toolinput toolInput { get; set; }
        public string toolCallId { get; set; }
        public string log { get; set; }
        public Messagelog[] messageLog { get; set; }
    }

    public class Toolinput
    {
        public ItemX[] items { get; set; }
    }

    public class ItemX
    {
        public string dish { get; set; }
        public int quantity { get; set; }
    }

    public class Messagelog
    {
        public int lc { get; set; }
        public string type { get; set; }
        public string[] id { get; set; }
        public Kwargs kwargs { get; set; }
    }

    public class Kwargs
    {
        public string content { get; set; }
        public Additional_Kwargs additional_kwargs { get; set; }
        public Response_Metadata response_metadata { get; set; }
        public Tool_Call_Chunks[] tool_call_chunks { get; set; }
        public string id { get; set; }
        public Usage_Metadata usage_metadata { get; set; }
        public Tool_Calls1[] tool_calls { get; set; }
        public object[] invalid_tool_calls { get; set; }
    }

    public class Additional_Kwargs
    {
        public Tool_Calls[] tool_calls { get; set; }
    }

    public class Tool_Calls
    {
        public Function function { get; set; }
        public string id { get; set; }
        public int index { get; set; }
        public string type { get; set; }
    }

    public class Function
    {
        public string arguments { get; set; }
        public string name { get; set; }
    }

    public class Response_Metadata
    {
        public int prompt { get; set; }
        public int completion { get; set; }
        public Usage usage { get; set; }
        public string finish_reason { get; set; }
        public string system_fingerprint { get; set; }
        public string model_name { get; set; }
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public Completion_Tokens_Details completion_tokens_details { get; set; }
        public int prompt_tokens { get; set; }
        public Prompt_Tokens_Details prompt_tokens_details { get; set; }
        public int total_tokens { get; set; }
    }

    public class Completion_Tokens_Details
    {
        public int accepted_prediction_tokens { get; set; }
        public int audio_tokens { get; set; }
        public int reasoning_tokens { get; set; }
        public int rejected_prediction_tokens { get; set; }
    }

    public class Prompt_Tokens_Details
    {
        public int audio_tokens { get; set; }
        public int cached_tokens { get; set; }
    }

    public class Usage_Metadata
    {
        public int input_tokens { get; set; }
        public int output_tokens { get; set; }
        public int total_tokens { get; set; }
        public Input_Token_Details input_token_details { get; set; }
        public Output_Token_Details output_token_details { get; set; }
    }

    public class Input_Token_Details
    {
        public int audio { get; set; }
        public int cache_read { get; set; }
    }

    public class Output_Token_Details
    {
        public int audio { get; set; }
        public int reasoning { get; set; }
    }

    public class Tool_Call_Chunks
    {
        public string name { get; set; }
        public string args { get; set; }
        public string id { get; set; }
        public int index { get; set; }
        public string type { get; set; }
    }

    public class Tool_Calls1
    {
        public string name { get; set; }
        public Args args { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Args
    {
        public DataItems[] items { get; set; }
    }

    public class DataItems
    {
        public string dish { get; set; }
        public int quantity { get; set; }
    }
}
