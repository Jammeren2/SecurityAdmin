using System;
using System.Collections.Generic;

namespace SecurityAdmin3
{
    public abstract class Client
    {
        public int Id { get; set; }
        public string TITLE { get; set; }
        public string Address { get; set; }
        public int Id_type { get; set; }
        public string Type { get; set; }
        public bool IsPhysical { get; set; }

        public override string ToString()
        {
            return TITLE;
        }   
    }

    public class ClientFizlico : Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string PasportSerNum { get; set; }
        public int PasportSer { get; set; }
        public int PasportNum { get; set; }
        public DateTime PasportData { get; set; }
        public string PasportVydan { get; set; }

        public ClientFizlico(Dictionary<string, object> client)
        {
            if (client.Count > 0)
            {
                base.Id = (int)client["ID"];
                base.IsPhysical = true;
                base.Id_type = 1;
                base.Type = "Физ.лицо";

                if (client.ContainsKey("NAME") && client.ContainsKey("SURNAME"))
                {
                    Name = (string)client["NAME"];
                    Surname = (string)client["SURNAME"];
                    Patronymic = (string)client["PATRONYMIC"];
                    base.Address = client["ADRES"] != DBNull.Value ? (string)client["ADRES"] : string.Empty;
                    Phone = client["PHONE"] != DBNull.Value ? (string)client["PHONE"] : string.Empty;
                    PasportSer = client["PASPORT_SER"] != DBNull.Value ? (int)client["PASPORT_SER"] : 0;
                    PasportNum = client["PASPORT_NUM"] != DBNull.Value ? (int)client["PASPORT_NUM"] : 0;
                    PasportData = client["PASPORT_DT"] != DBNull.Value ? (DateTime)client["PASPORT_DT"] : DateTime.MinValue;
                    PasportVydan = client["PASPORT_VYDAN"] != DBNull.Value ? (string)client["PASPORT_VYDAN"] : string.Empty;
                    PasportSerNum = PasportSer + " / " + PasportNum;
                    base.TITLE = $"{Surname} {Name[0]}.{(!string.IsNullOrEmpty(Patronymic) ? Patronymic[0] + "." : "")}";
                }
                else if (client.ContainsKey("TITLE"))
                {
                    base.TITLE = client["TITLE"] != DBNull.Value ? (string)client["TITLE"] : string.Empty;
                    Name = string.Empty;
                    Surname = string.Empty;
                    Patronymic = string.Empty;
                    Phone = string.Empty;
                    PasportSer = 0;
                    PasportNum = 0;
                    PasportData = DateTime.MinValue;
                    PasportVydan = string.Empty;
                    base.Address = string.Empty;
                }
            }
        }
    }

    public class ClientJurlico : Client
    {
        public string Dogovor_num { get; set; }
        public DateTime Dogovor_dt { get; set; }
        public DateTime Dogovor_end { get; set; }
        public string Phone { get; set; }

        public ClientJurlico(Dictionary<string, object> client)
        {
            if (client.Count > 0)
            {
                base.Id = client.ContainsKey("ID") ? (int)client["ID"] : 0;
                base.TITLE = client.ContainsKey("TITLE") && client["TITLE"] != DBNull.Value ? (string)client["TITLE"] : string.Empty;
                base.Address = client.ContainsKey("ADRES") ? (string)client["ADRES"] : string.Empty;
                this.Phone = client.ContainsKey("PHONE") && client["PHONE"] != DBNull.Value ? (string)client["PHONE"] : string.Empty;
                Dogovor_num = client.ContainsKey("DOGOVOR_NUM") && client["DOGOVOR_NUM"] != DBNull.Value ? (string)client["DOGOVOR_NUM"] : string.Empty;
                Dogovor_dt = client.ContainsKey("DOGOVOR_DT") && client["DOGOVOR_DT"] != DBNull.Value ? (DateTime)client["DOGOVOR_DT"] : DateTime.MinValue;
                Dogovor_end = client.ContainsKey("DOGOVOR_END") && client["DOGOVOR_END"] != DBNull.Value ? (DateTime)client["DOGOVOR_END"] : DateTime.MinValue;
                base.Id_type = 2;
                base.Type = "Юр.лицо";
                base.IsPhysical = false;
            }
        }
    }
}