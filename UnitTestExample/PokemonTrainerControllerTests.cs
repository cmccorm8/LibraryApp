using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using _7220_Media.Logic.Pokemon;
using _7220_Media.Model.Pokemon;
using _7220_Media.Controllers.Pokemon;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;



namespace _7220_Media_Test
{
    [TestClass]
    public class PokemonTrainerControllerTests
    {
        private readonly Mock<IPokemonTrainerLogic> mockLogic;
        private readonly PokemonTrainerController controller;

        public PokemonTrainerControllerTests()
        {
            //Here we create a mock example logic to be able to construct our controller to test
            mockLogic = new Mock<IPokemonTrainerLogic>();
            var mockLogger = new Mock<ILogger<PokemonTrainerController>>();
            //construct the controller with the mock logic
            controller = new PokemonTrainerController(mockLogger.Object, mockLogic.Object);

        }
        [TestMethod]
        //Name should be: [Name of the method you are testing]_[What is returned]_[What the condition is]
        public async Task GetPokemonTrainer_ReturnsOkWithTrainer_IfIdIsValid()
        {
            // Arrange

            //specify what the logic SHOULD return in this case
            var id = 42;
            mockLogic
                .Setup(e => e.GetPokemonTrainerAsync(id))
                .ReturnsAsync(new PokemonTrainerModel { Id = id });

            // Act

            //test the method
            var actionResult = await controller.GetPokemonTrainer(id) as OkObjectResult;

            // Assert

            //make sure the result is what we expect (not null, id matches)
            //if any of the Asserts fail, the whole test fails

            var value = actionResult.Value as PokemonTrainerModel;
            Assert.IsNotNull(value);

            //42 because we specified this higher up in this method
            Assert.AreEqual(id, value.Id);
        }

