using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using _7220_Media.Logic.PictureLogic;
using _7220_Media.Model.Picture;
using _7220_Media.Controllers.Picture;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace _7220_Media_Test
{
    [TestClass]
    public class PictureTagControllerTests
    {
        private readonly Mock<IPictureTagLogic> mockLogic;
        private readonly PictureTagController controller;
        public PictureTagControllerTests()
        {
            mockLogic = new Mock<IPictureTagLogic>();
            var mockLogger = new Mock<ILogger<PictureTagController>>();
            controller = new PictureTagController(mockLogger.Object, mockLogic.Object);
        }

        
        [TestMethod]
        public async Task GetAllPictureTags_ReturnsOkAndAllPictureTags_IfDatabaseHasRecords()
        {
            // Arrange
            var pictureTagModels = GetMockPictureTagSelectedColumnModels();

            //the logic will return 4 models, a mocked entirety for the database
            mockLogic
                .Setup(e => e.GetAllPictureTags())
                .ReturnsAsync(pictureTagModels);

            // Act
            var actionResult = await controller.GetAllPictureTags() as OkObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PictureTagSelectedColumnModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pictureTagModels.Count);
        }

        [TestMethod]
        public async Task GetAllPictureTags_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            // Arrange
            
            // Act
            var actionResult = await controller.GetAllPictureTags();

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllPictureTags_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetAllPictureTags())
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetAllPictureTags() as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
        private static ICollection<PictureTagSelectedColumnModel> GetMockPictureTagSelectedColumnModels()
        {
            var pictureTagModels = new List<PictureTagSelectedColumnModel>
            {
                new PictureTagSelectedColumnModel { PictureTagId = 1, Name = "Example Name 1"},
                new PictureTagSelectedColumnModel { PictureTagId = 2, Name = "Example Name 2"},
                new PictureTagSelectedColumnModel { PictureTagId = 3, Name = "Example Name 3"},
                new PictureTagSelectedColumnModel { PictureTagId = 4, Name = "Example Name 4"}
            };

            return pictureTagModels;
        }

        [TestMethod]
        public async Task CreateManyPictureTags_ReturnsCreatedPictureTags_IfInputIsValid()
        {
            // Arrange
            var mockInput = new CreateManyPictureTagsModel {
                PictureId = 1, PictureTags = new List<string>() {
                    "Casper, Wyoming", "Laramie, Wyoming", "Cheyenne, Wyoming"
                }
            };
            var mockResult = new List<PictureTagSelectedColumnModel>() { 
                new PictureTagSelectedColumnModel { PictureTagId = 0, PictureId = 1, Name = "Casper, Wyoming" },
                new PictureTagSelectedColumnModel { PictureTagId = 1, PictureId = 1, Name = "Laramie, Wyoming" },
                new PictureTagSelectedColumnModel { PictureTagId = 2, PictureId = 1, Name = "Cheyenne, Wyoming" }
            };
            mockLogic
                .Setup(l => l.CreateManyPictureTagsAsync(mockInput))
                .ReturnsAsync(mockResult as List<PictureTagSelectedColumnModel>);

            // Act
            var actionResult =
                await controller.CreateManyPictureTags(mockInput) as ObjectResult;
            var value = actionResult.Value as List<PictureTagSelectedColumnModel>;

            // Assert
            Assert.AreEqual(201, actionResult.StatusCode);
            Assert.IsNotNull(value);
            foreach (var t in value)
            {                
                Assert.AreNotEqual(0, t.PictureId);   
                Assert.AreEqual(mockInput.PictureId, t.PictureId);
                Assert.IsTrue(mockInput.PictureTags.Contains(t.Name));   
            }                       
        }

        [TestMethod]
        public async Task CreateManyPictureTags_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            // just pass in null
            var actionResult = await controller.CreateManyPictureTags(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateManyPictureTags_ReturnsBadRequest_IfInputIdIsNot0()
        {

            // Arrange

            // Act

            // Id can't be 0
            var mockInput = new CreateManyPictureTagsModel {
                PictureId = 0, PictureTags = new List<string>() {
                    "Casper, Wyoming", "Laramie, Wyoming", "Cheyenne, Wyoming"
                }
            };
            var actionResult = await controller.CreateManyPictureTags(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateManyPictureTags_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            // In the model picture tags cant be empty
            var mockInput = new CreateManyPictureTagsModel {
                PictureId = 0, PictureTags = new List<string>() 
            };
            controller.ModelState.AddModelError("PictureTags", "Minimum of one tag is Required");
            var actionResult = await controller.CreateManyPictureTags(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateManyPictureTags_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.CreateManyPictureTagsAsync(It.IsAny<CreateManyPictureTagsModel>()))
                .Throws(new Exception("Error"));
            var createManyPictureTagsModel = new CreateManyPictureTagsModel {
                PictureId = 1, PictureTags = new List<string>() {
                    "Casper, Wyoming", "Laramie, Wyoming", "Cheyenne, Wyoming"
                }
            };

            // Act
            var actionResult = await controller.CreateManyPictureTags(createManyPictureTagsModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
    }
}