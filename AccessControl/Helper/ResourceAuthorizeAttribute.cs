using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AccessControl.Helper
{
    /// <summary>
    /// Custom authorization for dynamic policies
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ExternalPermissionAttribute:AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "ExternalPermission";
        private string _resource;
        private string _action;
        /// <summary>
        /// Constructs a policy name
        /// </summary>
        /// <param name="resource">resoucrce the policy applies to e.g. widget</param>
        /// <param name="action">action the policy applies to e.g. PUT</param>
        public ExternalPermissionAttribute( string resource, string action) : base()
        {
            _resource = resource;
            _action = action;
            Policy = $"{POLICY_PREFIX}::{_resource}::{_action}";
        }
        
    }
    /// <summary>
    /// Creates dynamic policies
    /// </summary>
    public class ExternalPermissisonPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX = "ExternalPermission";

        /// <summary>
        /// Creates external permission policy
        /// </summary>
        /// <param name="policyName">dynamic plicy name</param>
        /// <returns>A dynamically created policy</returns>
        public  Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(POLICY_PREFIX)) return null;
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new ExternalPermissionRequirement(policyName));
            return Task.FromResult(policy.Build());

        }
        /// <summary>
        /// Default policy
        /// </summary>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            return Task.FromResult(policy.Build());
        }


    }
    /// <summary>
    /// Definition of an external dynamic policy
    /// </summary>
    public class ExternalPermissionRequirement : IAuthorizationRequirement
    {
        private const string POLICY_PREFIX = "ExternalPermission";
        /// <summary>
        /// Resource e.g. widget
        /// </summary>
        public string Resource { get; private set; }
        /// <summary>
        /// Action e,.g. POST
        /// </summary>
        public string Action { get; private set; }
        /// <summary>
        /// Constructor parses the policy name
        /// </summary>
        /// <param name="policyName"></param>
        public ExternalPermissionRequirement(string policyName)
        {
            var parts = policyName.Split("::");
            Resource = parts[1];
            Action = parts[2];
        }

    }
    /// <summary>
    /// Calls the policy evaluator
    /// </summary>
    public class ExternalPermissionHandler : AuthorizationHandler<ExternalPermissionRequirement>
    {
        private ICallPermissionCheck _permissionCheck;
        private IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// Dunno
        /// </summary>
        /// <param name="permissionCheck">dunno</param>
        /// <param name="contextAccessor">dunno</param>
        public ExternalPermissionHandler(ICallPermissionCheck permissionCheck,IHttpContextAccessor contextAccessor)
        {
            _permissionCheck = permissionCheck;
            _contextAccessor = contextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ExternalPermissionRequirement requirement)
        {

            //_contextAccessor.HttpContext.Request.Headers.
            //call the permission check point
            var subjectId = context.User.Claims.FirstOrDefault()?.Value;
            if (subjectId == null)
            {
                context.Fail();
                return;
            }
            var resource = requirement.Resource;
            var action = requirement.Action;

            var result = await _permissionCheck.CheckPermission(subjectId, resource, action);
            if (result)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return;
        }
    }


}
