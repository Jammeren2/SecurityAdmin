using System;
using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Employee
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public bool WeaponPermit { get; set; }
        public string IdCard { get; set; }
        public string Inn { get; set; }
        public string Pfr { get; set; }
        public decimal Bonus { get; set; }
        public DateTime HiredDate { get; set; }
        public DateTime? FiredDate { get; set; }
        public string License { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public List<SpecialTool> Tools { get; set; } = new List<SpecialTool>();

        public Employee(Dictionary<string, object> employee)
        {
            if (employee.Count > 0)
            {
                Id = (int)employee["id"];
                Surname = (string)employee["surname"];
                Name = (string)employee["name"];
                Patronymic = employee["patronymic"] != DBNull.Value ? (string)employee["patronymic"] : null;
                Address = (string)employee["address"];
                PositionId = (int)employee["positionid"];
                WeaponPermit = (bool)employee["weaponpermit"];
                IdCard = employee["idcard"] != DBNull.Value ? (string)employee["idcard"] : null;
                Inn = employee["inn"] != DBNull.Value ? (string)employee["inn"] : null;
                Pfr = employee["pfr"] != DBNull.Value ? (string)employee["pfr"] : null;
                Bonus = employee["bonus"] != DBNull.Value ? (decimal)employee["bonus"] : 0;
                HiredDate = (DateTime)employee["hireddate"];
                FiredDate = employee["fireddate"] != DBNull.Value ? (DateTime)employee["fireddate"] : (DateTime?)null;
                License = employee["license"] != DBNull.Value ? (string)employee["license"] : null;
                Login = employee["login"] != DBNull.Value ? (string)employee["login"] : null;
                Password = employee["password"] != DBNull.Value ? (string)employee["password"] : null;
                PositionName = employee.ContainsKey("positionname") ? (string)employee["positionname"] : string.Empty;
            }
        }

        public Employee() { }
    }
}