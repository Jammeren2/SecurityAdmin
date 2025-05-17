using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public Position(Dictionary<string, object> position)
        {
            if (position.Count > 0)
            {
                Id = (int)position["id"];
                Name = (string)position["name"];
                Salary = (decimal)position["salary"];
                RoleId = (int)position["roleid"];
                RoleName = position.ContainsKey("rolename") ? (string)position["rolename"] : string.Empty;
            }
        }

        public Position() { }
    }
}