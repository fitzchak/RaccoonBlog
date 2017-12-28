using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class UserAdminMapperProfile : Profile
	{
	    public UserAdminMapperProfile()
	    {
			CreateMap<User, UserSummeryViewModel>()
				.ForMember(x => x.Id, o => o.MapFrom(m => m.GetIdForUrl()))
				;

			CreateMap<UserInput, User>()
				.ForMember(x => x.Id, o => o.Ignore());

			CreateMap<User, UserInput>()
				.ForMember(x => x.Id, o => o.MapFrom(m => m.GetIdForUrl()))
				;
		}
	}
}