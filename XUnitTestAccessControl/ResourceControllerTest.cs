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
using SQLitePCL;
using Xunit;

namespace XUnitTestAccessControl
{

    public class ResourceControllerTest : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public ResourceControllerTest(DatabaseFixture dbFixture)
        {
            _databaseFixture = dbFixture;
        }

        [Fact]
        public async Task TestDelete()
        {
            await PatchAddActionTest();

        }

        [Fact]
        public async Task TestPost()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                {ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}"});
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ResourcesController(_databaseFixture.ACContext);

            var result = await testController.PostResource(new ResourcePost { ResourceName = "TestResource" , ApplicationAreaId = applicationareaId});
            Assert.NotNull(result);
            var resultValue = (result.Result as CreatedAtActionResult).Value as ResourceResponse;

            var ResourceId = resultValue.ResourceId;
            var ResourceRecord = await _databaseFixture.ACContext.Resource.FindAsync(ResourceId);
            Assert.Equal(ResourceRecord.ResourceId, ResourceId);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testController.PostResource(new ResourcePost() { ResourceName = "TestResource", ApplicationAreaId = applicationareaId}));

        }

        [Fact]
        public async Task TaskGetAll()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var Resources = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Resources.Add(guidString);
                _databaseFixture.ACContext.Resource.Add(new AccessControl.Models.Resource
                { ResourceId = guidString, ResourceName = $"name:{guidString}" ,ApplicationAreaId = applicationareaId});
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ResourcesController(_databaseFixture.ACContext);
            var result = await testController.GetResource();


            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            var testValue = (resultResult.Value as IEnumerable<ResourceResponse>).ToList();
            for (var i = 0; i < 10; i++)
            {
                Assert.Contains(testValue, o => o.ResourceId == Resources[i]);
            }
            var result404 = await testController.GetResource(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);

        }

        [Fact]
        public async Task Get()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var Resources = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Resources.Add(guidString);
                _databaseFixture.ACContext.Resource.Add(new AccessControl.Models.Resource
                { ResourceId = guidString, ResourceName = $"name:{guidString}",ApplicationAreaId = applicationareaId});
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ResourcesController(_databaseFixture.ACContext);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetResource(Resources[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
                var resultId = (resultResult.Value as ResourceResponse).ResourceId;
                Assert.Equal(Resources[i], resultId);
            }

        }

        [Fact]
        public async Task PatchAddActionTest()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var Resources = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Resources.Add(guidString);
                _databaseFixture.ACContext.Resource.Add(new AccessControl.Models.Resource
                    { ResourceId = guidString, ResourceName = $"name:{guidString}" , ApplicationAreaId = applicationareaId});
            }
            var actions = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                actions.Add(guidString);
                _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                    { ActionId = guidString, ActionName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ResourcesController(_databaseFixture.ACContext);
            var patch= new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, actions[0]);
            patch.Add(o => o.ActionId, actions[1]);
            var patchResponse=await testController.PatchResource(Resources[0], patch);

            var resourceaction = await _databaseFixture.ACContext.Resourceaction.ToListAsync();

            Assert.Equal(resourceaction.Where(o => o.ResourceId == Resources[0]).First().ActionId, actions[0]);

        }
        [Fact]
        public async Task PatchRemoveActionTest()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var Resources = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Resources.Add(guidString);
                _databaseFixture.ACContext.Resource.Add(new AccessControl.Models.Resource
                {
                    ResourceId = guidString, ResourceName = $"name:{guidString}", ApplicationAreaId = applicationareaId

                });
            }
            var actions = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                actions.Add(guidString);
                _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                    { ActionId = guidString, ActionName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ResourcesController(_databaseFixture.ACContext);
            var patch = new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, actions[0]);
            patch.Add(o => o.ActionId, actions[1]);
            var patchResponse = await testController.PatchResource(Resources[0], patch);

            var resourceaction = await _databaseFixture.ACContext.Resourceaction.ToListAsync();

            var resourceActionID = resourceaction[0].ResourceActionId;
            var removePatch=new JsonPatchDocument<ResourcePatch>();
            removePatch.Add(o => o.DeleteResourceActionId, resourceActionID);
            var resourceRemove = await testController.PatchResource(Resources[0], removePatch);
            Assert.DoesNotContain(_databaseFixture.ACContext.Resourceaction, o=>o.ResourceActionId==resourceActionID);
        }

        [Fact]
        public async Task DeleteResourceTest()
        {
            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            //create a resource
            var resourceId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Resource.Add(new Resource
                {ResourceId = resourceId, ResourceName = $"resource::{resourceId}", ApplicationAreaId = applicationareaId});
            _databaseFixture.ACContext.SaveChanges();
            //create and action
            var actionId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                {ActionId = actionId, ActionName = $"action::{actionId}"});
            _databaseFixture.ACContext.SaveChanges();
            //patch the action to a resource
            var resourceActionId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Resourceaction.Add(new Resourceaction
                {ActionId = actionId, ResourceId = resourceId, ResourceActionId = resourceActionId});
            _databaseFixture.ACContext.SaveChanges();
            //create a role
            var roleId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Role.Add(new Role{RoleId = roleId,RoleName = $"role::{roleId}"});
            //add the permission to a role
            var permissionId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Permission.Add(new Permission{Deny = Convert.ToByte( true),PermissionId = permissionId,ResourceActionId = resourceActionId,RoleId = roleId});
            _databaseFixture.ACContext.SaveChanges();

            //delete the resource
            var testController=new ResourcesController(_databaseFixture.ACContext);
            var deleteResult =await testController.DeleteResource(resourceId);
            var resultResult = deleteResult.Result as OkObjectResult;
            var resourceResponse = resultResult.Value as ResourceResponse;
            Assert.Equal(resourceId,resourceResponse.ResourceId);
            //check the resource is gone
            var resourceGone = _databaseFixture.ACContext.Resource.Find(resourceId);
            Assert.Null(resourceGone);
            //check the resource action is gone

            //check the action still exists

            //check the permission is gone

            //check the role exists
            var roleHere = _databaseFixture.ACContext.Role.Find(roleId);
            Assert.Equal(roleId,roleHere.RoleId);
        }

    }
}