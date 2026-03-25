namespace IPL.Gaming.Common.Models.CosmosDB
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DeleteStatus
    {
        public long Deleted { get; set; }
        public bool Continuation { get; set; }
    }
}
