using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Coefficient { get; set; }
        public decimal PricePerHour { get; set; }

        public Event(Dictionary<string, object> eventData)
        {
            if (eventData.Count > 0)
            {
                Id = (int)eventData["id"];
                Name = (string)eventData["name"];
                Coefficient = (decimal)eventData["coefficient"];
                PricePerHour = (decimal)eventData["priceperhour"];
            }
        }

        public Event() { }
    }
}