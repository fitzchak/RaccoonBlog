using System;
using AutoMapper;
using Microsoft.AspNetCore.Html;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper
{
	public class AutoMapperConfiguration
	{
	    public static void Configure()
	    {
	        Mapper.Initialize(cfg =>
	        {
	            cfg.CreateMap<string, HtmlString>().ConvertUsing<HtmlStringConverter>();
	            cfg.CreateMap<Guid, string>().ConvertUsing<GuidToStringConverter>();

	            cfg.CreateMap<Guid, string>().ConvertUsing<GuidToStringConverter>();

	            cfg.CreateMap<DateTimeOffset, DateTime>().ConvertUsing<DateTimeTypeConverter>();


	            // TODO: It would make sense to add all of those automatically with an IoC.
	            cfg.AddProfile(new PostViewModelMapperProfile());
	            cfg.AddProfile(new PostsViewModelMapperProfile());
	            cfg.AddProfile(new TagsListViewModelMapperProfile());
	            cfg.AddProfile(new SectionMapperProfile());
	            cfg.AddProfile(new EmailViewModelMapperProfile());
	            cfg.AddProfile(new SeriesMapperProfile());

	            cfg.AddProfile(new UserAdminMapperProfile());
	            cfg.AddProfile(new PostsAdminViewModelMapperProfile());
	        });
	    }
	}
}