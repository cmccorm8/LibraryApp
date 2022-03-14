using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using _7220_Media.Logic.PictureAlbumLogic;
using _7220_Media.Model.PictureAlbum;
using _7220_Media.Model.Picture;
using _7220_Media.Controllers.PictureAlbum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace _7220_Media_Test
{
    [TestClass]
    public class PictureAlbumApiControllerTests
    {
        private readonly Mock<IPictureAlbumLogic> mockLogic;
        private readonly PictureAlbumController controller;

        public PictureAlbumApiControllerTests()
        {
            mockLogic = new Mock<IPictureAlbumLogic>();
            var mockLogger = new Mock<ILogger<PictureAlbumController>>();
            controller = new PictureAlbumController(mockLogger.Object, mockLogic.Object);
        }

        [TestMethod]
        public async Task GetAllPictureAlbums_ReturnsOkAndAllPictureAlbums_IfDatabaseHasRecords()
        {
            // Arrange
            var pictureAlbumModels = GetMockPictureAlbumModels();

            mockLogic
                .Setup(e => e.GetAllPictureAlbums())
                .ReturnsAsync(pictureAlbumModels);

            // Act
            var actionResult = await controller.GetAllPictureAlbums() as OkObjectResult;

            // Assert

            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PictureAlbumSelectedColumnModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pictureAlbumModels.Count);
        }

        [TestMethod]
        public async Task GetAllPictureAlbums_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            //Arrange

            //Act
            var actionResult = await controller.GetAllPictureAlbums();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllPictureAlbums_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            //Arrange
            mockLogic
                .Setup(l => l.GetAllPictureAlbums())
                .ThrowsAsync(new Exception("Error"));

            //Act
            var actionResult = await controller.GetAllPictureAlbums() as ObjectResult;

            //Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPictureAlbum_ReturnsOkAndPictureAlbum_IfDatabaseIdIsValid()
        {
            //Arrange
            var id = 42;
            mockLogic
                .Setup(e => e.GetPictureAlbumAsync(id))
                .ReturnsAsync(new PictureAlbumSelectedColumnModel { PictureAlbumId = id});

            //Act

            var actionResult = await controller.GetPictureAlbum(id) as OkObjectResult;

            //Assert

            var value = actionResult.Value as PictureAlbumSelectedColumnModel;
            Assert.IsNotNull(value);

            Assert.AreEqual(id, value.PictureAlbumId);
        }

        [TestMethod]
        public async Task GetPictureAlbum_ReturnsNoContent_IfIdDoesNotExist()
        {
            //Arrange

            //Act
            var actionResult = await controller.GetPictureAlbum(-1);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPictureAlbumsByIds_ReturnsOkAndSomePictureAlbums_IfIdsAreValid()
        {
            // Arrange
            var pictureAlbumModels = GetMockPictureAlbumModels() as List<PictureAlbumSelectedColumnModel>;

            pictureAlbumModels.RemoveAt(0);

            var pictureAlbumModelMockIds = pictureAlbumModels.Select(e => e.PictureAlbumId).ToArray();

            mockLogic
                .Setup(l => l.GetManyPictureAlbumsAsync(pictureAlbumModelMockIds))
                .ReturnsAsync(pictureAlbumModels);

            // Act
            var actionResult = await controller.GetPictureAlbumsByIds(new [] { 2, 3, 4 }) as OkObjectResult;
            var values = actionResult.Value as List<PictureAlbumSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(values);

            Assert.AreEqual(pictureAlbumModels.Count, values.Count);
            Assert.AreEqual(pictureAlbumModels[0].PictureAlbumId, values[0].PictureAlbumId);
            Assert.AreEqual(pictureAlbumModels[1].PictureAlbumId, values[1].PictureAlbumId);
            Assert.AreEqual(pictureAlbumModels[2].PictureAlbumId, values[2].PictureAlbumId);
        }

        [TestMethod]
        public async Task GetPictureAlbumsByIds_ReturnsOkAndOnlyValidPictureAlbums_IfSomeIdsAreValid()
        {
            // Arrange
            var pictureAlbumModels = GetMockPictureAlbumModels() as List<PictureAlbumSelectedColumnModel>;

            pictureAlbumModels.RemoveAt(0);

            var pictureAlbumModelMockIds = pictureAlbumModels.Select(e => e.PictureAlbumId).ToArray();

            mockLogic
                .Setup(l => l.GetManyPictureAlbumsAsync(It.Is<int[]>(e => e.Intersect(pictureAlbumModelMockIds).Any())))
                .ReturnsAsync(pictureAlbumModels);

            // Act
            var actionResult = await controller.GetPictureAlbumsByIds(new [] { 2, 3, 4, 0, -7 }) as OkObjectResult;
            var values = actionResult.Value as List<PictureAlbumSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(values);

            //Make sure the counts AND the individual entries are the same
            Assert.AreEqual(pictureAlbumModels.Count, values.Count);
            Assert.AreEqual(pictureAlbumModels[0].PictureAlbumId, values[0].PictureAlbumId);
            Assert.AreEqual(pictureAlbumModels[1].PictureAlbumId, values[1].PictureAlbumId);
            Assert.AreEqual(pictureAlbumModels[2].PictureAlbumId, values[2].PictureAlbumId);
        }

        [TestMethod]
        public async Task GetPictureAlbumsByIds_ReturnsNoContent_IfAllIdsAreInvalid()
        {
            // Arrange

            var idsThatShouldNotExist = new int[] { 0, -1, -8 };

            // Act
            var actionResult = await controller.GetPictureAlbumsByIds(idsThatShouldNotExist);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPictureAlbum_ReturnsBadRequest_IfIdIs0()
        {
            //Arrange
            

            //Act
            var actionResult = await controller.GetPictureAlbum(0);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }
        [TestMethod]
        public async Task GetPictureAlbumsByIds_ReturnsBadRequest_IfIdsAreEmpty()
        {
            // Arrange

            var emptyIntList = new int[0];

            // Act
            var actionResult = await controller.GetPictureAlbumsByIds(emptyIntList);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPictureAlbum_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            //Arrange
            mockLogic
                .Setup(l => l.GetPictureAlbumAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            //Act
            var actionResult = await controller.GetPictureAlbum(1) as ObjectResult;

            //Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
        [TestMethod]
        public async Task GetPictureAlbumsByIds_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetManyPictureAlbumsAsync(It.IsAny<int[]>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPictureAlbumsByIds(new[] { 1, 2, 3 }) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        private static ICollection<PictureAlbumSelectedColumnModel> GetMockPictureAlbumModels()
        {
            var pictureAlbumModels = new List<PictureAlbumSelectedColumnModel>
            {
                new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Example Name 1"},
                new PictureAlbumSelectedColumnModel { PictureAlbumId = 2, Name = "Example Name 2"},
                new PictureAlbumSelectedColumnModel { PictureAlbumId = 3, Name = "Example Name 3"},
                new PictureAlbumSelectedColumnModel { PictureAlbumId = 4, Name = "Example Name 4"}
            };

            return pictureAlbumModels;

        }

        [TestMethod]
        public async Task DeletePictureAlbum_ReturnsBadRequest_IfInputIs0()
        {
            // Arrange

            // Act
            var actionResult = await controller.DeletePictureAlbum(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }
         [TestMethod]
        public async Task CreatePictureAlbum_ReturnsCreatedPictureAlbum_IfInputIsValid()
        {
            // Arrange
            var mockInput = new PictureAlbumSelectedColumnModel { Name = "Test Picture Album"};
            var mockResult = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Test Picture Album" };
            mockLogic
                .Setup(l => l.CreatePictureAlbumAsync(mockInput))
                .ReturnsAsync(mockResult as PictureAlbumSelectedColumnModel);

            // Act
            var actionResult =
                await controller.CreatePictureAlbum(mockInput) as ObjectResult;
            var value = actionResult.Value as PictureAlbumSelectedColumnModel;

            // Assert
            Assert.AreEqual(201, actionResult.StatusCode);
            Assert.IsNotNull(value);
            Assert.AreNotEqual(0, value.PictureAlbumId);
            Assert.AreEqual(mockInput.Name, value.Name);
        }

        [TestMethod]
        public async Task CreatePictureAlbum_ReturnsBadRequest_IfInputIsNull()
        {

            
            // Arrange

            // Act
            var actionResult = await controller.CreatePictureAlbum(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task DeletePictureAlbum_ReturnsOk_IfIdExists()
        {
            // Arrange
            var mockId = 1;

            // Act
            var actionResult = await controller.DeletePictureAlbum(mockId) as OkResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeletePictureAlbum_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            // Arrange
            mockLogic
                .Setup(l => l.DeletePictureAlbumAsync(It.IsAny<int>()))
                .Throws(new Exception("Error"));
            
            // Act
            var actionResult = await controller.DeletePictureAlbum(1) as ObjectResult;
            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
        [TestMethod]
        public async Task CreatePictureAlbum_ReturnsBadRequest_IfInputIdIsNot0()
        {

            // Arrange

            // Act
            var mockInput = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Test Picture Album"};
            var actionResult = await controller.CreatePictureAlbum(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreatePictureAlbum_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act
            var mockInput = new PictureAlbumSelectedColumnModel { PictureAlbumId = 0};
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.CreatePictureAlbum(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreatePictureAlbum_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.CreatePictureAlbumAsync(It.IsAny<PictureAlbumSelectedColumnModel>()))
                .Throws(new Exception("Error"));
            var pictureAlbumSelectedColumnModel = new PictureAlbumSelectedColumnModel { PictureAlbumId = 0, Name = "Test Picture Album"};

            // Act
            var actionResult = await controller.CreatePictureAlbum(pictureAlbumSelectedColumnModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdatePictureAlbum_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act
            var actionResult = await controller.UpdatePictureAlbum(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePictureAlbum_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act
            var mockInput = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1 };
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.UpdatePictureAlbum(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePictureAlbum_ReturnsOk_IfInputIdExistsAndInputIsValid()
        {

            // Arrange
            var mockInput = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Test Picture Album" };
            var mockResult = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Test Picture Album" };
            mockLogic
                .Setup(l => l.UpdatePictureAlbumAsync(mockInput))
                .ReturnsAsync(mockResult as PictureAlbumSelectedColumnModel);

            // Act
            var actionResult = await controller.UpdatePictureAlbum(mockInput) as OkObjectResult;
            var value = actionResult.Value as PictureAlbumSelectedColumnModel;

            // Assert
            // All values should match
            Assert.AreEqual(mockInput.PictureAlbumId, value.PictureAlbumId);
            Assert.AreEqual(mockInput.Name, value.Name);
        }

        [TestMethod]
        public async Task UpdatePictureAlbum_ReturnsNotFound_IfInputDoesNotExistsButInputIsValid()
        {

            // Arrange
            var mockInput = new PictureAlbumSelectedColumnModel { PictureAlbumId = -1, Name = "Test Picture Album" };
            mockLogic
                .Setup(l => l.UpdatePictureAlbumAsync(mockInput))
                .Returns(Task.FromResult<PictureAlbumSelectedColumnModel>(null));

            // Act
            var actionResult = await controller.UpdatePictureAlbum(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdatePictureAlbum_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.UpdatePictureAlbumAsync(It.IsAny<PictureAlbumSelectedColumnModel>()))
                .Throws(new Exception("Error"));
            var pictureAlbumSelectedColumnModel = new PictureAlbumSelectedColumnModel { PictureAlbumId = 1, Name = "Test Picture Album" };

            // Act
            var actionResult = await controller.UpdatePictureAlbum(pictureAlbumSelectedColumnModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPictureAlbumPicturesByPictureAlbumId_ReturnsOkAndOnlyValidPictureAlbumPictures_IfIdIsValid()
        {

            // Arrange
            var pictureModels = new List<PictureSelectedColumnModel>{ new PictureSelectedColumnModel
            { PictureId = 1, Name = "Sunset"}, new PictureSelectedColumnModel { PictureId = 3, Name = "Elephants"}};

            int albumId = 1;

            mockLogic.Setup(l => l.GetPicturesByAlbumIdAsync(albumId)).ReturnsAsync(pictureModels);

            // Act
            var actionResult = await controller.GetPicturesByAlbumId(albumId) as OkObjectResult;
            var values = actionResult.Value as List<PictureSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(values);

            Assert.AreEqual(pictureModels[0], values[0]);
            Assert.AreEqual(pictureModels[1], values[1]);
        }

        [TestMethod]
        public async Task GetPictureAlbumPicturesByPictureAlbumId_ReturnsNoContent_IfIdDoesNotExistInDatabase()
        {
            // Arrange
            var idThatShouldNotExist = 4;
            // Act
            var actionResult = await controller.GetPicturesByAlbumId(idThatShouldNotExist);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetPictureAlbumPicturesByPictureAlbumId_ReturnsBadRequest_IfIdIsLessThan1()
        {
            //Act
            var actionResult = await controller.GetPicturesByAlbumId(0);

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }


        [TestMethod]
        public async Task GetPictureAlbumPicturesByPictureAlbumId_ReturnsInternalServiceError_IfErrorIsThrown()
        {
            //Arrange
            mockLogic
                .Setup(l => l.GetPicturesByAlbumIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            //Act
            var actionResult = await controller.GetPicturesByAlbumId(1) as ObjectResult;

            //Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
    }
}