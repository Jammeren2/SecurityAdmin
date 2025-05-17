using System;
using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Payment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public Payment(Dictionary<string, object> paymentData)
        {
            if (paymentData.Count > 0)
            {
                Id = paymentData["id"] != DBNull.Value ? (int)paymentData["id"] : 0;
                ClientId = paymentData["clientid"] != DBNull.Value ? (int)paymentData["clientid"] : 0;
                ClientName = paymentData.ContainsKey("clientname") && paymentData["clientname"] != DBNull.Value
                    ? (string)paymentData["clientname"] : string.Empty;
                ContractId = paymentData["contractid"] != DBNull.Value ? (int)paymentData["contractid"] : 0;
                ContractNumber = paymentData.ContainsKey("contractnumber") && paymentData["contractnumber"] != DBNull.Value
                    ? (string)paymentData["contractnumber"] : string.Empty;
                Amount = paymentData["amount"] != DBNull.Value ? (decimal)paymentData["amount"] : 0m;
                PaymentDate = paymentData["paymentdate"] != DBNull.Value ? (DateTime)paymentData["paymentdate"] : DateTime.MinValue;
            }
        }

        public Payment()
        {
            ClientName = string.Empty;
            ContractNumber = string.Empty;
        }
    }
}