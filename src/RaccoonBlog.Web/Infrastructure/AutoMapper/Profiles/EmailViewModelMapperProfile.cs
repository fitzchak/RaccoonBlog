using AutoMapper;
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
                .ForMember(x => x.BlogName, o => o.Ignore())
                ;
        }
    }
}