using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Controllers;
using AccessControl.Messages;
using AccessControl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Action = System.Action;

namespace XUnitTestAccessControl
{

    public class ActionControllerTest:IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public ActionControllerTest(DatabaseFixture dbFixture)
        {
            _databaseFixture = dbFixture;
        }

        [Fact]
        public async Task TestPost()    
        {
            var testController=new ActionsController(_databaseFixture.ACContext);
            var result= await testController.CreateAction(new ActionDTO {ActionName = "TestAction"})  ;
            Assert.NotNull(result);
            var cont=new ActionContext();
            var resultValue= ( result.Result as CreatedAtActionResult).Value as ActionResponse ;

            var actionId = resultValue.ActionId;
            var actionRecord = await _databaseFixture.ACContext.Action.FindAsync(actionId);
            Assert.Equal(actionRecord.ActionId,actionId);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testController.CreateAction(new ActionDTO() { ActionName = "TestAction" }));

        }

        [Fact]
        public async Task TaskGetAll()
        {
            var actions=new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                actions.Add(guidString);
                _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                    { ActionId = guidString, ActionName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();

            actions=await _databaseFixture.ACContext.Action.Select(o => o.ActionId).ToListAsync();

            var testController = new ActionsController(_databaseFixture.ACContext);
            var result = await testController.GetActions();
            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult) result.Result;
            var testValue =( resultResult.Value as IEnumerable<ActionResponse>).ToList();
            foreach (var actionID in actions)
            {
                Assert.Contains(testValue, o => o.ActionId == actionID);
            }
        }

        [Fact]
        public async  Task Get()
        {
            var actions = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                actions.Add(guidString);
                _databaseFixture.ACContext.Action.Add(new AccessControl.Models.Action
                    { ActionId = guidString, ActionName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ActionsController(_databaseFixture.ACContext);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetAction(actions[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult) result.Result;
                var resultId = (resultResult.Value as ActionResponse).ActionId;
                Assert.Equal(actions[i],resultId);
            }

            var result404 = await testController.GetAction(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);
        }
    }
}