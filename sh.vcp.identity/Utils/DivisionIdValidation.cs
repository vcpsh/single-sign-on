using System;
using System.ComponentModel.DataAnnotations;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Utils
{
    public sealed class DivisionIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ldapConfig = (LdapConfig)validationContext.GetService(typeof(LdapConfig));
            if (validationContext.ObjectInstance is LdapModel model)
            {
                var divisionId = model.Dn.Replace($",{ldapConfig.GroupDn}", "");
                divisionId = divisionId.Substring(divisionId.LastIndexOf(",")).Replace(",cn=", "");
                return divisionId == value.ToString()
                    ? ValidationResult.Success
                    : new ValidationResult(
                        $"DivisionId \"{value.ToString()}\" does not match DnDivisionId \"{divisionId}\"");
            }

            return new ValidationResult("Validated Object is no LdapGroup");
        }
    }
}