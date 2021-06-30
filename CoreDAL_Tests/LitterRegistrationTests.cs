using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using AutoMapper;
using CoreDAL.Interfaces;
using CoreDAL.Mappings;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using CoreDAL.Services;
using Xunit;


namespace CoreDAL_Tests
{
    public class LitterRegistrationTests : BaseTestClass, IDisposable
    {
        public LitterRegistrationTests() : base()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<RegistrationMapping>();
                    cfg.AddProfile<BaseDogMapping>();
                    cfg.AddProfile<OwnerMapping>();
                });
        }
        public void Dispose()
        {
            // Mapper.Reset();
        }
        [Fact(DisplayName = "If Sire and Dam are provided, create draft litter registration")]
        public async Task ShouldCreateDraftIfSireAndDamAreProvided()
        {
            //SETUP
            using (var context = GetABKCContext("ShouldCreateNewDraftRegistrationIfIdDoesNotExist"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                ILitterService litterService = new LitterService(context, dogService);
                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                int sireId = await dogService.AddDog(new Dogs
                {
                    DogName = "Sire",
                    Gender = "Male",
                    OwnerId = 1,//test to make sure this doesnt fail
                    ModifiedBy = user.LoginName
                });
                int damId = await dogService.AddDog(new Dogs
                {
                    DogName = "Dam",
                    Gender = "Female",
                    ModifiedBy = user.LoginName
                });
                //act
                LitterRegistrationModel reg = await litterService.StartLitterRegistration(sireId, damId, user);
                //assert
                Assert.NotNull(reg);
                Assert.Single(context.LitterRegistrations);
                Assert.Equal(1, reg.Id);
                //make sure the parents were added to new ABKC table
                BaseDBModel abkcSire = context.ABKCDogs.FirstOrDefault(d => d.OriginalDogTableId == sireId);
                BaseDBModel abkcDam = context.ABKCDogs.FirstOrDefault(d => d.OriginalDogTableId == damId);
                Assert.NotNull(abkcSire);
                Assert.NotNull(abkcDam);
                Assert.Equal(abkcSire, reg.Sire);
                Assert.Equal(abkcSire, reg.Dam);
                Assert.Equal(RegistrationStatusEnum.Draft, reg.CurrentStatus.Status);
                Assert.Equal(RegistrationStatusEnum.Draft, reg.CurStatus);
            }
        }


        [Fact(DisplayName = "If a registration had been previously saved and status moved out of draft, it will go back to draft")]
        public async Task RegistrationStatusChangesToDraftAfterSave()
        {
            //SETUP
            using (var context = GetABKCContext("RegistrationStatusChangesToDraftAfterSave"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                ILitterService litterService = new LitterService(context, dogService);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };

                int sireId = await dogService.AddDog(new Dogs
                {
                    DogName = "Sire",
                    Gender = "Male",
                    OwnerId = 1,//test to make sure this doesnt fail
                    ModifiedBy = user.LoginName
                });
                int damId = await dogService.AddDog(new Dogs
                {
                    DogName = "Dam",
                    Gender = "Female",
                    ModifiedBy = user.LoginName
                });

                LitterRegistrationModel reg = await litterService.StartLitterRegistration(sireId, damId, user);

                //put saved into a pending status to check reset to draft
                reg.StatusHistory.Add(new LitterRegistrationStatusModel()
                {
                    Status = RegistrationStatusEnum.Pending
                });
                await context.SaveChangesAsync();
                //act
                LitterRegistrationModel updated = await litterService.SaveLitterDraft(reg.Id, new LitterDraftDTO
                {
                    NumberOfFemalesBeingRegistered = 5,
                    NumberOfMalesBeingRegistered = 10,
                    DateOfBreeding = DateTime.Now,
                    DateOfLitterBirth = DateTime.Now,
                    FrozenSemenUsed = false,
                }, user);

                //assert
                Assert.Single(context.LitterRegistrations);
                Assert.Equal(1, updated.Id);
                Assert.Equal(5, updated.NumberOfFemalesBeingRegistered);
                Assert.Equal(10, updated.NumberOfMalesBeingRegistered);
                Assert.NotNull(updated.CurrentStatus);
                Assert.Equal(3, updated.StatusHistory.Count);//initial draft status, submitted status, draft status
                Assert.Equal(RegistrationStatusEnum.Draft, updated.CurrentStatus.Status);
            }
        }

        [Fact(DisplayName = "If a registration is approved, a litter will be created and unregistered puppies for each puppy will be added to the tables")]
        public async Task ApproveLitterRegistrationWillCreateLitterAndPuppies()
        {
            //SETUP
            using (var context = GetABKCContext("ApproveLitterRegistrationWillCreateLitterAndPuppies"))
            {
                // var moq = new Moq.Mock<IOwnerService>();
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                IOwnerService ownerService = new OwnerService(context);
                IDogService dogService = new DogService(context, Mapper.Instance, ownerService);
                ILitterService litterService = new LitterService(context, dogService);
                var jrHandlerMoq = new Moq.Mock<IJrHandlerService>();
                IDogRegistrationService dogRegService = new DogRegistrationService(context, ownerService, dogService, Mapper.Instance, notifyMoq.Object);
                IGeneralRegistrationService genRegService = new GeneralRegistrationService(context, ownerService, dogService,
                    Mapper.Instance, notifyMoq.Object, litterService, dogRegService, jrHandlerMoq.Object, null, null);
                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "TEST",
                    Id = 2,
                    OwnerId = 2,
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "BREED",
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();

                int sireId = await dogService.AddDog(new Dogs
                {
                    DogName = "Sire",
                    Gender = "Male",
                    OwnerId = owner.Id,
                    ModifiedBy = user.LoginName
                });
                int damId = await dogService.AddDog(new Dogs
                {
                    DogName = "Dam",
                    Gender = "Female",
                    OwnerId = owner.Id,
                    ModifiedBy = user.LoginName
                });


                LitterRegistrationModel reg = await litterService.StartLitterRegistration(sireId, damId, user);
                LitterRegistrationModel updated = await litterService.SaveLitterDraft(reg.Id, new LitterDraftDTO
                {
                    NumberOfFemalesBeingRegistered = 2,
                    NumberOfMalesBeingRegistered = 3,
                    DateOfBreeding = DateTime.Now,
                    DateOfLitterBirth = DateTime.Now,
                    FrozenSemenUsed = false,
                    Breed = breed
                }, user);
                byte[] image = await GetBinaryResource("frontPhoto.jpg");
                await genRegService.AddSupportingDocument(reg.Id, "frontPhoto.jpg", image, SupportingDocumentTypeEnum.SireOwnerSignature, RegistrationTypeEnum.Litter, user);
                await genRegService.AddSupportingDocument(reg.Id, "backPedigree.jpg", image, SupportingDocumentTypeEnum.DamOwnerSignature, RegistrationTypeEnum.Litter, user);
                await genRegService.SubmitRegistration(reg.Id, reg.RegistrationType, user, null);
                //act
                IRegistration approved = await genRegService.ApproveRegistration(reg.Id, user, "dasfa", reg.RegistrationType);
                //assert

                LitterRegistrationModel finalReg = await litterService.GetLitterRegistrationById(reg.Id);


                Assert.Single(context.LitterRegistrations);
                Assert.Equal(RegistrationStatusEnum.Approved, updated.CurrentStatus.Status);
                Assert.Single(context.Litters);
                Litters litter = context.Litters.First();
                Assert.Equal(finalReg.NumberOfMalesBeingRegistered, litter.Males);
                Assert.Equal(finalReg.NumberOfFemalesBeingRegistered, litter.Females);
                Assert.Equal(finalReg.Dam.Owner.Id, litter.OwnerId);//dam is the owner
                Assert.Equal(finalReg.DateOfLitterBirth, litter.Birthdate);
                Assert.Equal(finalReg.DateOfBreeding, litter.BreedingDate);
                Assert.Equal(finalReg.Breed.Breed, litter.Breed);
                Assert.Equal(finalReg.Sire.Owner.Id, litter.SireOwnerId);
                //check puppies
                // List<string> males = context.ABKCDogs.Where(d => d.Gender == BaseDogModel.GenderEnum.Male).Select(d => d.DogName).ToList();
                int maleCountABKC = context.ABKCDogs.Where(d => d.Gender == BaseDogModel.GenderEnum.Male).Count();
                int femaleCountABKC = context.ABKCDogs.Where(d => d.Gender == BaseDogModel.GenderEnum.Female).Count();
                int maleCount = context.Dogs.Where(d => d.Gender == BaseDogModel.GenderEnum.Male.ToString()).Count();
                int femaleCount = context.Dogs.Where(d => d.Gender == BaseDogModel.GenderEnum.Female.ToString()).Count();
                Assert.Equal(finalReg.NumberOfMalesBeingRegistered + 1, maleCountABKC);
                Assert.Equal(finalReg.NumberOfFemalesBeingRegistered + 1, femaleCountABKC);
                Assert.Equal(finalReg.NumberOfMalesBeingRegistered + 1, maleCount);
                Assert.Equal(finalReg.NumberOfFemalesBeingRegistered + 1, femaleCount);
            }
        }

    }
}