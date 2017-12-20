using System.Collections.Generic;
using System.Linq;

namespace RaccoonBlog.Web.Models.SocialNetwork
{
    public class Reddit
    {
        public class PostSubmission
        {
            public string Subreddit { get; set; }

            public SubmissionStatus? Status { get; set; }

            public int Attempts { get; set; }

            public bool ShouldTrySubmit => Attempts < 3 &&
                                          (Status.HasValue == false ||
                                           Status == SubmissionStatus.UnknownFailure);

            public bool IsManualSubmissionPending => Status.HasValue && 
                Status.Value == SubmissionStatus.ManualSubmissionPending;

            public bool IsFailure => Status == SubmissionStatus.CaptchaFailure ||
                                     Status == SubmissionStatus.ManualSubmissionFailure ||
                                     Status == SubmissionStatus.UnknownFailure;
        }

        public enum SubmissionStatus
        {
            CaptchaFailure,
            UnknownFailure,
            Submitted,
            ManualSubmissionPending,
            ManualSubmissionFailure
        }

        public HashSet<PostSubmission> PostSubmissions { get; set; }

        public IList<PostSubmission> GetFailedSubmissions()
        {
            return PostSubmissions
                .Where(x => x.Status == SubmissionStatus.CaptchaFailure || 
                            x.Status == SubmissionStatus.UnknownFailure ||
                            x.Status == SubmissionStatus.ManualSubmissionFailure)
                .ToList();
        }

        public bool Submitted
        {
            get
            {
                return PostSubmissions != null &&
                    PostSubmissions.Any() && 
                    PostSubmissions.All(x => x.Status == SubmissionStatus.Submitted);
            }
        }

        public PostSubmission GetPostSubmissionForSubreddit(string sr)
        {
            return PostSubmissions.SingleOrDefault(x => x.Subreddit == sr);
        }

        public void RegisterPostSubmission(PostSubmission postSubmission)
        {
            postSubmission.Attempts++;
            PostSubmissions.Add(postSubmission);
        } 
   }
}