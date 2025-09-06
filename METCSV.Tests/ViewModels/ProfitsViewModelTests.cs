using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace METCSV.UnitTests.ViewModels
{
    [TestClass]
    public class ProfitsViewModelTests
    {
        //[TestMethod]
        //public void Values_are_changed_when_selecting_other_provider()
        //{
        //    // Arrange
        //    ProfitsViewModel profitsViewModel = Factory.GetProfitsViewModel();
        //    profitsViewModel.AddManufacturers(Factory.GetManufacturers(Providers.Lama));
        //    profitsViewModel.SelectedProfits = profitsViewModel.ProfitsCollections[0];

        //    // Act
        //    var values = profitsViewModel.Values;
        //    profitsViewModel.SelectedProfits = profitsViewModel.ProfitsCollections[1];
        //    var values2 = profitsViewModel.Values;

        //    // Assert
        //    Assert.IsFalse(values == values2);
        //}

        //[TestMethod]
        //public void MessageErrorShowWhenOneOfTheFilesDoesNotExists()
        //{
        //    // Arrange
        //    ProfitsViewModel profitsViewModel = Factory.GetProfitsViewModel();
        //    DeleteAllProfits();

        //    // Act
        //    profitsViewModel.LoadFromFiles();

        //    // Assert
        //    Assert.AreEqual(Visibility.Visible, profitsViewModel.ErrorTextVisibility);
        //}

        //[TestMethod]
        //public void MessageDoesNotShowWhenAllFilesExists()
        //{
        //    // Arrange
        //    ProfitsViewModel profitsViewModel = Factory.GetProfitsViewModel();
        //    ProfitsIO.SaveToFile(Factory.GetProfits(Providers.AB));
        //    ProfitsIO.SaveToFile(Factory.GetProfits(Providers.Lama));
        //    ProfitsIO.SaveToFile(Factory.GetProfits(Providers.TechData));

        //    // Act
        //    profitsViewModel.LoadFromFiles();

        //    // Assert
        //    Assert.AreEqual(Visibility.Hidden, profitsViewModel.ErrorTextVisibility);
        //}

        //[TestMethod]
        //public void ChangesAreNotApplyRightAfterEditingValue()
        //{
        //    // Arrange
        //    ProfitsViewModel profitsViewModel = Factory.GetProfitsViewModel();
        //    profitsViewModel.SelectedProfits = profitsViewModel.ProfitsCollections[0];
        //    var startingValue = profitsViewModel.ProfitsCollections[0].Values.Values.First();

        //    // Act
        //    var valueToEdit = profitsViewModel.Values.First();
        //    valueToEdit.Value = 100;

        //    // Assert
        //    Assert.AreNotEqual(100, startingValue);
        //}

        //[TestMethod]
        //public void ChangesAreApplyAfterSave()
        //{
        //    // Arrange
        //    ProfitsViewModel profitsViewModel = Factory.GetProfitsViewModel();
        //    profitsViewModel.SelectedProfits = profitsViewModel.ProfitsCollections[0];

        //    // Act
        //    var valueToEdit = profitsViewModel.Values.First();
        //    valueToEdit.Value = 100;
        //    profitsViewModel.SaveAllProfits();

        //    // Assert
        //    Assert.AreEqual(100, profitsViewModel.ProfitsCollections[0].Values.Values.First());
        //}


    }
}
