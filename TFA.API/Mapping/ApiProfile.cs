using AutoMapper;
using TFA.API.Models;

namespace TFA.API.Mapping;

internal class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<Domain.Models.Forum, Forum>();
        CreateMap<Domain.Models.Topic, Topic>();
    }
}
