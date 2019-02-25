using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AccessControl.Models;

namespace AccessControl.Helper
{
    /// <summary>
    /// Call an external service to validate permissions for a user
    /// </summary>
    public class RemotePermissionCheck:ICallPermissionCheck
    {
        private RemotePermissionCheckConfig _config;

        /// <summary>
        /// Inject configuration
        /// </summary>
        /// <param name="config"></param>
        public RemotePermissionCheck(RemotePermissionCheckConfig config)
        {
            _config = config;
        }
        /// <summary>
        /// Check specific permission
        /// </summary>
        /// <param name="JWT">Containing the subject id</param>
        /// <param name="resource">The resource to check. e.g. widget </param>
        /// <param name="action">The action to check. e.g. POST</param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(string JWT, string resource, string action)
        {
            var httpClient = new HttpClient();

            var path =
                $"https://{_config.Host}{_config.Path.Replace("{resource}", resource).Replace("{action}", action)}";
            var response =await  httpClient.GetAsync(path);
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get a list of permissions for a user
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns>Deny=true/false</returns>
        public async Task<IQueryable<UserPermissionCheck>> GetPermission(string subjectId)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Config for external permission checker
    /// </summary>
    public class RemotePermissionCheckConfig
    {
        /// <summary>
        /// host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// path
        /// </summary>
        public string Path { get; set; }
    }

}
