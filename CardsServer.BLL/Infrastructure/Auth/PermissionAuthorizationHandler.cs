﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
namespace CardsServer.BLL.Infrastructure.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            string? userId = context.User.Claims.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (!Int32.TryParse(userId, out int parsedUserId))
            {
                return;
            }

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IPermissionService permissionService = scope.ServiceProvider
                .GetRequiredService<IPermissionService>();

            HashSet<string> permissions = await permissionService
                .GetPermissionsAsync(parsedUserId);

            if(permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