        [TestMethod]
        public async Task GetPokemonTrainer_ReturnsNotfound_IfIdDoesNotExist()
        {
            // Arrange

            // Act

            //because tables start at 1, -1 should not exist
            var actionResult = await controller.GetPokemonTrainer(-1);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPokemonTrainer_ReturnsBadRequest_IfIdIs0()
        {
            // Arrange

            // Act

            // 0 means nothing was passed in, a null integer converts to 0 in C#
            var actionResult = await controller.GetPokemonTrainer(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPokemonTrainer_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetPokemonTrainerAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPokemonTrainer(1) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAllPokemonTrainers_ReturnsOkAndAllTrainers_IfDatabaseHasRecords()
        {
            // Arrange
            var pokemonTrainerModels = GetMockPokemonTrainerModels();

            //the logic will return 4 models, a mocked entirety for the database
            mockLogic
                .Setup(e => e.GetAllPokemonTrainers())
                .ReturnsAsync(pokemonTrainerModels);

            // Act
            var actionResult = await controller.GetAllPokemonTrainers() as OkObjectResult;

            // Assert

            //results should be not null and the count should match up
            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PokemonTrainerModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pokemonTrainerModels.Count);
        }

        [TestMethod]
        public async Task GetAllPokemonTrainers_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            // Arrange
            //don't mock any examples, we should make sure none are returned

            // Act
            var actionResult = await controller.GetAllPokemonTrainers();

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllPokemonTrainers_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetAllPokemonTrainers())
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actionResult = await controller.GetAllPokemonTrainers() as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPokemonTrainersByIds_ReturnsOkAndSomeTrainers_IfIdsAreValid()
        {
            // Arrange
            var pokemonTrainerModels = GetMockPokemonTrainerModels() as List<PokemonTrainerModel>;

            // Remove one from the list, this mock data should only be a subset of our "database"
            pokemonTrainerModels.RemoveAt(0);

            //get just the ids
            var pokemonTrainerModelMockIds = pokemonTrainerModels.Select(e => e.Id).ToArray();

            mockLogic
                .Setup(l => l.GetManyPokemonTrainersAsync(pokemonTrainerModelMockIds))
                .ReturnsAsync(pokemonTrainerModels);

            // Act
            var actionResult = await controller.GetPokemonTrainersByIds(new [] { 2, 3, 4 }) as OkObjectResult;
            var values = actionResult.Value as List<PokemonTrainerModel>;

            // Assert
            Assert.IsNotNull(values);

            //Make sure the counts AND the individual entries are the same
            Assert.AreEqual(pokemonTrainerModels.Count, values.Count);
            Assert.AreEqual(pokemonTrainerModels[0].Id, values[0].Id);
            Assert.AreEqual(pokemonTrainerModels[1].Id, values[1].Id);
            Assert.AreEqual(pokemonTrainerModels[2].Id, values[2].Id);
        }

        [TestMethod]
        public async Task GetPokemonTrainersByIds_ReturnsOkAndOnlyValidTrainers_IfSomeIdsAreValid()
        {
            // Arrange
            var pokemonTrainerModels = GetMockPokemonTrainerModels() as List<PokemonTrainerModel>;

            // Remove one from the list, this mock data should only be a subset of our "database"
            pokemonTrainerModels.RemoveAt(0);

            //get just the ids
            var pokemonTrainerModelMockIds = pokemonTrainerModels.Select(e => e.Id).ToArray();

            mockLogic
                .Setup(l => l.GetManyPokemonTrainersAsync(It.Is<int[]>(e => e.Intersect(pokemonTrainerModelMockIds).Any())))
                .ReturnsAsync(pokemonTrainerModels);

            // Act
            // 0 and -7 are invalid ids
            var actionResult = await controller.GetPokemonTrainersByIds(new [] { 2, 3, 4, 0, -7 }) as OkObjectResult;
            var values = actionResult.Value as List<PokemonTrainerModel>;

            // Assert
            Assert.IsNotNull(values);

            //Make sure the counts AND the individual entries are the same
            Assert.AreEqual(pokemonTrainerModels.Count, values.Count);
            Assert.AreEqual(pokemonTrainerModels[0].Id, values[0].Id);
            Assert.AreEqual(pokemonTrainerModels[1].Id, values[1].Id);
            Assert.AreEqual(pokemonTrainerModels[2].Id, values[2].Id);
        }

        [TestMethod]
        public async Task GetPokemonTrainersByIds_ReturnsNoContent_IfAllIdsAreInvalid()
        {
            // Arrange

            //these three ids should not exist in our database
            var idsThatShouldNotExist = new int[] { 0, -1, -8 };

            // Act
            var actionResult = await controller.GetPokemonTrainersByIds(idsThatShouldNotExist);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPokemonTrainersByIds_ReturnsBadRequest_IfIdsAreEmpty()
        {
            // Arrange

            // empty list
            var emptyIntList = new int[0];

            // Act
            var actionResult = await controller.GetPokemonTrainersByIds(emptyIntList);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPokemonTraineraByIds_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.GetManyPokemonTrainersAsync(It.IsAny<int[]>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.GetPokemonTrainersByIds(new[] { 1, 2, 3 }) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreatePokemonTrainer_ReturnsCreatedTrainer_IfInputIsValid()
        {
            //this one is simple, make sure a successful create returns Ok()

            // Arrange
            var mockInput = new PokemonTrainerModel { Name = "FirstName LastName", NumberOfBadges = 5 };
            var mockResult = new PokemonTrainerModel { Id = 1, Name = "FirstName LastName", NumberOfBadges = 5 };
            mockLogic
                .Setup(l => l.CreatePokemonTrainerAsync(mockInput))
                .ReturnsAsync(mockResult as PokemonTrainerModel);

            // Act
            var actionResult =
                await controller.CreatePokemonTrainer(mockInput) as ObjectResult;
            var value = actionResult.Value as PokemonTrainerModel;

            // Assert
            Assert.AreEqual(201, actionResult.StatusCode);
            Assert.IsNotNull(value);
            Assert.AreNotEqual(0, value.Id);
            Assert.AreEqual(mockInput.Name, value.Name);
            Assert.AreEqual(mockInput.NumberOfBadges, value.NumberOfBadges);
        }

        [TestMethod]
        public async Task CreatePokemonTrainer_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            // just pass in null
            var actionResult = await controller.CreatePokemonTrainer(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreatePokemonTrainer_ReturnsBadRequest_IfInputIdIsNot0()
        {

            // Arrange

            // Act

            // Id can't be 0
            var mockInput = new PokemonTrainerModel { Id = 1, Name = "FirstName LastName", NumberOfBadges = 5 };
            var actionResult = await controller.CreatePokemonTrainer(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreatePokemonTrainer_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            // In the model, Name is required
            var mockInput = new PokemonTrainerModel { Id = 0, NumberOfBadges = 5 };
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.CreatePokemonTrainer(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreatePokemonTrainer_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.CreatePokemonTrainerAsync(It.IsAny<PokemonTrainerModel>()))
                .Throws(new Exception("Error"));
            var pokemonTrainerModel = new PokemonTrainerModel { Id = 0, Name = "Firstname Lastname", NumberOfBadges = 5 };

            // Act
            var actionResult = await controller.CreatePokemonTrainer(pokemonTrainerModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdatePokemonTrainer_ReturnsBadRequest_IfInputIsNull()
        {

            // Arrange

            // Act

            // just pass in null
            var actionResult = await controller.UpdatePokemonTrainer(null);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePokemonTrainer_ReturnsBadRequest_IfInputModelStateIsBad()
        {

            // Arrange

            // Act

            // In the model, Name is required
            var mockInput = new PokemonTrainerModel { Id = 1, NumberOfBadges = 5 };
            controller.ModelState.AddModelError("Name", "Name is Required");
            var actionResult = await controller.UpdatePokemonTrainer(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdatePokemonTrainer_ReturnsOk_IfInputIdExistsAndInputIsValid()
        {

            // Arrange
            var mockInput = new PokemonTrainerModel { Id = 1, Name = "FirstName LastName", NumberOfBadges = 5 };
            var mockResult = new PokemonTrainerModel { Id = 1, Name = "FirstName LastName", NumberOfBadges = 5 };
            mockLogic
                .Setup(l => l.UpdatePokemonTrainerAsync(mockInput))
                .ReturnsAsync(mockResult as PokemonTrainerModel);

            // Act

            // In the model, Name is required
            var actionResult = await controller.UpdatePokemonTrainer(mockInput) as OkObjectResult;
            var value = actionResult.Value as PokemonTrainerModel;

            // Assert
            // All values should match
            Assert.AreEqual(mockInput.Id, value.Id);
            Assert.AreEqual(mockInput.Name, value.Name);
            Assert.AreEqual(mockInput.NumberOfBadges, value.NumberOfBadges);
        }

        [TestMethod]
        public async Task UpdatePokemonTrainer_ReturnsNotFound_IfInputDoesNotExistsButInputIsValid()
        {

            // Arrange
            var mockInput = new PokemonTrainerModel { Id = -1, Name = "FirstName LastName", NumberOfBadges = 5 };
            mockLogic
                .Setup(l => l.UpdatePokemonTrainerAsync(mockInput))
                .Returns(Task.FromResult<PokemonTrainerModel>(null));

            // Act

            // In the model, Name is required
            var actionResult = await controller.UpdatePokemonTrainer(mockInput);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdatePokemonTrainer_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.UpdatePokemonTrainerAsync(It.IsAny<PokemonTrainerModel>()))
                .Throws(new Exception("Error"));
            var pokemonTrainerModel = new PokemonTrainerModel { Id = 1, Name = "Firstname Lastname", NumberOfBadges = 5 };

            // Act
            var actionResult = await controller.UpdatePokemonTrainer(pokemonTrainerModel) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task DeletePokemonTrainer_ReturnsBadRequest_IfInputIdIs0()
        {

            // Arrange

            // Act

            // Id can't be 0
            var actionResult = await controller.DeletePokemonTrainer(0);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task DeletePokemonTrainer_ReturnsOk_IfIdExists()
        {

            // Arrange
            var mockId = 1;

            // Act

            // In the model, Name is required
            var actionResult = await controller.DeletePokemonTrainer(mockId) as OkResult;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeletePokemonTrainer_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.DeletePokemonTrainerAsync(It.IsAny<int>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.DeletePokemonTrainer(1) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteManyPokemonTrainers_ReturnsBadRequest_IfInputIdsAreEmpty()
        {

            // Arrange
            var ids = new int[0];

            // Act

            // Ids can't be emtpy
            var actionResult = await controller.DeleteManyPokemonTrainers(ids);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task DeleteManyPokemonTrainers_ReturnsOk_IfIdsExists()
        {

            // Arrange
            var mockIds = new[] { 1, 2, 3 };

            // Act

            // In the model, Name is required
            var actionResult = await controller.DeleteManyPokemonTrainers(mockIds) as OkResult;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeleteManyPokemonTrainers_ReturnsInternalServiceError_IfErrorIsThrown()
        {

            // Arrange
            mockLogic
                .Setup(l => l.DeleteManyPokemonTrainersAsync(It.IsAny<int[]>()))
                .Throws(new Exception("Error"));

            // Act
            var actionResult = await controller.DeleteManyPokemonTrainers(new[] { 1, 2, 3 }) as ObjectResult;

            // Assert
            Assert.AreEqual(500, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAllPokemonTrainersAndPokemon_ReturnsOkAndAllTrainers_IfDatabaseHasRecords()
        {
            // Arrange
            var pokemonTrainerModels = GetMockPokemonTrainerModelsAndPokemonModels();

            //the logic will return 4 models, a mocked entirety for the database
            mockLogic
                .Setup(e => e.GetPokemonTrainersAndPokemonAsync())
                .ReturnsAsync(pokemonTrainerModels);

            // Act
            var actionResult = await controller.GetPokemonTrainersAndPokemon() as OkObjectResult;

            // Assert

            //results should be not null and the count should match up
            Assert.IsNotNull(actionResult);

            var value = actionResult.Value as List<PokemonTrainerAndPokemonModel>;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Count, pokemonTrainerModels.Count);
        }

        [TestMethod]
        public async Task GetAllPokemonTrainersAndPokemon_ReturnsNoContent_IfDatabaseHasNoRecords()
        {
            // Arrange
            //don't mock any examples, we should make sure none are returned

            // Act
            var actionResult = await controller.GetAllPokemonTrainers();

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        //Private method used in tests
        private static ICollection<PokemonTrainerModel> GetMockPokemonTrainerModels()
        {
            var pokemonTrainerModels = new List<PokemonTrainerModel>
            {
                new PokemonTrainerModel { Id = 1, Name = "Example Name 1", NumberOfBadges = 1 },
                new PokemonTrainerModel { Id = 2, Name = "Example Name 2", NumberOfBadges = 2 },
                new PokemonTrainerModel { Id = 3, Name = "Example Name 3", NumberOfBadges = 3 },
                new PokemonTrainerModel { Id = 4, Name = "Example Name 4", NumberOfBadges = 4 }
            };

            return pokemonTrainerModels;
        }

        private static ICollection<PokemonTrainerAndPokemonModel> GetMockPokemonTrainerModelsAndPokemonModels()
        {
            var pokemonTrainerModels = new List<PokemonTrainerAndPokemonModel>
            {
                new PokemonTrainerAndPokemonModel
                {
                    TrainerName = "FirstName LastName1",
                    NumberOfBadges = 1,
                    Pokemon = new List<PokemonAndMoveModel>
                    {
                        new PokemonAndMoveModel
                        {
                            NickName = "PokemonTrainer1 Nickname 1",
                            Level = 50,
                            Species = "Species 1",
                            PrimaryType = "PrimaryType1",
                            SecondaryType = "SecondaryType1",
                            Moves = new List<MoveModel>
                            {
                                new MoveModel { Move = "Move1", MoveType = "Physical" },
                                new MoveModel { Move = "Move2", MoveType = "Status" },
                                new MoveModel { Move = "Move3", MoveType = "Status" },
                                new MoveModel { Move = "Move4", MoveType = "Physical" }
                            }
                        },
                        new PokemonAndMoveModel
                        {
                            NickName = "PokemonTrainer1 Nickname 2",
                            Level = 40,
                            Species = "Species2",
                            PrimaryType = "PrimaryType2",
                            SecondaryType = "SecondaryType2",
                            Moves = new List<MoveModel>
                            {
                                new MoveModel { Move = "Move1", MoveType = "Physical" },
                                new MoveModel { Move = "Move2", MoveType = "Status" },
                                new MoveModel { Move = "Move3", MoveType = "Status" },
                                new MoveModel { Move = "Move4", MoveType = "Physical" }
                            }
                        }
                    }
                },
                new PokemonTrainerAndPokemonModel
                {
                    TrainerName = "FirstName LastName2",
                    NumberOfBadges = 2,
                    Pokemon = new List<PokemonAndMoveModel>
                    {
                        new PokemonAndMoveModel
                        {
                            Level = 50,
                            Species = "Species 1",
                            PrimaryType = "PrimaryType1",
                            SecondaryType = "SecondaryType1",
                            Moves = new List<MoveModel>
                            {
                                new MoveModel { Move = "Move1", MoveType = "Physical" },
                                new MoveModel { Move = "Move2", MoveType = "Status" },
                                new MoveModel { Move = "Move3", MoveType = "Status" },
                                new MoveModel { Move = "Move4", MoveType = "Physical" }
                            }
                        }
                    }
                },
                new PokemonTrainerAndPokemonModel
                {
                    TrainerName = "FirstName LastName3",
                    NumberOfBadges = 3
                }
            };

            return pokemonTrainerModels;
        }
    }
}