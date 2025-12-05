using FluentValidation;
using SaaS.Platform.API.Application.DTOs.RolePermissions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateTenantdto with business rules
    /// </summary>
    public class CreateRolePermissionsdtoValidator : AbstractValidator<CreateRolePermissionsdto>
    {
        public CreateRolePermissionsdtoValidator()
        {
            

            

        }
    }

    /// <summary>
    /// Validator for UpdateRolePermissionsdto with business rules
    /// </summary>
    public class UpdateRolePermissionsdtoValidator : AbstractValidator<UpdateRolePermissionsdto>
    {
        public UpdateRolePermissionsdtoValidator()
        {


            


        }
    }
}