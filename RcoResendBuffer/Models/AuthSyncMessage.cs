using RcoResendBuffer.Models.Enums;

namespace RcoResendBuffer.Models
{
    public class AuthSyncMessage
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public HttpVerb HttpVerb { get; set; }
        public MessageState State { get; set; }
    }

}
