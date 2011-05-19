using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
    public class EmailViewModelMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<PostComments.Comment, NewCommentEmailViewModel>()
                .ForMember(x => x.PostId, o => o.Ignore())
                .ForMember(x => x.PostTitle, o => o.Ignore())
				.ForMember(x => x.Body, o => o.MapFrom(m => MarkdownResolver.Resolve(m.Body)))
                .ForMember(x => x.BlogName, o => o.Ignore())
                ;
        }
    }
}
