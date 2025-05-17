using System.Collections.Generic;

namespace SecurityAdmin3
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Role(Dictionary<string, object> role)
        {
            if (role.Count > 0)
            {
                Id = (int)role["id"];
                Name = (string)role["name"];
            }
        }

        public Role()
        {
        }
    }
}