namespace MakeDsm
{
    internal partial class MySolution
    {
        internal partial class Project
        {
            public class Denpendency
            {
                public Project Project { get; }
                public Denpendency(Project project)
                {
                    this.Project = project;
                }

            }
        }

    }
}