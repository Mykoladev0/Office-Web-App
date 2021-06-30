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
    public class PedigreeRegistrationServiceTests : BaseTestClass, IDisposable
    {
        public PedigreeRegistrationServiceTests() : base()
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
        [Fact(DisplayName = "When Id < 1, a new draft registration should be created")]
        public async Task ShouldCreateNewDraftRegistrationIfIdDoesNotExist()
        {
            //SETUP
            using (var context = GetABKCContext("ShouldCreateNewDraftRegistrationIfIdDoesNotExist"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
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
                Assert.Equal(dog.DogName, saved.DogInfo.DogName);
                Assert.Equal(dog.DateOfBirth, saved.DogInfo.DateOfBirth);
                Assert.Equal(BaseDogModel.GenderEnum.Male, saved.DogInfo.Gender);
                Assert.NotNull(saved.CurrentStatus);
                Assert.Equal(RegistrationStatusEnum.Draft, saved.CurrentStatus.Status);
            }
        }

        [Fact(DisplayName = "All dog information should be added to the ABKC Dogs table")]
        public async Task AllDogInfoShouldBeAddedToTable()
        {
            //SETUP
            using (var context = GetABKCContext("AllDogInfoShouldBeAddedToTable"))
            {
                IOwnerService ownerService = new OwnerService(context);
                IDogService dogService = new DogService(context, Mapper.Instance, ownerService);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                IDogRegistrationService regService = new DogRegistrationService(context, ownerService, dogService, Mapper.Instance, notifyMoq.Object);

                Colors color = new Colors { Color = "BLACK" };
                Breeds breed = new Breeds { Breed = "TEST" };
                context.Colors.Add(color);
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();
                //act
                BaseDogDTO dog = new BaseDogDTO()
                {
                    DogName = "TEST",
                    DateOfBirth = DateTime.Parse("2019-02-01"),
                    Gender = "Male",

                };
                PedigreeRegistrationDraftDTO newReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "TEST",
                        ColorId = color.Id,
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        MicrochipNumber = "ABCD",
                        BreedId = breed.Id,
                        DamId = 1,
                        SireId = 1

                    },
                    Owner = new OwnerDTO
                    {
                        FirstName = "Owner",
                        LastName = "LastName",
                        Address1 = "First Address",
                        Address2 = "Second Address",
                        Address3 = "Third Address",
                        City = "CITY",
                        State = "STATE",
                        Country = "COUNTRY",
                        Zip = "45657",
                        International = true,
                        Email = "email@email.com",
                        Phone = "564-65464"

                    },
                    CoOwner = new OwnerDTO
                    {
                        FirstName = "CoOwner",
                        LastName = " CoOwnerLastName",
                        Address1 = "First Address",
                        Address2 = "Second Address",
                        Address3 = "Third Address",
                        City = "CITY",
                        State = "STATE",
                        Country = "COUNTRY",
                        Zip = "45657",
                        International = true,
                        Email = "coowner@email.com",
                        Phone = "564-65464"
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
                    RegistrationModel started = await regService.StartDraftPedigreeRegistration(dog, user);
                    newReg.Id = started.Id;
                    saved = await regService.SaveDraftPedigreeRegistration(newReg, user);
                }
                catch (Exception e)
                {
                    throw e;
                }
                //assert
                Assert.Single(context.Registrations);
                Assert.Equal(1, saved.Id);
                Assert.NotNull(saved.DogInfo);
                Assert.Equal(newReg.DogInfo.DogName, saved.DogInfo.DogName);
                Assert.Equal(newReg.DogInfo.DateOfBirth, saved.DogInfo.DateOfBirth);
                Assert.Equal(newReg.DogInfo.MicrochipNumber, saved.DogInfo.MicrochipNumber);
                Assert.Equal(BaseDogModel.GenderEnum.Male, saved.DogInfo.Gender);
                Assert.NotNull(saved.CurrentStatus);
                Assert.Equal(RegistrationStatusEnum.Draft, saved.CurrentStatus.Status);
                Owners foundOwner = context.Owners.FirstOrDefault(o => o.FirstName == newReg.Owner.FirstName);
                Assert.NotNull(foundOwner);
                Assert.Equal(foundOwner.Id, saved.DogInfo.Owner.Id);
                Owners foundCoOwner = context.Owners.FirstOrDefault(o => o.FirstName == newReg.CoOwner.FirstName);
                Assert.NotNull(foundCoOwner);
                Assert.Equal(foundCoOwner.Id, saved.DogInfo.CoOwner.Id);

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
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
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
                    var started = await regService.StartDraftPedigreeRegistration(new BaseDogDTO()
                    {
                        DogName = "TEST",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male"
                    }, user);
                    newReg.Id = started.Id;
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

        [Fact(DisplayName = "Only registrations that have been submitted but not approved should be returned")]
        public async Task GetAllSubmittedButNotCompleteRegistrations()
        {
            //SETUP
            using (var context = GetABKCContext("GetAllSubmittedButNotCompleteRegistrations"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };

                PedigreeRegistrationDraftDTO submittedReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Submitted",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male"
                    }
                };

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male"
                    }
                };
                RegistrationModel submitted = null;
                RegistrationModel draft = null;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                    submitted = await regService.SaveDraftPedigreeRegistration(submittedReg, user);
                    await regService.SaveDraftPedigreeRegistration(draftReg, user);//add second draft
                }
                catch (System.Exception)
                {

                    throw;
                }

                //put saved into a pending status to check reset to draft
                submitted.StatusHistory.Add(new DogRegistrationStatusModel()
                {
                    Status = RegistrationStatusEnum.Pending
                });
                await context.SaveChangesAsync();
                //act
                ICollection<RegistrationModel> foundPending = await regService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending);

                //assert
                Assert.Single(foundPending);
                Assert.Equal(submitted, foundPending.First());
                Assert.True(foundPending.First().CurrentStatus.Status == RegistrationStatusEnum.Pending);
            }
        }

        [Fact(DisplayName = "Approving a registration should add the dog record to old database")]
        public async Task ApproveRegistrationShouldAddDogToOldTable()
        {
            //SETUP
            using (var context = GetABKCContext("ApproveRegistrationShouldAddDogToOldTable"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                var jrHandlerMoq = new Moq.Mock<IJrHandlerService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);
                IGeneralRegistrationService genRegService = new GeneralRegistrationService(context, moq.Object, dogService,
                    Mapper.Instance, notifyMoq.Object, litterMoq.Object, regService, jrHandlerMoq.Object, null, null);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "Owner"
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "TEST BREED"
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        BreedId = breed.Id
                    },
                    Owner = new OwnerDTO()
                    {
                        Id = owner.Id,
                    }
                };
                RegistrationModel draft = null;
                RegistrationTypeEnum regType = RegistrationTypeEnum.Pedigree;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                    draft.DogInfo.Owner = owner;
                    draft.DogInfo.Breed = breed;
                    await context.SaveChangesAsync();
                    byte[] image = await GetBinaryResource("frontPhoto.jpg");
                    await genRegService.AddSupportingDocument(draft.Id, "front.png", image, SupportingDocumentTypeEnum.FrontPhoto, regType, user);
                    await genRegService.AddSupportingDocument(draft.Id, "back.png", new byte[6], SupportingDocumentTypeEnum.SidePhoto, regType, user);
                    await genRegService.AddSupportingDocument(draft.Id, "side.png", new byte[6], SupportingDocumentTypeEnum.FrontPedigree, regType, user);
                    await genRegService.AddSupportingDocument(draft.Id, "up.png", new byte[6], SupportingDocumentTypeEnum.OwnerSignature, regType, user);
                }
                catch (System.Exception)
                {

                    throw;
                }
                RegistrationModel submitted = await genRegService.SubmitRegistration(draft.Id, regType, user, null) as RegistrationModel;
                Assert.True(submitted.CurrentStatus.Status == RegistrationStatusEnum.Pending);

                //act
                RegistrationModel approved = await genRegService.ApproveRegistration(submitted.Id, user, "", RegistrationTypeEnum.Pedigree) as RegistrationModel;
                //assert
                Assert.NotNull(approved.DogInfo);
                Assert.True(approved.DogInfo.OriginalDogTableId > 0);
                Dogs dog = context.Dogs.Where(d => d.Id == approved.DogInfo.OriginalDogTableId).FirstOrDefault();
                Assert.NotNull(dog);
                Assert.Equal(draft.DogInfo.DogName, dog.DogName);
                Assert.Equal(draft.DogInfo.DateOfBirth, dog.Birthdate);
                Assert.NotNull(dog.DateRegistered);
                Assert.Equal(dog.DateRegistered.Value.Second, DateTime.UtcNow.Second);
                Assert.Equal(dog.DateRegistered.Value.Minute, DateTime.UtcNow.Minute);
                Assert.Equal(dog.DateRegistered.Value.Hour, DateTime.UtcNow.Hour);

            }
        }

        [Fact(DisplayName = "Registration with Sire already in system will be added")]
        public async Task AddDogWithParentDefined()
        {
            //SETUP
            using (var context = GetABKCContext("ApproveRegistrationShouldAddDogToOldTable"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "Owner"
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "TEST BREED"
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();
                Dogs sire = new Dogs
                {
                    DogName = "SIRE DOG",
                    OwnerId = owner.Id
                };
                context.Dogs.Add(sire);
                await context.SaveChangesAsync();

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        BreedId = breed.Id,
                        SireId = sire.Id
                    },
                    Owner = new OwnerDTO()
                    {
                        Id = owner.Id,
                    }
                };
                RegistrationModel draft = null;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                }
                catch (System.Exception)
                {

                    throw;
                }
                Assert.NotNull(draft);
                Assert.NotNull(draft.DogInfo);
                Assert.NotNull(draft.DogInfo.Sire);

            }
        }


        [Fact(DisplayName = "Submitting a Front Dog Image should set the thumbnail string")]
        public async Task AddThumbnailfromFrontImage()
        {
            //SETUP
            using (var context = GetABKCContext("AddThumbnailfromFrontImage"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);
                var jrHandlerMoq = new Moq.Mock<IJrHandlerService>();
                IGeneralRegistrationService genRegService = new GeneralRegistrationService(context, moq.Object, dogService,
                    Mapper.Instance, notifyMoq.Object, litterMoq.Object, regService, jrHandlerMoq.Object, null, null);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "Owner"
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "TEST BREED"
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        BreedId = breed.Id
                    },
                    Owner = new OwnerDTO()
                    {
                        Id = owner.Id,
                    }
                };
                RegistrationModel draft = null;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                    draft.DogInfo.Owner = owner;
                    draft.DogInfo.Breed = breed;
                    await context.SaveChangesAsync();
                    byte[] image = await GetBinaryResource("frontPhoto.jpg");
                    await genRegService.AddSupportingDocument(draft.Id, "frontPhoto.jpg", image, SupportingDocumentTypeEnum.FrontPhoto, RegistrationTypeEnum.Pedigree, user);
                }
                catch (System.Exception)
                {

                    throw;
                }
                Assert.NotNull(draft.RegistrationThumbnailBase64);
                Assert.True(!string.IsNullOrEmpty(draft.RegistrationThumbnailBase64));

            }
        }

        [Fact(DisplayName = "Get Documents Provided Should Be Empty if no documents are associated to a registration")]
        public async Task DocumentsProvidedShouldBeEmpty()
        {
            //SETUP
            using (var context = GetABKCContext("DocumentsProvidedShouldBeEmpty"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                var jrHandlerMoq = new Moq.Mock<IJrHandlerService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);
                IGeneralRegistrationService genRegService = new GeneralRegistrationService(context, moq.Object, dogService,
                    Mapper.Instance, notifyMoq.Object, litterMoq.Object, regService, jrHandlerMoq.Object, null, null);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "Owner"
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "TEST BREED"
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        BreedId = breed.Id
                    },
                    Owner = new OwnerDTO()
                    {
                        Id = owner.Id,
                    }
                };
                RegistrationModel draft = null;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                    draft.DogInfo.Owner = owner;
                    draft.DogInfo.Breed = breed;
                    await context.SaveChangesAsync();

                }
                catch (System.Exception)
                {

                    throw;
                }
                //Act
                ICollection<SupportingDocumentTypeEnum> supportingDocs = genRegService.GetDocumentTypesProvidedForRegistration(draft.Id, draft.RegistrationType);
                Assert.NotNull(supportingDocs);
                Assert.Empty(supportingDocs);


            }
        }

        [Fact(DisplayName = "Get Documents Provided Should Be Return All Supporting Record Types that are associated to a registration")]
        public async Task DocumentsProvidedShouldBeHaveOnlyMatching()
        {
            //SETUP
            using (var context = GetABKCContext("DocumentsProvidedShouldBeHaveOnlyMatching"))
            {
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                var notifyMoq = new Moq.Mock<IRegistrationNotificationService>();
                var litterMoq = new Moq.Mock<ILitterService>();
                var jrHandlerMoq = new Moq.Mock<IJrHandlerService>();
                IDogRegistrationService regService = new DogRegistrationService(context, moq.Object, dogService, Mapper.Instance, notifyMoq.Object);
                IGeneralRegistrationService genRegService = new GeneralRegistrationService(context, moq.Object, dogService,
                    Mapper.Instance, notifyMoq.Object, litterMoq.Object, regService, jrHandlerMoq.Object, null, null);

                UserModel user = new UserModel
                {
                    OktaId = "ABCD",
                    LoginName = "user@login"
                };
                Owners owner = new Owners
                {
                    FirstName = "Owner"
                };
                context.Owners.Add(owner);
                Breeds breed = new Breeds
                {
                    Breed = "TEST BREED"
                };
                context.Breeds.Add(breed);
                await context.SaveChangesAsync();

                PedigreeRegistrationDraftDTO draftReg = new PedigreeRegistrationDraftDTO()
                {
                    DogInfo = new BaseDogDTO()
                    {
                        DogName = "Draft",
                        DateOfBirth = DateTime.Parse("2019-02-01"),
                        Gender = "Male",
                        BreedId = breed.Id
                    },
                    Owner = new OwnerDTO()
                    {
                        Id = owner.Id,
                    }
                };
                RegistrationModel draft = null;
                try
                {
                    draft = await regService.SaveDraftPedigreeRegistration(draftReg, user);
                    draft.DogInfo.Owner = owner;
                    draft.DogInfo.Breed = breed;
                    await context.SaveChangesAsync();

                    byte[] image = await GetBinaryResource("frontPhoto.jpg");
                    await genRegService.AddSupportingDocument(draft.Id, "frontPhoto.jpg", image, SupportingDocumentTypeEnum.FrontPhoto, RegistrationTypeEnum.Pedigree, user);
                    await genRegService.AddSupportingDocument(draft.Id, "backPedigree.jpg", image, SupportingDocumentTypeEnum.BackPedigree, RegistrationTypeEnum.Pedigree, user);


                }
                catch (System.Exception)
                {

                    throw;
                }
                //Act
                ICollection<SupportingDocumentTypeEnum> supportingDocs = genRegService.GetDocumentTypesProvidedForRegistration(draft.Id, draft.RegistrationType);
                Assert.NotNull(supportingDocs);
                Assert.Equal(2, supportingDocs.Count);
                Assert.Contains(SupportingDocumentTypeEnum.FrontPhoto, supportingDocs);
                Assert.Contains(SupportingDocumentTypeEnum.BackPedigree, supportingDocs);


            }
        }

    }
}