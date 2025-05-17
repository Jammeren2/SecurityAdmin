using System;
using System.Collections.Generic;

namespace SecurityAdmin3.Objects
{
    public class SpecialTool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }

        public SpecialTool(Dictionary<string, object> tool)
        {
            if (tool.Count > 0)
            {
                Id = (int)tool["id"];
                Name = (string)tool["name"];
                Number = (string)tool["number"];
                Description = tool["description"] != DBNull.Value ? (string)tool["description"] : string.Empty;
            }
        }

        public SpecialTool() { }

        public override string ToString()
        {
            return $"{Name} (№{Number})";
        }
    }
}