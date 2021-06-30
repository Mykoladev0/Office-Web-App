using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs.Pedigree;
using AutoMapper;
using BullITPDF;
using CoreDAL.Interfaces;
using CoreDAL.Mappings;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using CoreDAL.Services;
using Xunit;


namespace CoreDAL_Tests
{
    public class PedigreeServiceTests : BaseTestClass, IDisposable
    {
        public PedigreeServiceTests() : base()
        {
            // Mapper.Reset();
            // Mapper.Initialize(cfg =>
            //     {
            //         // cfg.AddProfile<RegistrationMapping>();
            //         cfg.AddProfile<BaseDogMapping>();
            //         cfg.AddProfile<OwnerMapping>();
            //         cfg.AddProfile<PedigreeMapping>();
            //     });
        }
        public void Dispose()
        {
            // Mapper.Reset();
        }
        // [Fact(DisplayName = "When Id < 1, a new draft registration should be created")]
        public async Task ShouldCreateNewDraftRegistrationIfIdDoesNotExist()
        {
            //SETUP
            using (var context = GetABKCContext("ShouldCreateNewDraftRegistrationIfIdDoesNotExist"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);

                //act
                BaseDogDTO dog = new BaseDogDTO()
                {
                    DogName = "TEST",
                    DateOfBirth = DateTime.Parse("2019-02-01"),
                    Gender = "Male"
                };
                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                RegistrationModel saved = await regService.StartDraftPedigreeRegistration(dog, user);
                //assert
                Assert.Single(context.Registrations);
                Assert.Equal(1, saved.Id);
                Assert.NotNull(saved.DogInfo);
                Assert.Equal(saved.DogInfo.DogName, saved.DogInfo.DogName);
                Assert.Equal(saved.DogInfo.DateOfBirth, saved.DogInfo.DateOfBirth);
                Assert.Equal(BaseDogModel.GenderEnum.Male, saved.DogInfo.Gender);
                Assert.NotNull(saved.CurrentStatus);
                Assert.Equal(RegistrationStatusEnum.Draft, saved.CurrentStatus.Status);
            }
        }
        // [Fact(DisplayName = "If a registration had been previously saved and status moved out of draft, it will go back to draft")]
        public async Task RegistrationStatusChangesToDraftAfterSave()
        {
            //SETUP
            using (var context = GetABKCContext("RegistrationStatusChangesToDraftAfterSave"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);
                PedigreeRegistrationDraftDTO newReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "TEST",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male"
                    }
                };
                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                RegistrationModel saved = null;
                try
                {
                    saved = await regService.SaveDraftPedigreeRegistration(newReg, user);
                    Assert.Equal(1, saved.Id);//test to ensure separate context used in parallel tests
                }
                catch (System.Exception)
                {

                    throw;
                }

                //put saved into a pending status to check reset to draft
                saved.StatusHistory.Add(new DogRegistrationStatusModel()
                {
                    Status = RegistrationStatusEnum.Pending
                });
                await context.SaveChangesAsync();
                //act

                PedigreeRegistrationDraftDTO toUpdate = Mapper.Map<PedigreeRegistrationDraftDTO>(saved);
                toUpdate.DogInfo.DogName = "Changed Name";
                RegistrationModel updated = await regService.SaveDraftPedigreeRegistration(toUpdate, user);
                //assert
                Assert.Single(context.Registrations);
                Assert.Equal(1, updated.Id);
                Assert.NotNull(updated.DogInfo);
                Assert.Equal(toUpdate.DogInfo.DogName, updated.DogInfo.DogName);
                Assert.Equal(toUpdate.DogInfo.DateOfBirth, updated.DogInfo.DateOfBirth);
                Assert.Equal(BaseDogModel.GenderEnum.Male, updated.DogInfo.Gender);
                Assert.NotNull(updated.CurrentStatus);
                Assert.Equal(3, updated.StatusHistory.Count);//initial draft status, submitted status, draft status
                Assert.Equal(RegistrationStatusEnum.Draft, updated.CurrentStatus.Status);
            }
        }
        [Fact(DisplayName = "Load Background PDF from resource")]
        public async Task GetPDF()
        {
            var builder = new ABKCBuilder();
            string json = await GetTextResource("fullPedigree.json");
            PedigreeDTO pedigree = Newtonsoft.Json.JsonConvert.DeserializeObject<PedigreeDTO>(json);

            /*for static testing */
            // string filePath = @"C:\\temp\output.pdf";
            // if (System.IO.File.Exists(filePath))
            //     System.IO.File.Delete(filePath);
            // using (var fileStream = System.IO.File.OpenWrite(filePath))
            // {
            //     Stream pdfStream = await builder.GeneratePedigree(pedigree, true);
            //     pdfStream.CopyTo(fileStream);
            //     fileStream.Flush();
            //     fileStream.Close();

            //     Assert.NotNull(pdfStream);
            //     Assert.True(pdfStream.Length > 0);
            // }
            Stream pdfStream = await builder.GeneratePedigree(pedigree, true);
            Assert.NotNull(pdfStream);
            Assert.True(pdfStream.Length > 0);
        }

        public async Task<string> GetTextResource(string resourceName)
        {
            var assembly = typeof(CoreDAL_Tests.PedigreeServiceTests).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"CoreDAL_Tests.Resources.{resourceName}");
            // return resourceStream;
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}