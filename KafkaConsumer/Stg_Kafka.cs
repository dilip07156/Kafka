//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KafkaConsumer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Stg_Kafka
    {
        public System.Guid Row_Id { get; set; }
        public string Status { get; set; }
        public string Topic { get; set; }
        public string PayLoad { get; set; }
        public string Error { get; set; }
        public string Key { get; set; }
        public string Offset { get; set; }
        public string Partion { get; set; }
        public byte[] TimeStamp { get; set; }
        public string TopicPartion { get; set; }
        public string TopicPartionOffset { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Process_User { get; set; }
        public Nullable<System.DateTime> Process_Date { get; set; }
        public Nullable<System.Guid> Accommodation_Id { get; set; }
        public string message_log { get; set; }
    }
}
