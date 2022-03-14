using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using _7220_Media.Logic.PictureAlbumLogic;
using _7220_Media.Model.PictureAlbum;
using _7220_Media.Controllers.PictureAlbumPicture;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace _7220_Media_Test
{
    [TestClass]
    public class PictureAlbumPictureApiControllerTests
    {
        private readonly Mock<IPictureAlbumLogic> mockLogic;
        private readonly PictureAlbumPictureController controller;

        public PictureAlbumPictureApiControllerTests()
        {
            mockLogic = new Mock<IPictureAlbumLogic>();
            var mockLogger = new Mock<ILogger<PictureAlbumPictureController>>();
            controller = new PictureAlbumPictureController(mockLogger.Object, mockLogic.Object);
        }

        private static ICollection<PictureAlbumPictureSelectedColumnModel> GetMockPictureAlbumPictureModels()
        {
            var pictureAlbumPictureModels = new List<PictureAlbumPictureSelectedColumnModel>
            {
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 1, PictureAlbumId = 1, PictureId = 1},
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 2, PictureAlbumId = 2, PictureId = 2},
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 3, PictureAlbumId = 3, PictureId = 3},
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 4, PictureAlbumId = 4, PictureId = 4}
            };

            return pictureAlbumPictureModels;
        }        
        
        [TestMethod]
        public async Task GetAllPictureAlbumPictures_ReturnsOkAndAllPictureAlbumPictures_IfDatabaseHasRecords()
        {
            // Arrange
            var pictureAlbumPictureModels = GetMockPictureAlbumPictureModels();

            mockLogic
                .Setup(e => e.GetAllPictureAlbumPicturesAsync())
                .ReturnsAsync(pictureAlbumPictureModels);

            // Act
            var actionResult = await controller.GetAllPictureAlbumPictures() as OkObjectResult;

            // Assert

            //results should not null and the count should match up
            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PictureAlbumPictureSelectedColumnModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pictureAlbumPictureModels.Count);
        }

        [TestMethod]
        public async Task GetAllPictureAlbumPictures_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            //Arrange

            //Act
            var actionResult = await controller.GetAllPictureAlbumPictures();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllPictureAlbumPictures_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            //Arrange
            mockLogic
                .Setup(l => l.GetAllPictureAlbumPicturesAsync())
                .ThrowsAsync(new Exception("Error"));

            //Act
            var actionResult = await controller.GetAllPictureAlbumPictures() as ObjectResult;

            //Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateManyPictureAlbumPictures_ReturnsStatusCode201_IfInputIsValid()
        {
            //Arrange
            var mockInput = new CreateManyPictureAlbumPicturesModel {
                PictureAlbumId = 1, PictureIds = new List<int>() {
                    1, 2, 3
                }
            };
            var mockResult = new List<PictureAlbumPictureSelectedColumnModel>() {
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 0, PictureAlbumId = 1, PictureId = 1},
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 1, PictureAlbumId = 1, PictureId = 2},
                new PictureAlbumPictureSelectedColumnModel { PictureAlbumPictureId = 2, PictureAlbumId = 1, PictureId = 3}
            };
            mockLogic
                .Setup(l => l.CreateManyPictureAlbumPicturesAsync(mockInput))
                .Returns(System.Threading.Tasks.Task.FromResult(mockInput));
            
            //Act
            var actionResult = await controller.CreateManyPictureAlbumPictures(mockInput) as ObjectResult;
            var Value = actionResult.Value as CreateManyPictureAlbumPicturesModel;

            //Assert
            Assert.AreEqual(201, actionResult.StatusCode);
            Assert.IsNotNull(Value);    
            CollectionAssert.AreEqual(mockInput.PictureIds, Value.PictureIds);
            Assert.AreNotEqual(0, Value.PictureAlbumId);
            Assert.IsFalse(Value.PictureIds.Any(p => p < 1));
            
        }

        [TestMethod]
        public async Task CreateManyPictureAlbumPictures_ReturnsBadRequest_IfInputIdIsNull()
        {
            //Arrange

            //Act

            //Pass in null
            var actionResult = await controller.CreateManyPictureAlbumPictures(null);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }    

        [TestMethod]
        public async Task CreateManyPictureAlbumPictures_IfInputIdIsNot0()
        {
            //Arrange

            //Act

            //Id can't be 0
            var mockInput = new CreateManyPictureAlbumPicturesModel {
                PictureAlbumId = 0, PictureIds = new List<int>() {
                    1, 2, 3
                }
            };

            var actionResult = await controller.CreateManyPictureAlbumPictures(mockInput);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateManyPictureAlbumPictures_ReturnsBadRequest_IfInputModelStateIsBad()
        {
            //Arrange

            //Act

            //In the model PictureIds can't be empty
            var mockInput = new CreateManyPictureAlbumPicturesModel {
                PictureAlbumId = 1, PictureIds = new List<int>()
            };
            controller.ModelState.AddModelError("PictureIds", "Minimum of one id is Required");
            var actionResult = await controller.CreateManyPictureAlbumPictures(mockInput);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));

        }  

        [TestMethod]
        public async Task CreateManyPictureAlbumPictures_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            //Arrange
            mockLogic
                .Setup(l => l.CreateManyPictureAlbumPicturesAsync(It.IsAny<CreateManyPictureAlbumPicturesModel>()))
                .Throws(new Exception("Error"));
            var createManyPictureAlbumPicturesModel = new CreateManyPictureAlbumPicturesModel { 
                PictureAlbumId = 1, PictureIds = new List<int> {
                    1, 2, 3
                }
            };

            //Act
            var actionResult = await controller.CreateManyPictureAlbumPictures(createManyPictureAlbumPicturesModel) as ObjectResult;

            //Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }  
    }
}