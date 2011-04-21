using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class UserAdminMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<UserInput, User>()
                .ConstructUsing(input => GetUserByEmail(input.Email) ?? new User())
                ;

            Mapper.CreateMap<User, UserSummeryViewModel>()
                ;
        }
    }
}