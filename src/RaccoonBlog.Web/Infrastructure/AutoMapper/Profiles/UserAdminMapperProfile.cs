using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class UserAdminMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<User, UserSummeryViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                ;

        	Mapper.CreateMap<UserInput, User>()
        		.ForMember(x => x.Id, o => o.Ignore());

            Mapper.CreateMap<User, UserInput>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                ;
        }
    }
}
