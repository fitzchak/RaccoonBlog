using AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class TagsListViewModelMapperProfile : Profile
	{
	    public TagsListViewModelMapperProfile()
	    {
	        CreateMap<Tags_Count.ReduceResult, TagsListViewModel>()
	            ;
	    }
	}
}