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

    public class UserControllerTest : IClassFixture<TestFixtures>
    {
        private DatabaseFixture _databaseFixture;
        private PermissionCheckFixture _permissionFixture;

        public UserControllerTest(TestFixtures testFixtures)
        {
            _databaseFixture = testFixtures.DatabaseFixture;
            _permissionFixture = testFixtures.PermissionCheckFixture;
        }

        [Fact]
        public async Task TestPatch()
        {
            string userId = Guid.NewGuid().ToString();
            string subjectId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.User.Add(new User
                { UserId = userId, SubjectId = subjectId, LocalName = $"{userId}::{subjectId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var testController=new UsersController(_databaseFixture.ACContext,_permissionFixture.PermissionCheck);
            var patchDocument=new JsonPatchDocument<UserPatch>();
            patchDocument.Add(o => o.LocalName , $"{subjectId}::{userId}");
            var result = testController.PatchUser(subjectId, patchDocument);

            Assert.Equal($"{subjectId}::{userId}", _databaseFixture.ACContext.User.Find(userId).LocalName);
        }

        [Fact]
        public async Task TestPost()
        {
            var testController = new UsersController(_databaseFixture.ACContext, _permissionFixture.PermissionCheck);
            var subjectID = Guid.NewGuid().ToString();
            var result = await testController.PostUser(new UserPost { LocalName = "TestUser" , SubjectId = subjectID});
            Assert.NotNull(result);
            var resultValue = (result.Result as CreatedAtActionResult).Value as UserResponse;

            var UserId = resultValue.UserId;
            var UserRecord = await _databaseFixture.ACContext.User.FindAsync(UserId);
            Assert.Equal(UserRecord.UserId, UserId);
            Assert.Equal(UserRecord.SubjectId,subjectID);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testController.PostUser(new UserPost() { LocalName = "TestUser2",SubjectId = subjectID}));

        }

        [Fact]
        public async Task TaskGetAll()
        {
            var Users = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Users.Add(guidString);
                _databaseFixture.ACContext.User.Add(new AccessControl.Models.User
                { UserId = guidString, LocalName = $"name:{guidString}", SubjectId = Guid.NewGuid().ToString()});
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new UsersController(_databaseFixture.ACContext, _permissionFixture.PermissionCheck);
            var result = await testController.GetUser();
            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;

            var testValue = (resultResult.Value as IEnumerable<UserResponse>).ToList();
            for (var i = 0; i < 10; i++)
            {
                Assert.Contains(testValue, o => o.UserId == Users[i]);
            }
        }

        [Fact]
        public async Task Get()
        {
            var Users = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                var w = guidString.ToCharArray();
                Array.Reverse(w);
                var rev=new string( w);
                Users.Add(guidString);
                _databaseFixture.ACContext.User.Add(new AccessControl.Models.User
                { UserId = rev, LocalName = $"name:{guidString}" ,SubjectId = guidString});
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new UsersController(_databaseFixture.ACContext,_permissionFixture.PermissionCheck);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetUser(Users[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
                var resultId = (resultResult.Value as UserResponse).SubjectId;
                Assert.Equal(Users[i], resultId);
            }
            var result404 = await testController.GetUser(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);

        }

        [Fact]
        public async Task CheckPermission()
        {
            #region CreateUsers
            var userIds = new List<string>();
            var subjectIds = new List<string>();

                for (int i = 0; i< 10; i++)
            {
                var userId = Guid.NewGuid().ToString();
                userIds.Add(userId);
                var subjectId = Guid.NewGuid().ToString();
                subjectIds.Add(subjectId);
                _databaseFixture.ACContext.User.Add(new User
                    {UserId = userId, SubjectId = subjectId, LocalName = $"{userId}::{subjectId}"});
            }

            await _databaseFixture.ACContext.SaveChangesAsync();
            #endregion

            #region Create a group
            //Create a group
            var groupId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Group.Add(new Group {GroupId = groupId, GroupName = $"name::{groupId}"});
            await _databaseFixture.ACContext.SaveChangesAsync();

            var testGroupController = new GroupsController(_databaseFixture.ACContext);
            #endregion

            #region Patch Users into group
            //Patch users into group
            var addUserPatch = new JsonPatchDocument<GroupPatch>();
            addUserPatch.Add(o => o.AddUserId, new[] {userIds[0], userIds[1], userIds[2]});
            addUserPatch.Add(o => o.AddUserId, userIds[3]);
            var addUserResponse = await testGroupController.PatchGroup(groupId, addUserPatch);

            //Create some roles
            var roleIds = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                var roleId = Guid.NewGuid().ToString();
                roleIds.Add(roleId);
                _databaseFixture.ACContext.Role.Add(new Role
                    { RoleId = roleId, RoleName = $"role::{roleId}" });
            }

            await _databaseFixture.ACContext.SaveChangesAsync();

            //Patch roles to group
            var addRolePatch = new JsonPatchDocument<GroupPatch>();
            addRolePatch.Add(o => o.AddRoleId, new[] { roleIds[0], roleIds[1], roleIds[2] });
            addRolePatch.Add(o => o.AddRoleId, roleIds[3]);
            var addRoleResult = await testGroupController.PatchGroup(groupId, addRolePatch);
            #endregion

            var applicationareaId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea()
                { ApplicationAreaId = applicationareaId, ApplicationAreaName = $"applicationarea::{applicationareaId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            //Create some resources
            var Resources = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Resources.Add(guidString);
                _databaseFixture.ACContext.Resource.Add(new AccessControl.Models.Resource
                    { ResourceId = guidString, ResourceName = $"name:{guidString}",ApplicationAreaId = applicationareaId});
            }

            //create some actions
            var actions = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                actions.Add(guidString);
                _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                    { ActionId = guidString, ActionName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            #region patch actions to resource
            //patch actions to role
            var testController = new ResourcesController(_databaseFixture.ACContext);
            var patch = new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, actions[0]);
            patch.Add(o => o.ActionId, actions[1]);
            await testController.PatchResource(Resources[0], patch);
            await testController.PatchResource(Resources[1], patch);
            await testController.PatchResource(Resources[2], patch);

            patch = new JsonPatchDocument<ResourcePatch>();
            patch.Add(o => o.ActionId, actions[2]);
            patch.Add(o => o.ActionId, actions[3]);
            await testController.PatchResource(Resources[2], patch);
            await testController.PatchResource(Resources[3], patch);
            await testController.PatchResource(Resources[4], patch);
            #endregion

            var permissionsList = _databaseFixture.ACContext.Permission;

            #region Add permissions to roles
            //add permissions to roles...
            var rolesList = _databaseFixture.ACContext.Role;
            var resourceAction = await _databaseFixture.ACContext.Resourceaction.ToListAsync();
            var resourceActionCount = resourceAction.Count;
            var random=new Random();
            foreach (var role in rolesList)
            {
                var resouceActionIds = new List<string>();
                while (resouceActionIds.Count < 4)
                {
                    var randomIndex = random.Next(0, resourceActionCount - 1);
                    var resourceActionId = resourceAction[randomIndex].ResourceActionId;
                    if(!resouceActionIds.Contains(resourceActionId)) resouceActionIds.Add(resourceActionId);
                }

                foreach (var resourceActionId in resouceActionIds)
                {
                    var deny = Convert.ToByte(random.Next(10) > 7);
                    _databaseFixture.ACContext.Permission.Add(new Permission{Deny = deny,PermissionId = Guid.NewGuid().ToString(),ResourceActionId = resourceActionId,RoleId = role.RoleId});
                }

            }

            _databaseFixture.ACContext.SaveChanges();
            #endregion

            //now to check
            var testUserController=new UsersController(_databaseFixture.ACContext, _permissionFixture.PermissionCheck);
            var permissions=await testUserController.GetUserPermission(subjectIds[0]);

            var permissionResult = permissions.Result as OkObjectResult;
            var permissionResponse = permissionResult.Value as PermissionResponse;
            //now got to verify output
            Assert.NotNull(permissionResponse);
            var first = permissionResponse.Permission.FirstOrDefault();

            var check = testUserController.CheckUserPermission(subjectIds[0], first.ResourceName, first.ActionName);
            var checkResult = check.Result.Result as OkObjectResult;
            Assert.True(Convert.ToBoolean( ( checkResult.Value as PermissionCheckResult).Allow));
        }
    }
}