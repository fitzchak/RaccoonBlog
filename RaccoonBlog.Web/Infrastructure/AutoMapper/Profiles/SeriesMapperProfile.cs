namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    using global::AutoMapper;
    using Profiles.Resolvers;
    using Common;
    using Indexes;
    using ViewModels;

    public class SeriesMapperProfile : Profile
    {
        public SeriesMapperProfile()
        {
            CreateMap<Posts_Series.PostInformation, PostInSeries>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.PublishAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Slug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
                .ForMember(x => x.Title, o => o.MapFrom(m => m.Title))
                ;

            CreateMap<Posts_Series.Result, SeriesInfo>()
                .ForMember(x => x.SeriesTitle, o => o.MapFrom(x => x.Series))
                .ForMember(x => x.PostsInSeries, o => o.Ignore())
                ;
        }
    }
}