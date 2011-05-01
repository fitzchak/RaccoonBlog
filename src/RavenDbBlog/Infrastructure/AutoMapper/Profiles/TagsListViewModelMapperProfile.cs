using AutoMapper;
using RavenDbBlog.Indexes;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class TagsListViewModelMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<TempTagCount, TagsListViewModel>()
                ;
        }
    }
}