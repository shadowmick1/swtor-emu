namespace NodeViewer
{
    using Hero;
    using Hero.Definition;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Worker
    {
        protected GOM gom;
        protected static Worker instance;
        protected Log log;
        protected Repository repository;

        public event InitCompleteHandler OnInitComplete;

        public Worker()
        {
            instance = this;
            this.gom = GOM.Instance;
            this.log = Log.Instance;
            this.repository = Repository.Instance;
            new Thread(new ThreadStart(this.DoWork)).Start();
        }

        protected void DoWork()
        {
            Config.Load();
            this.InitializeRepository();
            if (this.repository.RepositoryFiles.Count != 0)
            {
                this.InitializeObjectModel();
                if (this.OnInitComplete != null)
                {
                    this.OnInitComplete();
                }
            }
        }

        protected void InitializeObjectModel()
        {
            this.log.AddLog("Initializing object model");
            this.gom.Parse(this.repository.GetFile("/resources/systemgenerated/client.gom"));
            int count = this.gom.DefinitionsByName[HeroDefinition.Types.Enumeration].Count;
            int num2 = this.gom.DefinitionsByName[HeroDefinition.Types.Association].Count;
            int num3 = this.gom.DefinitionsByName[HeroDefinition.Types.Field].Count;
            int num4 = this.gom.DefinitionsByName[HeroDefinition.Types.Class].Count;
            int num5 = this.gom.DefinitionsByName[HeroDefinition.Types.Node].Count;
            this.log.AddLog(string.Format("client.gom loaded ({0} enums, {1} associations, {2} fields, {3} classes, {4} nodes)", new object[] { count, num2, num3, num4, num5 }));
            if (Config.LoadPrototypeNodes)
            {
                this.log.AddLog("Loading prototype nodes");
                this.gom.LoadPrototypes(this.repository.GetFile("/resources/systemgenerated/prototypes.info"));
                num5 = this.gom.DefinitionsByName[HeroDefinition.Types.Node].Count - num5;
                this.log.AddLog(string.Format("Prototype nodes loaded ({0} nodes)", new object[] { num5 }));
            }
            this.log.AddLog("Loading prototype buckets");
            for (int i = 0; i < 500; i++)
            {
                this.gom.LoadBucket(i);
            }
            num5 = this.gom.DefinitionsByName[HeroDefinition.Types.Node].Count - num5;
            this.log.AddLog(string.Format("Prototype buckets loaded ({0} nodes)", new object[] { num5 }));
            this.log.AddLog("Object model initialization complete");
            this.gom.SortRoot();
        }

        protected void InitializeRepository()
        {
            this.log.AddLog("Initializing assets");
            try
            {
                this.repository.Initialize(Config.AssetsPath);
                if (this.repository.RepositoryFiles.Count == 0)
                {
                    this.log.AddLog("The assets directory doesn't contain asset files. Update the config file with the right path to the asset files");
                }
            }
            catch (DirectoryNotFoundException)
            {
                this.log.AddLog("The assets directory could not be found. Update the config file with the right path to the asset files");
            }
            this.log.AddLog(string.Format("Assets initialization complete ({0} files)", this.repository.RepositoryFiles.Count));
        }

        public static Worker Instance
        {
            get
            {
                return instance;
            }
        }

        public delegate void InitCompleteHandler();
    }
}

