using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Models.ViewModels
{
    public class ResFunctions
    {
    }

    public partial class SmallFunctionDTO
    {
        public int FunctionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string KeySearch { get; set; }
        public int? Level { get; set; }
        public bool disabled { get; set; }
        public bool? IsParamRoute { get; set; }
    }

    public partial class FunctionDT
    {
        public int id { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public int? location { get; set; }
        public bool? selected { get; set; }
        public bool? is_max { get; set; }
        public List<FunctionDT> children { get; set; }
    }

}
