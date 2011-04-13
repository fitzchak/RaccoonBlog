using System.Threading.Tasks;

namespace RavenDbBlog.Commands
{
    public static class CommandExcucator
    {
        public static void ExcuteLater(ICommand command)
        {
            Task.Factory.StartNew(command.Execute, TaskCreationOptions.LongRunning);
        }
    }
}