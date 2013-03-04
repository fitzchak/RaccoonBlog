RaccoonBlog
===========

A Blog engine powered by ASP.NET MVC web application framework and RavenDB document database

To get to Admin
---------------
http://localhost/admin

RaccoonBlog research
--------------------
* why have both posts.CommentsCount && posts.Comments ????
* figure out where/how the SendEmailTask gets invoked -> AddCommentTask (when a comment gets posted to a post, unauthenticated user)
* figure out if there is a reason NOT to update to ASP.NET MVC 4 (it's currently on ASP.NET MVC 3)

Dependencies
------------
- xUnit to run integration tests

Some Specific ways of contributing to RaccoonBlog
-------------------------------------------------
* move calls to HttpUtility.HtmlDecode(post.Title) into post model
* move calls to SlugConverter.TitleToSlug(post.Title) into post model
* write integration tests for AddCommentTask
* inject AkismetService.CheckForSpam into AddCommentTask
* inject into PostDetailsController:
-- TaskExecutor
-- CommenterUtil.SetCommenterCookie
* move tasks to their own messaage queue applications (doing a task would mean putting a message on the qeueue)
* specify that it is ok for Owner email to be ; separated
* move ConfigurationManager.AppSettings["OwnerEmail"] to confg entry in the database
* integrate twitter bootstrap
* fix all broken unit tests

In Progress
-----------
* update RaccoonBlog to use ASP.NET MVC 4 framework (is now currently on ASP.NET MVC 3) (verifying)

Done
----
* update MarkdownDeep to 1.5
* update DataAnnotationExteions to latest version