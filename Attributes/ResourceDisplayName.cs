namespace Suflow.Common.Utils
{
    //To support cache with while 
    public class ResourceDisplayName: System.ComponentModel.DisplayNameAttribute
    {
        private readonly string _resourceValue = string.Empty;
       
        public ResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                //do not cache resources because it causes issues when you have multiple languages
                //if (!_resourceValueRetrived)
                //{
                //var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                //    _resourceValue = EngineContext.Current
                //        .Resolve<ILocalizationService>()
                //        .GetResource(ResourceKey, langId, true, ResourceKey);
                //    _resourceValueRetrived = true;
                //}
                return _resourceValue;
            }
        }

    }
}
