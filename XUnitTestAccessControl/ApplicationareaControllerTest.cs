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

namespace XUnitTestAccessControl
{

    public class ApplicationareaControllerTest : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public ApplicationareaControllerTest(DatabaseFixture dbFixture)
        {
            _databaseFixture = dbFixture;
        }

        [Fact]
        public async Task TestPost()
        {
            var testController = new ApplicationAreasController(_databaseFixture.ACContext);
            var result = await testController.CreateApplicationArea(new ApplicationAreaDTO { ApplicationAreaName =  "TestApplicationarea" });
            Assert.NotNull(result);
            var resultValue = (result.Result as CreatedAtActionResult).Value as ApplicationAreaResponse;

            var applicationareaId = resultValue.ApplicationAreaId;
            var aoolicationAreaRecord = await _databaseFixture.ACContext.Applicationarea.FindAsync(applicationareaId);
            Assert.Equal(aoolicationAreaRecord.ApplicationAreaId, applicationareaId);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await testController.CreateApplicationArea(new ApplicationAreaDTO() { ApplicationAreaName = "TestApplicationArea" }));

        }

        [Fact]
        public async Task TaskGetAll()
        {
            var applicationAreas = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                applicationAreas.Add(guidString);
                _databaseFixture.ACContext.Applicationarea.Add(new AccessControl.Models.Applicationarea
                { ApplicationAreaId = guidString, ApplicationAreaName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();

            applicationAreas = await _databaseFixture.ACContext.Applicationarea.Select(o => o.ApplicationAreaId).ToListAsync();

            var testController = new ApplicationAreasController(_databaseFixture.ACContext);
            var result = await testController.GetApplicationAreas();
            var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            var testValue = (resultResult.Value as IEnumerable<ApplicationAreaResponse>).ToList();
            foreach (var applicationAreaId in applicationAreas)
            {
                Assert.Contains(testValue, o => o.ApplicationAreaId == applicationAreaId);
            }
        }

        [Fact]
        public async Task Get()
        {
            var applicationAreas = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var guidString = Guid.NewGuid().ToString();
                applicationAreas.Add(guidString);
                _databaseFixture.ACContext.Applicationarea.Add(new Applicationarea
                { ApplicationAreaId = guidString, ApplicationAreaName = $"name:{guidString}" });
            }
            await _databaseFixture.ACContext.SaveChangesAsync();
            var testController = new ApplicationAreasController(_databaseFixture.ACContext);
            for (var i = 0; i < 10; i++)
            {
                var result = await testController.GetApplicationArea(applicationAreas[i]);
                var resultResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
                var resultId = (resultResult.Value as ApplicationAreaResponse).ApplicationAreaId;
                Assert.Equal(applicationAreas[i], resultId);
            }

            var result404 = await testController.GetApplicationArea(Guid.NewGuid().ToString());
            Assert.NotNull(result404.Result as NotFoundResult);
        }
    }
}