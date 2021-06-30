using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.EntityFrameworkCore;

using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using System.Linq;
using ABKCCommon.Models.DTOs;

namespace CoreDAL.SeedData
{
    public class DogRegistrationSeed
    {
        private const int REGISTRATIONPENDINGCONST = 25;
        public async Task AddPendingRegistrations(ModelBuilder mb)
        {
            Dogs fixedSireOldSource = new Dogs
            {
                AbkcNo = "123,456",
                DogName = "Original TEST Dog Sire",
                Id = 900000,
            };
            mb.Entity<Dogs>().HasData(fixedSireOldSource);

            BaseDogModel fixedSire = new BaseDogModel
            {
                DogName = "Original TEST Dog Sire",
                Id = 900000,
                OriginalDogTableId = fixedSireOldSource.Id
            };
            // mb.Entity<BaseDogModel>().HasData(fixedSire);
            Dogs fixedDamOldSource = new Dogs
            {
                AbkcNo = "111,111",
                DogName = "Original TEST Dog Dam",
                Id = 900001
            };
            BaseDogModel fixedDam = new BaseDogModel
            {
                DogName = "Original TEST Dog Dam",
                Id = 900001,
                OriginalDogTableId = fixedDamOldSource.Id
            };
            mb.Entity<Dogs>().HasData(fixedDamOldSource);
            // mb.Entity<BaseDogModel>().HasData(fixedDam);

            Breeds breed = new Breeds
            {
                Id = 5000,
                Breed = "SAMPLE BREED"
            };
            // mb.Entity<Breeds>().HasData(breed);

            Colors color = new Colors
            {
                Id = 5000,
                Color = "SAMPLE COLOR"
            };
            mb.Entity<Colors>().HasData(color);


            byte[] fixedOwnerSig = await HelperClasses.GetBinaryResource("signature1.jpg");
            byte[] fixedCoOwnerSig = await HelperClasses.GetBinaryResource("signature2.png");
            byte[] frontPhoto = await HelperClasses.GetBinaryResource("frontPhoto.jpg");
            byte[] sidePhoto = await HelperClasses.GetBinaryResource("sidePhoto.jpg");
            byte[] pedigreeFront = await HelperClasses.GetBinaryResource("PedigreeFront.jpg");
            byte[] pedigreeBack = await HelperClasses.GetBinaryResource("PedigreeBack.jpg");

            int startId = 500000;
            UserModel submitted = new UserModel
            {
                Id = startId,
                LoginName = $"Representative {startId}",
                OktaId = "DUMMY",
                // Roles = new List<RoleType>() { new RoleType { Type = SystemRoleEnum.Representative } }
            };
            UserModel ownerUser = new UserModel
            {
                Id = startId * 2,
                LoginName = $"Owner {startId}",
                OktaId = "DUMMY"
            };

            mb.Entity<UserModel>().HasData(submitted, ownerUser);
            mb.Entity<RoleType>().HasData(new
            {
                Id = startId,
                Type = SystemRoleEnum.Representative,
                UserModelId = submitted.Id
            }, new
            {
                Id = startId + 1,
                Type = SystemRoleEnum.Owner,
                UserModelId = ownerUser.Id
            });

            Owners owner = new Owners
            {
                Id = startId,
                OwnerId = startId,
                FirstName = $"TEST OWNER {startId}",
                Email = "owner@abkconline.com"
            };
            Owners coOwner = new Owners
            {
                Id = startId * 2,
                OwnerId = startId * 2,
                FirstName = $"TEST CO-OWNER {startId}",
                Email = "coowner@abkconline.com"
            };
            for (int i = 0; i < REGISTRATIONPENDINGCONST; i++)
            {
                startId = startId + i;
                mb.Entity<BaseDogModel>(e =>
                {
                    e.HasData(new
                    {
                        Id = startId,
                        DogName = $"TEST DOG {startId}",
                        Gender = BaseDogModel.GenderEnum.Male,
                        MicrochipNumber = "654615",
                        OriginalDogTableId = -1,
                        OwnerId = owner.Id,
                        CoOwnerId = coOwner.Id,
                        BreedId = breed.Id,
                        ColorId = color.Id,
                        DamId = fixedDam.Id,
                        SireId = fixedSire.Id,
                        DateOfBirth = DateTime.Now.Subtract(new TimeSpan(2, 5, 0, 0)),
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    });
                });

                mb.Entity<RegistrationModel>(e =>
                {
                    e.HasData(new
                    {
                        Id = startId,
                        SubmittedById = startId % 3 == 0 ? ownerUser.Id : submitted.Id,
                        DogId = startId,
                        CoOwnerSignature = fixedCoOwnerSig,
                        OwnerSignature = fixedOwnerSig,
                        BackPedigree = pedigreeBack,
                        FrontPedigree = pedigreeFront,
                        FrontPhoto = frontPhoto,
                        SidePhoto = sidePhoto,
                        RushRequested = startId % 3 == 0,
                        OvernightRequested = startId % 8 == 0,
                        IsInternationalRegistration = false,
                        SubmissionNotes = "SEEDED DUMMY REGISTRATION",
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    });
                    // e.OwnsOne(r => r.DogInfo).HasData(regDog);
                });
            }
        }

