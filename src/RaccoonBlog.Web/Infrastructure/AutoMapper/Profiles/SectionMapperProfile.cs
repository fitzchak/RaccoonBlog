using AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class SectionMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Section, SectionDetails>()
                ;
        }
    }
}