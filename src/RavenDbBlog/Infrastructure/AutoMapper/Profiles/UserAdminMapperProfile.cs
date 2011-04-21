using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class UserAdminMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<User, UserSummeryViewModel>()
                ;

            Mapper.CreateMap<User, UserInput>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                ;
        }
    }
}