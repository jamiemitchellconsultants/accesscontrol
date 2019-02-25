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

    public class GroupControllerTest : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public GroupControllerTest(DatabaseFixture dbFixture)
        {
            _databaseFixture = dbFixture;
        }

        [Fact]
        public async Task TestPost()
        {
            var testController = new GroupsController(_databaseFixture.ACContext);
            var result = await testController.PostGroup(new GroupPost { GroupName = "TestGroup" });
            Assert.NotNull(result);
            var resultValue = (result.Result as CreatedAtActionResult).Value as GroupResponse;

            var GroupId = resultValue.GroupId;
            var GroupRecord = await _databaseFixture.ACContext.Group.FindAsync(GroupId);
            Assert.Equal(GroupRecord.GroupId, GroupId);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testController.PostGroup(new GroupPost() { GroupName = "TestGroup" }));

        }

        [Fact]
        public async Task TaskGetAll()
        {
            var Groups = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Groups.Add(guidString);
                _databaseFixture.ACContext.Group.Add(new AccessControl.Models.Group
                { GroupId = guidString, GroupName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new GroupsController(_databaseFixture.ACContext);
            var result = await testController.GetGroup();


            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            var testValue = (resultResult.Value as IEnumerable<GroupResponse>).ToList();
            for (var i = 0; i < 10; i++)
            {
                Assert.Contains(testValue, o => o.GroupId == Groups[i]);
            }
        }

        [Fact]
        public async Task Get()
        {
            var Groups = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                Groups.Add(guidString);
                _databaseFixture.ACContext.Group.Add(new AccessControl.Models.Group
                { GroupId = guidString, GroupName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new GroupsController(_databaseFixture.ACContext);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetGroup(Groups[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
                var resultId = (resultResult.Value as GroupResponse).GroupId;
                Assert.Equal(Groups[i], resultId);
            }

        }

        [Fact]
        async Task PatchTestUser()
        {
            var userIds = new List<string>();
            var subjectIds = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                var userId = Guid.NewGuid().ToString();
                userIds.Add(userId);
                var subjectId = Guid.NewGuid().ToString();
                subjectIds.Add(subjectId);
                _databaseFixture.ACContext.User.Add(new User
                    {UserId = userId, SubjectId = subjectId, LocalName = $"{userId}::{subjectId}"});
            }

            await _databaseFixture.ACContext.SaveChangesAsync();
            var groupId=Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Group.Add(new Group {GroupId = groupId, GroupName = $"name::{groupId}"});
            await _databaseFixture.ACContext.SaveChangesAsync();

            var testGroupController=new GroupsController(_databaseFixture.ACContext);
            var addUserPatch=new JsonPatchDocument<GroupPatch>();
            addUserPatch.Add(o => o.AddUserId, new[] {userIds[0], userIds[1], userIds[2]});
            addUserPatch.Add(o => o.AddUserId, userIds[3]);
            var addUserResponse =await  testGroupController.PatchGroup(groupId, addUserPatch);
            var userGroup=await _databaseFixture.ACContext.Usergroup.Where(o => o.GroupId == groupId).Select(o=>o.UserId).ToListAsync();
            Assert.Contains(userIds[0], userGroup);
            var remmoveUser=new JsonPatchDocument<GroupPatch>();
            remmoveUser.Add(o => o.RemoveUserId, new[] {userIds[0]});
            remmoveUser.Add(o => o.RemoveUserId, userIds[1]);
            var removeUserResp = await testGroupController.PatchGroup(groupId, remmoveUser);
            userGroup = await _databaseFixture.ACContext.Usergroup.Where(o => o.GroupId == groupId).Select(o => o.UserId).ToListAsync();
            Assert.DoesNotContain(userIds[0], userGroup);

            var itemsInGroup = await testGroupController.GetGroupUsers(groupId);
            var iigResult = itemsInGroup.Result as OkObjectResult;
            var iigEnumerable = iigResult.Value as IEnumerable<UserResponse>;
            Assert.Contains(userIds[2], iigEnumerable.Select(o => o.UserId));


        }
        [Fact]
        async Task PatchTestRole()
        {
            var roleIds = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                var roleId = Guid.NewGuid().ToString();
                roleIds.Add(roleId);
                _databaseFixture.ACContext.Role.Add(new Role
                    { RoleId = roleId, RoleName = $"role::{roleId}" });
            }

            await _databaseFixture.ACContext.SaveChangesAsync();
            var groupId = Guid.NewGuid().ToString();
            _databaseFixture.ACContext.Group.Add(new Group { GroupId = groupId, GroupName = $"name::{groupId}" });
            await _databaseFixture.ACContext.SaveChangesAsync();

            var testGroupController = new GroupsController(_databaseFixture.ACContext);
            var addRolePatch = new JsonPatchDocument<GroupPatch>();
            addRolePatch.Add(o => o.AddRoleId, new[] { roleIds[0], roleIds[1], roleIds[2] });
            addRolePatch.Add(o => o.AddRoleId, roleIds[3]);
            var addRoleResult = await testGroupController.PatchGroup(groupId, addRolePatch);
            var roleGroup = await _databaseFixture.ACContext.Grouprole.Where(o => o.GroupId == groupId).Select(o => o.RoleId).ToListAsync();
            Assert.Contains(roleIds[0], roleGroup);
            var removeRole = new JsonPatchDocument<GroupPatch>();
            removeRole.Add(o => o.RemoveRoleId, new[] { roleIds[0] });
            removeRole.Add(o => o.RemoveRoleId, roleIds[1]);
            var removeUserResp = await testGroupController.PatchGroup(groupId, removeRole);
            roleGroup = await _databaseFixture.ACContext.Grouprole.Where(o => o.GroupId == groupId).Select(o => o.RoleId).ToListAsync();
            Assert.DoesNotContain(roleIds[0], roleGroup);

            var itemsInGroup = await testGroupController.GetGroupRoles(groupId);
            var iigResult = itemsInGroup.Result as OkObjectResult;
            var iigEnumerable = iigResult.Value as IEnumerable<RoleResponse>;
            Assert.Contains(roleIds[2], iigEnumerable.Select(o => o.RoleId));

        }
    }
}