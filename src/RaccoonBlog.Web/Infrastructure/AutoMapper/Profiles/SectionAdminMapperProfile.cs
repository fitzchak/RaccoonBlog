using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class SectionAdminMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Section, SectionSummery>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                ;
            
            Mapper.CreateMap<Section, SectionInput>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                ;

            Mapper.CreateMap<SectionInput, Section>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Position, o => o.Ignore())
                ;
        }
    }
}
