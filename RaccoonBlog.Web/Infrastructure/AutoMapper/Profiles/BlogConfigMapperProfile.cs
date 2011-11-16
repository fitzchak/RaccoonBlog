using AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class BlogConfigMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<BlogConfig, BlogConfigViewModel>()
                .ForMember(x => x.CustomCss, o => o.MapFrom(m => (m.CustomCss ?? "").ToLowerInvariant()))
                ;
        }
    }
}
