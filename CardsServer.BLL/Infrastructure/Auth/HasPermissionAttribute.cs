﻿using CardsServer.BLL.Infrastructure.Auth.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CardsServer.BLL.Infrastructure.Auth
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public string POLICY_PREFIX = "HasPermission";
        public HasPermissionAttribute(Permission permission) : base(permission.ToString())
        {
        }
    }
}
