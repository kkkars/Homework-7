using RequestProcessor.App.Models;

namespace RequestProcessor.App
{
    internal class Response : IResponse
    {
        public Response(int statusCode, string content, bool handled)
        {
            Code = statusCode;
            Content = content;
            Handled = handled;
        }

        public bool Handled { get; set; }

        public int Code { get; set; }

        public string Content { get; set; }
    }
}
