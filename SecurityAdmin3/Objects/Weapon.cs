using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string RegNumber { get; set; }

        public Weapon(Dictionary<string, object> weapon)
        {
            if (weapon.Count > 0)
            {
                Id = (int)weapon["id"];
                Brand = (string)weapon["brand"];
                RegNumber = (string)weapon["regnumber"];
            }
        }

        public Weapon() { }

        public override string ToString()
        {
            return $"{Brand} (№{RegNumber})";
        }
    }
}