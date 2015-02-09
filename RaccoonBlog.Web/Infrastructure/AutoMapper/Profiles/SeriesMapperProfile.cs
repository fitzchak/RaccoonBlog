namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    using global::AutoMapper;
    using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
    using RaccoonBlog.Web.Infrastructure.Common;
    using RaccoonBlog.Web.Infrastructure.Indexes;
    using RaccoonBlog.Web.ViewModels;

    public class SeriesMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Posts_Series.PostInformation, PostInSeries>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.PublishAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Slug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
                .ForMember(x => x.Title, o => o.MapFrom(m => m.Title))
                ;

            Mapper.CreateMap<Posts_Series.Result, SeriesInfo>()
                .ForMember(x => x.SeriesTitle, o => o.MapFrom(x => x.Series))
                .ForMember(x => x.PostsInSeries, o => o.Ignore())
                ;
        }
    }
}