using System;
using System.ComponentModel.DataAnnotations;
using sh.vcp.ldap;

namespace sh.vcp.identity.Utils
{
    public sealed class DivisionIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ldapConfig = (LdapConfig) validationContext.GetService(typeof(LdapConfig));

            if (ldapConfig == null)
            {
                throw new Exception("Ldap config not found.");
            }
            
            if (!(validationContext.ObjectInstance is LdapModel model))
            {
                return new ValidationResult("Validated Object is no LdapGroup");
            }

            var divisionId = model.Dn.Replace($",{ldapConfig.GroupDn}", "");
            divisionId = divisionId.Substring(divisionId.LastIndexOf(",", StringComparison.Ordinal)).Replace(",cn=", "");
            return divisionId == value.ToString()
                ? ValidationResult.Success
                : new ValidationResult(
                    $"DivisionId \"{value}\" does not match DnDivisionId \"{divisionId}\"");
        }
    }
}