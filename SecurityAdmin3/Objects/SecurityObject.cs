using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class SecurityObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Coefficient { get; set; }
        public decimal PricePerHour { get; set; }

        public SecurityObject(Dictionary<string, object> objectData)
        {
            if (objectData.Count > 0)
            {
                Id = (int)objectData["id"];
                Name = (string)objectData["name"];
                Coefficient = (decimal)objectData["coefficient"];
                PricePerHour = (decimal)objectData["priceperhour"];
            }
        }

        public SecurityObject() { }
    }
}