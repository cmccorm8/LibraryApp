using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using _7220_Media.Logic.PictureLogic;
using _7220_Media.Model.Picture;
using _7220_Media.Controllers.Picture;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace _7220_Media_Test
{
    [TestClass]
    public class PictureApiControllerTests
    {
        private readonly Mock<IPictureLogic> mockLogic;
        private readonly Mock<IPictureTagLogic> mockTagLogic;
        private readonly PictureController controller;

        public PictureApiControllerTests()
        {
            mockLogic = new Mock<IPictureLogic>();
            mockTagLogic = new Mock<IPictureTagLogic>();
            var mockLogger = new Mock<ILogger<PictureController>>();
            var mockConfig = new Mock<IConfiguration>();
            controller = new PictureController(mockLogger.Object, mockLogic.Object, mockTagLogic.Object, mockConfig.Object);

        }

        [TestMethod]
        public async Task GetAllPictures_ReturnsOkAndAllPictures_IfDatabaseHasRecords()
        {
            // Arrange
            var pictureModels = GetMockPictureSelectedColumnModels();

            mockLogic
                .Setup(e => e.GetAllPictures())
                .ReturnsAsync(pictureModels);

            // Act
            var actionResult = await controller.GetAllPictures() as OkObjectResult;

            // Assert

            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PictureSelectedColumnModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pictureModels.Count);
        }

        [TestMethod]
        public async Task GetAllPictures_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            // Arrange
            
            // Act
            var actionResult = await controller.GetAllPictures();

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllPictures_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetAllPictures())
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetAllPictures() as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }


         [TestMethod]
        public async Task DeletePicture_ReturnsBadRequest_IfInputIdIs0()
        {

            // Arrange

            // Act
            var actionResult = await controller.DeletePicture(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task DeletePicture_ReturnsOk_IfIdExists()
        {

            // Arrange
            var mockId = 1;

            // Act

            var actionResult = await controller.DeletePicture(mockId) as OkResult;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeletePicture_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.DeletePictureAsync(It.IsAny<int>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.DeletePicture(1) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

                [TestMethod]
        public async Task UpdatePicture_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            var actionResult = await controller.UpdatePicture(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePicture_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            var mockInput = new PictureSelectedColumnModel { PictureId = 1 };
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.UpdatePicture(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePicture_ReturnsOk_IfInputIdExistsAndInputIsValid()
        {

            // Arrange
            var mockInput = new PictureSelectedColumnModel { PictureId = 1, Name = "TestName" };
            var mockResult = new PictureSelectedColumnModel { PictureId = 1, Name = "TestName" };
            mockLogic
                .Setup(l => l.UpdatePictureAsync(mockInput))
                .ReturnsAsync(mockResult as PictureSelectedColumnModel );

            // Act

            var actionResult = await controller.UpdatePicture(mockInput) as OkObjectResult;
            var value = actionResult.Value as PictureSelectedColumnModel;

            // Assert
            Assert.AreEqual(mockInput.PictureId, value.PictureId);
            Assert.AreEqual(mockInput.Name, value.Name);
        }

        [TestMethod]
        public async Task UpdatePicture_ReturnsNotFound_IfInputDoesNotExistsButInputIsValid()
        {

            // Arrange
            var mockInput = new PictureSelectedColumnModel { PictureId = -1, Name = "FirstName" };
            mockLogic
                .Setup(l => l.UpdatePictureAsync(mockInput))
                .Returns(Task.FromResult<PictureSelectedColumnModel >(null));

            // Act

            var actionResult = await controller.UpdatePicture(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdatePicture_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.UpdatePictureAsync(It.IsAny<PictureSelectedColumnModel >()))
                .Throws(new Exception("Error"));
            var pictureSelectedColumnModel = new PictureSelectedColumnModel { PictureId = 1, Name = "Firstname" };

            // Act
            var actionResult = await controller.UpdatePicture(pictureSelectedColumnModel ) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
        
        private static ICollection<PictureSelectedColumnModel> GetMockPictureSelectedColumnModels()
        {
            var pokemonTrainerModels = new List<PictureSelectedColumnModel>
            {
                new PictureSelectedColumnModel { PictureId = 1, Name = "Example Name 1"},
                new PictureSelectedColumnModel { PictureId = 2, Name = "Example Name 2"},
                new PictureSelectedColumnModel { PictureId = 3, Name = "Example Name 3"},
                new PictureSelectedColumnModel { PictureId = 4, Name = "Example Name 4"}
            };
            return pokemonTrainerModels;
        }

        [TestMethod]
        public async Task CreateMockPicture_ReturnsCreateMockPicture_IfInputIsValid()
        {
            // Arrange
            var mockInput = new PictureSelectedColumnModel { Name = "Name"};
            var mockResult = new PictureSelectedColumnModel { PictureId = 1, Name = "Name"};
            mockLogic
                .Setup(l => l.CreatePicture(mockInput))
                .ReturnsAsync(mockResult as PictureSelectedColumnModel);

            // Act
            var actionResult =
                await controller.CreatePicture(mockInput) as ObjectResult;
            var value = actionResult.Value as PictureSelectedColumnModel;

            // Assert
            Assert.AreEqual(201, actionResult.StatusCode);
            Assert.IsNotNull(value);
            Assert.AreNotEqual(0, value.PictureId);
            Assert.AreEqual(mockInput.Name, value.Name);
        }

        [TestMethod]
        public async Task CreateMockPicture_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            // just pass in null
            var actionResult = await controller.CreatePicture(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateMockPicture_ReturnsBadRequest_IfInputIdIsNot0()
        {

            // Arrange

            // Act

            var mockInput = new PictureSelectedColumnModel { PictureId = 1, Name = "Name"};
            var actionResult = await controller.CreatePicture(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateMockPicture_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            var mockInput = new PictureSelectedColumnModel { PictureId = 0, Name = "Name"};
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.CreatePicture(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateMockPicture_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.CreatePicture(It.IsAny<PictureSelectedColumnModel>()))
                .Throws(new Exception("Error"));
            var pictureSelectedColumnModel = new PictureSelectedColumnModel { PictureId = 0, Name = "Name"};

            // Act
            var actionResult = await controller.CreatePicture(pictureSelectedColumnModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPicture_ReturnsOkWithTicture_IfIdIsValid()
        {
            // Arrange

            var id = 2;
            mockLogic
                .Setup(e => e.GetPictureAsync(id))
                .ReturnsAsync(new PictureSelectedColumnModel { PictureId = id });

            // Act

            var actionResult = await controller.GetPicture(id) as OkObjectResult;

            // Assert

            var value = actionResult.Value as PictureSelectedColumnModel;
            Assert.IsNotNull(value);

            Assert.AreEqual(id, value.PictureId);
        }

        [TestMethod]
        public async Task GetPicture_ReturnsNotfound_IfIdDoesNotExist()
        {
            // Arrange

            // Act

            var actionResult = await controller.GetPicture(-1);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPicture_ReturnsBadRequest_IfIdIs0()
        {
            // Arrange

            // Act

            var actionResult = await controller.GetPicture(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPicture_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetPictureAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPicture(1) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPicturesByIds_ReturnsOkAndSomePictures_IfIdsAreValid()
        {
            // Arrange
            var pictureModels = GetMockPictureSelectedColumnModels() as List<PictureSelectedColumnModel>;

            pictureModels.RemoveAt(0);

            var pictureModelMockIds = pictureModels.Select(e => e.PictureId).ToArray();

            mockLogic
                .Setup(l => l.GetManyPicturesAsync(pictureModelMockIds))
                .ReturnsAsync(pictureModels);

            // Act
            var actionResult = await controller.GetPictureByIds(new[] { 2, 3, 4 }) as OkObjectResult;
            var values = actionResult.Value as List<PictureSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(values);

            //Make sure the counts AND the individual entries are the same
            Assert.AreEqual(pictureModels.Count, values.Count);
            Assert.AreEqual(pictureModels[0].PictureId, values[0].PictureId);
            Assert.AreEqual(pictureModels[1].PictureId, values[1].PictureId);
            Assert.AreEqual(pictureModels[2].PictureId, values[2].PictureId);
        }

        [TestMethod]
        public async Task GetPicturesByIds_ReturnsOkAndOnlyValidPictures_IfSomeIdsAreValid()
        {
            // Arrange
            var pictureModels = GetMockPictureSelectedColumnModels() as List<PictureSelectedColumnModel>;

            pictureModels.RemoveAt(0);

            var pictureMockIds = pictureModels.Select(e => e.PictureId).ToArray();

            mockLogic
                .Setup(l => l.GetManyPicturesAsync(It.Is<int[]>(e => e.Intersect(pictureMockIds).Any())))
                .ReturnsAsync(pictureModels);

            // Act
            var actionResult = await controller.GetPictureByIds(new[] { 2, 3, 4, 0, -7 }) as OkObjectResult;
            var values = actionResult.Value as List<PictureSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(values);

            Assert.AreEqual(pictureModels.Count, values.Count);
            Assert.AreEqual(pictureModels[0].PictureId, values[0].PictureId);
            Assert.AreEqual(pictureModels[1].PictureId, values[1].PictureId);
            Assert.AreEqual(pictureModels[2].PictureId, values[2].PictureId);
        }

        [TestMethod]
        public async Task GetPictureByIds_ReturnsNoContent_IfAllIdsAreInvalid()
        {
            // Arrange

            var idsThatShouldNotExist = new int[] { 0, -1, -8 };

            // Act
            var actionResult = await controller.GetPictureByIds(idsThatShouldNotExist);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPictureByIds_ReturnsBadRequest_IfIdsAreEmpty()
        {
            // Arrange

            var emptyIntList = new int[0];

            // Act
            var actionResult = await controller.GetPictureByIds(emptyIntList);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPictureByIds_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetManyPicturesAsync(It.IsAny<int[]>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPictureByIds(new[] { 1, 2, 3 }) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPictureTagsByPictureId_ReturnsOkWithPictureTag_IfIdIsValid()
        {
            // Arrange
            var id = 42;
            mockTagLogic
                .Setup(e => e.GetPictureTagsByPictureId(id))
                .ReturnsAsync(new List<PictureTagSelectedColumnModel> {
                    new PictureTagSelectedColumnModel { PictureId = id },
                    new PictureTagSelectedColumnModel { PictureId = id },
                    new PictureTagSelectedColumnModel { PictureId = id }
                });

            // Act
            var actionResult = await controller.GetPictureTagsByPictureId(id) as OkObjectResult;
            var value = actionResult.Value as List<PictureTagSelectedColumnModel>;

            // Assert
            Assert.IsNotNull(value);
            foreach (var t in value)
            {
                Assert.AreEqual(id, t.PictureId);
                Assert.AreNotEqual(0, t.PictureId);
            }
        }

        [TestMethod]
        public async Task GetPictureTagsByPictureId_ReturnsBadRequest_IfInputIs0()
        {
            // Arrange

            // Act

            // picture Id can't be 0
            var actionResult = await controller.GetPictureTagsByPictureId(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPictureTagsByPictureId_ReturnsNoContent_IfNoPictureTags()
        {
            // Arrange
            var id = 42;
            mockTagLogic
                .Setup(e => e.GetPictureTagsByPictureId(id))
                .ReturnsAsync(new List<PictureTagSelectedColumnModel> {});

            // Act
            var actionResult = await controller.GetPictureTagsByPictureId(id) as NoContentResult;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetPictureTagsByPictureId_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockTagLogic
                .Setup(l => l.GetPictureTagsByPictureId(It.IsAny<int>()))
                .Throws(new Exception("Error"));
            int id = 42;

            // Act
            var actionResult = await controller.GetPictureTagsByPictureId(id) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPictureByGuidId_ReturnsOkWithPicture_IfIdIsValid()
        {
            // Arrange

            var guidId = Guid.NewGuid();
            mockLogic
                .Setup(e => e.GetPictureByBlobIdAsync(guidId))
                .ReturnsAsync(new PictureSelectedColumnModel { PictureId = 2 });

            // Act

            var actionResult = await controller.GetPictureByBlobId(guidId) as OkObjectResult;

            // Assert

            var value = actionResult.Value as PictureSelectedColumnModel;
            Assert.IsNotNull(value);

            Assert.AreEqual(2, value.PictureId);
        }

        [TestMethod]
        public async Task GetPictureByGuidId_ReturnsNoContent_IfGuidIdDoesNotExistForAnyPicture()
        {
            // Arrange
            var guidId = Guid.NewGuid();
            mockLogic
                .Setup(e => e.GetPictureByBlobIdAsync(guidId))
                .ReturnsAsync(null as PictureSelectedColumnModel);

            // Act

            var actionResult = await controller.GetPictureByBlobId(guidId);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetPictureByGuidId_ReturnsBadRequest_IfGuidIdIsNull()
        {
            // Arrange
            Guid myGuid = Guid.Empty;

            // Act

            var actionResult = await controller.GetPictureByBlobId(myGuid);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPictureByGuidId_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetPictureByBlobIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPictureByBlobId(Guid.NewGuid()) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateHasLowResolution_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            var actionResult = await controller.UpdateHasLowResolution(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateHasLowResolution_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            var mockInput = new UpdatePictureHasLowResolutionModel { PictureId = -1,};
            var actionResult = await controller.UpdateHasLowResolution(mockInput) as BadRequestResult;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateHasLowResolution_ReturnsOk_IfInputIdExistsAndInputIsValid()
        {

            // Arrange
            var mockInput = new UpdatePictureHasLowResolutionModel { PictureId = 1, HasLowResolution = true };
            var mockResult = new UpdatePictureHasLowResolutionModel { PictureId = 1, HasLowResolution = true };
            mockLogic
                .Setup(l => l.UpdatePictureHasLowResolutionAsync(mockInput))
                .ReturnsAsync(mockResult as UpdatePictureHasLowResolutionModel );

            // Act

            var actionResult = await controller.UpdateHasLowResolution(mockInput) as OkObjectResult;
            var value = actionResult.Value as UpdatePictureHasLowResolutionModel;

            // Assert
            Assert.AreEqual(mockInput.PictureId, value.PictureId);
            Assert.AreEqual(mockInput.HasLowResolution, value.HasLowResolution);
        }

        [TestMethod]
        public async Task UpdateHasLowResolution_ReturnsNotFound_IfInputDoesNotExistsButInputIsValid()
        {

            // Arrange
            var mockInput = new UpdatePictureHasLowResolutionModel { PictureId = 2, HasLowResolution = true, };
            mockLogic
                .Setup(l => l.UpdatePictureHasLowResolutionAsync(mockInput))
                .Returns(Task.FromResult<UpdatePictureHasLowResolutionModel >(null));

            // Act

            var actionResult = await controller.UpdateHasLowResolution(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateHasLowResolution_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.UpdatePictureHasLowResolutionAsync(It.IsAny<UpdatePictureHasLowResolutionModel >()))
                .Throws(new Exception("Error"));
            var updatePictureHasLowResolutionModel = new UpdatePictureHasLowResolutionModel { PictureId = 1, HasLowResolution = true, };

            // Act
            var actionResult = await controller.UpdateHasLowResolution(updatePictureHasLowResolutionModel ) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }
    }
}