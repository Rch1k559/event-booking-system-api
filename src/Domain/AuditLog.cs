using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public Guid EntityId { get; set; }
        public string Details { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
