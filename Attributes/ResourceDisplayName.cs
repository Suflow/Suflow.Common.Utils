////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
namespace Suflow.Common.Utils
{   
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
