using System;
using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Contract
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public Contract(Dictionary<string, object> contractData)
        {
            if (contractData.Count > 0)
            {
                Id = contractData.ContainsKey("id") ? (int)contractData["id"] : 0;
                ContractNumber = contractData.ContainsKey("contractnumber") && contractData["contractnumber"] != DBNull.Value
                    ? (string)contractData["contractnumber"] : string.Empty;
                ClientId = contractData.ContainsKey("ClientID") ? (int)contractData["ClientID"] : 0;
                ClientName = contractData.ContainsKey("clientname") && contractData["clientname"] != DBNull.Value
                    ? (string)contractData["clientname"] : string.Empty;
                ObjectId = contractData.ContainsKey("ObjectID") ? (int)contractData["ObjectID"] : 0;
                ObjectName = contractData.ContainsKey("objectname") && contractData["objectname"] != DBNull.Value
                    ? (string)contractData["objectname"] : string.Empty;
                StartDate = contractData.ContainsKey("startdate") ? (DateTime)contractData["startdate"] : DateTime.Now;
                EndDate = contractData.ContainsKey("enddate") ? (DateTime)contractData["enddate"] : DateTime.Now.AddYears(1);
                Price = contractData.ContainsKey("price") ? (decimal)contractData["price"] : 0m;
                Description = contractData.ContainsKey("description") && contractData["description"] != DBNull.Value
                    ? (string)contractData["description"] : string.Empty;
                Address = contractData.ContainsKey("address") && contractData["address"] != DBNull.Value
                    ? (string)contractData["address"] : string.Empty;
            }
        }

        public Contract()
        {
            ContractNumber = string.Empty;
            ClientName = string.Empty;
            ObjectName = string.Empty;
            Description = string.Empty;
            Address = string.Empty;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(1);
        }
    }
}