        private async Task<IEnumerable<UserModel>> GetUsers(IABKCUserService userService, SystemRoleEnum role)
        {
            var repUsers = await userService.GetByRole(role);
            repUsers = repUsers.Where(u => u.OktaId == "DUMMY").ToList();
            int repCount = repUsers.Count();
            if (repCount < 4)
            {
                for (int i = 0; i < 4 - repCount; i++)
                {
                    UserModel newUser = await userService.AddUser("DUMMY", $"dummy{role.ToString()}User{i + 1}@abkconline.com");
                    await userService.AddUserToRole(newUser, role);
                    repUsers.Add(newUser);
                }
            }
            return repUsers;
        }
        public async Task<int> SeedRegistrations(IGeneralRegistrationService regService, IOwnerService ownerService, IABKCUserService userService, IDogRegistrationService dogRegService)
        {
            //seed users
            var repUsers = await GetUsers(userService, SystemRoleEnum.Representative);
            var ownerUsers = await GetUsers(userService, SystemRoleEnum.Owner);

            byte[] fixedOwnerSig = await HelperClasses.GetBinaryResource("signature1.jpg");
            byte[] fixedCoOwnerSig = await HelperClasses.GetBinaryResource("signature2.png");
            byte[] frontPhoto = await HelperClasses.GetBinaryResource("frontPhoto.jpg");
            byte[] sidePhoto = await HelperClasses.GetBinaryResource("sidePhoto.jpg");
            byte[] pedigreeFront = await HelperClasses.GetBinaryResource("PedigreeFront.jpg");
            byte[] pedigreeBack = await HelperClasses.GetBinaryResource("PedigreeBack.jpg");
            int count = 0;
            Random r = new Random(2);
            for (int i = 0; i < REGISTRATIONPENDINGCONST; i++)
            {
                string regName = $"Dog Number {i + 1}";
                //find registration by name
                bool exists = (await regService.SearchRegistrationsByDogName(regName)).Any();
                if (!exists)
                {
                    PedigreeRegistrationDraftDTO newReg = new PedigreeRegistrationDraftDTO()
                    {
                        DogInfo = new BaseDogDTO()
                        {
                            DogName = regName,
                            DateOfBirth = DateTime.Parse("2019-02-01"),
                            Gender = "Male",
                            BreedId = 1,
                            ColorId = r.Next(250),
                            MicrochipNumber = "RANDOM MICROCHIP",
                            DamId = r.Next(284000),
                            SireId = r.Next(284000)
                        },
                        Owner = new OwnerDTO
                        {
                            Id = r.Next(60000),
                        },
                        CoOwner = new OwnerDTO
                        {
                            Id = r.Next(60000),
                        },
                    };
                    RegistrationTypeEnum regType = RegistrationTypeEnum.Pedigree;
                    RegistrationModel saved = null;
                    try
                    {
                        var r2 = new Random();
                        UserModel submitted = null;
                        submitted = i % 5 == 0 ? repUsers.ElementAt(r2.Next(0, repUsers.Count())) : ownerUsers.ElementAt(r2.Next(0, ownerUsers.Count()));
                        saved = await dogRegService.SaveDraftPedigreeRegistration(newReg, submitted);
                        await regService.AddSupportingDocument(saved.Id, "frontPhoto.jpg", frontPhoto, SupportingDocumentTypeEnum.FrontPhoto, regType, submitted);
                        await regService.AddSupportingDocument(saved.Id, "sidePhoto.jpg", sidePhoto, SupportingDocumentTypeEnum.SidePhoto, regType, submitted);
                        await regService.AddSupportingDocument(saved.Id, "pedigreefront.jpg", pedigreeBack, SupportingDocumentTypeEnum.FrontPedigree, regType, submitted);
                        await regService.AddSupportingDocument(saved.Id, "pedigreeback.jpg", pedigreeFront, SupportingDocumentTypeEnum.BackPedigree, regType, submitted);
                        await regService.AddSupportingDocument(saved.Id, "signature1.jpg", fixedOwnerSig, SupportingDocumentTypeEnum.OwnerSignature, regType, submitted);
                        await regService.AddSupportingDocument(saved.Id, "signature2.png", fixedCoOwnerSig, SupportingDocumentTypeEnum.CoOwnerSignature, regType, submitted);
                        saved.IsInternationalRegistration = false;
                        saved.OvernightRequested = i % 8 == 0;
                        saved.RushRequested = i % 3 == 0;
                        await regService.SubmitRegistration(saved.Id, regType, submitted, null);
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                    count += 1;
                }
            }

            return REGISTRATIONPENDINGCONST;
        }
    }

}