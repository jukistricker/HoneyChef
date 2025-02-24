using System;
using System.Collections.Generic;
using System.Text;

namespace IOITCore.Repositories.Bases
{
    public class AppSpParameter
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public dynamic Value { get; set; }

        public AppSpParameter(string name, dynamic value)
        {
            Name = name;
            Value = value;
        }

        public AppSpParameter(string name, dynamic value, string typeName)
        {
            Name = name;
            TypeName = typeName;
            Value = value;
        }
    }
}
