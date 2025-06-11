using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Million.Api.Controllers;
using Million.Application.DTOs;
using Million.Application.Services;
using Moq;
using NUnit.Framework;

namespace Million.Tests.Controllers
{
    [TestFixture]
    public class OwnersControllerTests
    {
        private Mock<OwnerService> _serviceMock;
        private OwnersController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<OwnerService>();
            _controller = new OwnersController(_serviceMock.Object);
        }

        [Test]
        public async Task Get_ReturnsOkWithResult()
        {
            var expected = new List<OwnerDto> { new OwnerDto { Name = "Owner1" } };
            _serviceMock.Setup(s => s.GetFilteredAsync("John")).ReturnsAsync(expected);

            var result = await _controller.Get("John") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(expected, result.Value);
        }

        [Test]
        public async Task Post_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var dto = new OwnerDto();

            var result = await _controller.Post(dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Post_ValidModel_ReturnsOk()
        {
            var dto = new OwnerDto { Name = "Owner1" };
            _serviceMock.Setup(s => s.AddAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Post(dto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(dto, result.Value);
        }

        [Test]
        public async Task Put_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var dto = new OwnerDto();

            var result = await _controller.Put("1", dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Put_ValidModel_ReturnsOk()
        {
            var dto = new OwnerDto { Name = "Owner1" };
            _serviceMock.Setup(s => s.UpdateAsync("1", dto)).Returns(Task.CompletedTask);

            var result = await _controller.Put("1", dto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(dto, result.Value);
        }
    }
}
