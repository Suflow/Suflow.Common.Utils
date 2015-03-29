using System.ComponentModel.DataAnnotations;

namespace Suflow.Common.Utils
{
    public class EmailAttribute : RegularExpressionAttribute//, IClientValidatable
    {
        public EmailAttribute()
            : base(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})$")
        {

        }

        //public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRegexRule(this.ErrorMessageString, base.Pattern);
        //    return new[] { rule };
        //}
    }
}