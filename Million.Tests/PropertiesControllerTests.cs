using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Million.Api.Controllers;
using Million.Application.DTOs;
using Million.Application.Services;
using Moq;
using NUnit.Framework;

namespace Million.Tests.Controllers
{
    [TestFixture]
    public class PropertiesControllerTests
    {
        private Mock<PropertyService> _serviceMock;
        private PropertiesController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<PropertyService>(null, null);
            _controller = new PropertiesController(_serviceMock.Object);
        }

        [Test]
        public async Task Get_ReturnsOkWithResult()
        {
            var expected = new List<PropertyDto> { new PropertyDto { Name = "Test" } };
            _serviceMock.Setup(s => s.GetFilteredAsync("n", "a", 1, 2)).ReturnsAsync(expected);

            var result = await _controller.Get("n", "a", 1, 2) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(expected, result.Value);
        }

        [Test]
        public async Task Post_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var dto = new PropertyDto();

            var result = await _controller.Post(dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Post_ValidModel_ReturnsOk()
        {
            var dto = new PropertyDto { Name = "Test" };
            _serviceMock.Setup(s => s.AddAsync(dto, It.IsAny<string>())).Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await _controller.Post(dto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(dto, result.Value);
        }

        [Test]
        public async Task GetPropertyNames_ReturnsDistinctNames()
        {
            var names = new List<string> { "A", "B", "A" };
            _serviceMock.Setup(s => s.GetAllPropertyNamesAsync()).ReturnsAsync(names);

            var result = await _controller.GetPropertyNames() as OkObjectResult;

            Assert.IsNotNull(result);
            var returnedNames = ((IEnumerable<string>)result.Value).ToList();
            Assert.AreEqual(2, returnedNames.Count);
            Assert.Contains("A", returnedNames);
            Assert.Contains("B", returnedNames);
        }

        [Test]
        public async Task Create_ReturnsOkWithMessage()
        {
            var dto = new PropertyTraceDto { IdProperty = "1", Name = "Test", DateSale = DateTime.Now, Value = 100, Tax = 10 };
            _serviceMock.Setup(s => s.CreateAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Create(dto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(((dynamic)result.Value).message.ToString().Contains("Trazabilidad registrada"));
        }

        [Test]
        public async Task GetAll_ReturnsOkWithTraces()
        {
            var traces = new List<PropertyTrace> { new PropertyTrace { Id = "1", Name = "Trace" } };
            _serviceMock.Setup(s => s.GetAllAsync("1")).ReturnsAsync(traces);

            var result = await _controller.GetAll("1") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(traces, result.Value);
        }
    }
}
