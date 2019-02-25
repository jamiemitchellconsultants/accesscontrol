using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Controllers;
using AccessControl.Messages;
using AccessControl.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace XUnitTestAccessControl
{

    public class RoleControllerTest : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public RoleControllerTest(DatabaseFixture dbFixture)
        {
            _databaseFixture = dbFixture;
        }

        [Fact]
        public async Task TestPost()
        {
            var testController = new RolesController(_databaseFixture.ACContext);
            var result = await testController.PostRole(new RolePost {RoleName = "TestRole"});
            Assert.NotNull(result);
            var resultValue = (result.Result as CreatedAtActionResult).Value as RoleResponse;

            var RoleId = resultValue.RoleId;
            var RoleRecord = await _databaseFixture.ACContext.Role.FindAsync(RoleId);
            Assert.Equal(RoleRecord.RoleId, RoleId);
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await testController.PostRole(new RolePost {RoleName = "TestRole"}));
            _databaseFixture.ACContext.Role.Remove(_databaseFixture.ACContext.Role.Find(RoleId));
            await _databaseFixture.ACContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TaskGetAll()
        {
            var Roles = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Roles.Add(guidString);
                var entityRole = new AccessControl.Models.Role
                    {RoleId = guidString, RoleName = $"name:{guidString}"};
                _databaseFixture.ACContext.Role.Add(entityRole);
            }

            await _databaseFixture.ACContext.SaveChangesAsync();

            var testController = new RolesController(_databaseFixture.ACContext);
            var result = await testController.GetRole();


            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult) result.Result;
            var testValue = (resultResult.Value as IEnumerable<RoleResponse>).ToList();
            foreach (var roleid in Roles)
            {

                Assert.Contains(testValue, o => o.RoleId == roleid);
            }
        }

        [Fact]
        public async Task Get()
        {
            var Roles = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Roles.Add(guidString);
                _databaseFixture.ACContext.Role.Add(new AccessControl.Models.Role
                    {RoleId = guidString, RoleName = $"name:{guidString}"});
            }

            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new RolesController(_databaseFixture.ACContext);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetRole(Roles[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult) result.Result;
                var resultId = (resultResult.Value as RoleResponse).RoleId;
                Assert.Equal(Roles[i], resultId);
            }
            var result404 = await testController.GetRole(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);

        }

        [Fact]
        public async Task AddPermission()
        {
            //create resource
            var testresourceController = new ResourcesController(_databaseFixture.ACContext);
            var testActionController = new ActionsController(_databaseFixture.ACContext);
            var testRoleController = new RolesController(_databaseFixture.ACContext);

            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var resourceresult =await testresourceController.PostResource(new ResourcePost {ResourceName = $"TestResource::{Guid.NewGuid().ToString()}",ApplicationAreaId = applicationareaId});
            var resourceId = ((resourceresult.Result as CreatedAtActionResult).Value as ResourceResponse).ResourceId;

            //create action
            var actionresult = await testActionController.PostAction(new ActionPost {ActionName = $"TestAction::{Guid.NewGuid().ToString()}"});

            //patch action to resource
            var patch = new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, ((actionresult.Result as CreatedAtActionResult).Value as ActionResponse).ActionId);
            await testresourceController.PatchResource(resourceId, patch);

            //create role
            var result = await testRoleController.PostRole(new RolePost {RoleName = $"name:{Guid.NewGuid().ToString()}" });
            var roleId = ((result.Result as CreatedAtActionResult).Value as RoleResponse).RoleId;

            //add permission
            var addPermission = new PermissionPost
            {
                ActionId = ((actionresult.Result as CreatedAtActionResult).Value as ActionResponse).ActionId,
                Deny = true, ResourceId = resourceId
            };
            var permissionResult = await testRoleController.PostPermission(roleId,addPermission);
            Assert.NotNull(permissionResult);
            var permissionResultResult= (Microsoft.AspNetCore.Mvc.OkObjectResult) permissionResult.Result;
            Assert.Equal(resourceId,(permissionResultResult.Value as PermissionResponseDetail).ResourceId);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await testRoleController.PostPermission(roleId, addPermission));
        }

        [Fact]
        public async Task DeleteRoleSimple()
        {
            var dbRole = _databaseFixture.ACContext.Role.Add(new Role
                {RoleId = Guid.NewGuid().ToString(), RoleName = $"delete::{Guid.NewGuid().ToString()}"});
            await _databaseFixture.ACContext.SaveChangesAsync();
            var roleid = dbRole.Entity.RoleId;
            var testController = new RolesController(_databaseFixture.ACContext);
            var result = await testController.DeleteRole(roleid);
            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            var resultId = (resultResult.Value as RoleResponse).RoleId;
            Assert.Equal(roleid, resultId);
        }

        [Fact]
        public async Task DeleteRoleMissing()
        {
            var testController = new RolesController(_databaseFixture.ACContext);
            var result404 = await testController.DeleteRole(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);

        }

        [Fact]
        public async Task DeleteRoleWithPermissions()
        {
            //create a role with permission
            var testresourceController = new ResourcesController(_databaseFixture.ACContext);
            var testActionController = new ActionsController(_databaseFixture.ACContext);
            var testRoleController = new RolesController(_databaseFixture.ACContext);

            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();


            var resourceresult = await testresourceController.PostResource(new ResourcePost { ResourceName = $"TestResource::{Guid.NewGuid().ToString()}" ,ApplicationAreaId = applicationareaId});
            var resourceId = ((resourceresult.Result as CreatedAtActionResult).Value as ResourceResponse).ResourceId;

            var actionresult = await testActionController.PostAction(new ActionPost { ActionName = $"TestAction::{Guid.NewGuid().ToString()}" });
            var actionId= ((actionresult.Result as CreatedAtActionResult).Value as ActionResponse).ActionId;
            var patch = new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, actionId);
            await testresourceController.PatchResource(resourceId, patch);

            var result = await testRoleController.PostRole(new RolePost { RoleName = $"name:{Guid.NewGuid().ToString()}" });
            var roleId = ((result.Result as CreatedAtActionResult).Value as RoleResponse).RoleId;

            var permissionResult = await testRoleController.PostPermission(roleId,
                new PermissionPost { ActionId = actionId, Deny = true, ResourceId = resourceId });
            
            var permissionResultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)permissionResult.Result;
            var permissionId = (permissionResultResult.Value as PermissionResponseDetail).PermissionId;
            var dbPermission = await _databaseFixture.ACContext.Permission.FindAsync(permissionId);
            Assert.Equal(dbPermission.RoleId,roleId);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testRoleController.DeleteRole(roleId));
            await testRoleController.DeletePermission(roleId, permissionId);

            var dbPermission2 = await _databaseFixture.ACContext.Permission.FindAsync(permissionId);

            await testRoleController.DeleteRole(roleId);
        }
    }
}