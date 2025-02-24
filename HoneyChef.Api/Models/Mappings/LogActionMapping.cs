using AutoMapper;
using IOITCore.Entities;
using IOITCore.Models.ViewModels;

namespace IOITCore.Models.Mappings
{
    public class LogActionMapping : Profile
    {
        public LogActionMapping()
        {
            CreateMap<LogAction, LogActionModel>();
            CreateMap<LogActionModel, LogAction>();

        }
    }
}
