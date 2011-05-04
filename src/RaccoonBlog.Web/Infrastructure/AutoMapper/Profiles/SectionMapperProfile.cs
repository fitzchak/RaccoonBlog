using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
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