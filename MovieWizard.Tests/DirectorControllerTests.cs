using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieWizard_API_ADO.NET_.Controllers;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardController.Tests
{
    public class DirectorControllerTests
    {
        private readonly DirectorController _directorController;
        private Mock<IDirectorService> _directorServiceMock;

        public DirectorControllerTests()
        {
            _directorServiceMock = new Mock<IDirectorService>();
            _directorController = new DirectorController(_directorServiceMock.Object);
        }

        [Fact]
        public async Task GetAllDirectors_RetunsListOfDirectors()
        {
            var allDirectorsList = new List<Director>() {
                new Director { DirectorID = 1 , Name = "Christopher Nolan"},
                new Director { DirectorID = 2 , Name = "Stephen Spielberg" }
            };


            _directorServiceMock.Setup(d=> d.GetAllDirectors()).ReturnsAsync(allDirectorsList);

            var listOfDirectors = await _directorController.GetAllDirectors();

            Assert.NotNull(listOfDirectors);

            var returnValue = Assert.IsType<List<Director>?>(listOfDirectors.Value);
            Assert.Equal(allDirectorsList.Count, returnValue.Count);
            Assert.Equal(allDirectorsList, returnValue);

        }

       
    }
}
