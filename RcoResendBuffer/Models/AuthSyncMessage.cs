using RcoResendBuffer.Models.Enums;
using System;

namespace RcoResendBuffer.Models
{
    public class AuthSyncMessage
    {
        public int Id { get; set; }
        public Guid TransactionId { get; set; } 
        public string Data { get; set; }
        public string Url { get; set; }
        public HttpVerb HttpVerb { get; set; }
        public MessageState State { get; set; }

        public Guid HostId { get; set; }
    }

}
