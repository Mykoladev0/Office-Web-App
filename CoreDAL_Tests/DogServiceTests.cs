using AutoMapper;
using CoreDAL;
using CoreDAL.Interfaces;
using CoreDAL.Mappings;
using CoreDAL.Models;
using CoreDAL.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
using Xunit;

namespace CoreDAL_Tests
{
    public class DogServiceTests : BaseTestClass, IDisposable
    {
        public DogServiceTests() : base()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<RegistrationMapping>();
                    cfg.AddProfile<BaseDogMapping>();
                    cfg.AddProfile<OwnerMapping>();
                });
        }

        [Fact(DisplayName = "A dog with basic properties set is added to datastore")]
        public async Task BasicDogIsAddedToDatastore()
        {
            //SETUP
            using (var context = GetABKCContext("BasicDogIsAddedToDatastore"))
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST"
                };
                await dogService.AddDog(dog);

                //verify one dog exists
                //var count = await context.Dogs.CountAsync();
                Assert.Single(context.Dogs);
            }
        }

        [Fact(DisplayName = "Dog should be updated if it was previously added")]
        public async Task DogIsUpdatedIfExistsInDataStore()
        {
            Dogs originalDog = null;
            var moq = new Moq.Mock<IOwnerService>();
            using (var context = GetABKCContext("DogIsUpdatedIfExistsInDataStore"))
            {
                context.Database.EnsureCreated();

                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST"
                };
                await dogService.AddDog(new CoreDAL.Models.Dogs
                {
                    DogName = "TEST2",
                    ModifiedBy = "TEST"
                });//hack for id starting at 1
                await dogService.AddDog(dog);

                originalDog = await dogService.GetById(dog.Id);

                await dogService.UpdateDog(originalDog);
                Assert.Equal(originalDog.DogName, context.Dogs.Last().DogName);
            }
        }
        [Fact(DisplayName = "An exception should be raised if a dog is updated with an id not in the system")]
        public async Task ExceptionRaisedIfDogUpdateOnIdNotInDatastore()
        {
            Dogs originalDog = null;
            var moq = new Moq.Mock<IOwnerService>();
            using (var context = GetABKCContext(""))
            {
                context.Database.EnsureCreated();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST"
                };
                await dogService.AddDog(dog);
                int id = await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST2",
                    ModifiedBy = "TEST"
                });//get second dog for id purposes HACK!
                originalDog = await dogService.GetById(id);
            }
            using (var context = GetABKCContext(""))
            {
                //give it a bad id
                originalDog.Id = -5;
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);


                await Assert.ThrowsAnyAsync<Exception>(() => dogService.UpdateDog(originalDog));
            }
        }
        [Fact(DisplayName = "An exception should be raised if a dog is created without ModifiedBy set")]
        public async Task ExceptionRaisedIfDogInsertDoesNotHaveModifiedBySet()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);
                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                };
                await Assert.ThrowsAsync<InvalidOperationException>(() => dogService.AddDog(dog));
            }
        }
        [Fact(DisplayName = "Question marks are being replaced with Unknown for Gender on Add")]
        public async Task QuestionMarksForGenderWillBeConvertedToUnknownForAdd()
        {
            string originalGender = "(???)";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST",
                    Gender = originalGender
                };
                await dogService.AddDog(dog);
                Assert.NotEqual(originalGender, dog.Gender);
                Assert.Equal("Unknown", dog.Gender);
            }

        }

        [Fact(DisplayName = "Question marks are being replaced with Unknown for Gender on Update")]
        public async Task QuestionMarksForGenderWillBeConvertedToUnknownForUpdate()
        {
            string originalGender = "(???)";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST",
                    Gender = originalGender
                };
                await context.Dogs.AddAsync(dog);//direct context to bypass conversion
                await context.SaveChangesAsync();
                dog.DogName = "Name Changed";
                await dogService.UpdateDog(dog);
                Assert.NotEqual(originalGender, dog.Gender);
                Assert.Equal("Unknown", dog.Gender);
            }

        }

        [Fact(DisplayName = "If the incoming ABKC # is different, run through abkc # validation logic on update")]
        public async Task ABKCNumberOnUpdateShouldCheckForValidityIfChanged()
        {
            CoreDAL.Models.Dogs dog = null;
            var moq = new Moq.Mock<IOwnerService>();
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST",
                    AbkcNo = "123,4"
                };
                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "HACK",
                    ModifiedBy = "TEST",
                    AbkcNo = "123,45"
                });
                await dogService.AddDog(dog);
                var dgToUpdate = new CoreDAL.Models.Dogs()
                {
                    Id = dog.Id,
                    AbkcNo = "1AA237A8888"
                };
                // dog.AbkcNo = "1AA237A8888";

                await dogService.UpdateDog(dgToUpdate);

                Assert.Equal("123,700", context.Dogs.Last().AbkcNo);
            }

        }

        [Fact(DisplayName = "If no ABKC Number is provided, the Dog's bully id will be used on Add")]
        public async Task ABKCNumberOnCreateWillUseBullyIDIfBlank()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST",
                };
                await dogService.AddDog(dog);
                Assert.True(!string.IsNullOrEmpty(dog.AbkcNo));
                Assert.Equal(dog.BullyId.ToString("000,000"), dog.AbkcNo);
            }
        }
        [Fact(DisplayName = "An ABKC number should adhere to the 000,000 format, will be corrected if wrong")]
        public async Task ABKCNumberCommaFormat()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST",
                    ModifiedBy = "TEST",
                    AbkcNo = "123,4"
                };
                await dogService.AddDog(dog);
                Assert.True(!string.IsNullOrEmpty(dog.AbkcNo));
                Assert.Equal("123,400", dog.AbkcNo);

                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST2",
                    ModifiedBy = "TEST2",
                    AbkcNo = "1,235"
                });
                Assert.Equal("123,500", context.Dogs.Last().AbkcNo);

                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST3",
                    ModifiedBy = "TEST2",
                    AbkcNo = "1AA236A"
                });
                Assert.Equal("123,600", context.Dogs.Last().AbkcNo);

                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST4",
                    ModifiedBy = "TEST2",
                    AbkcNo = "1AA237A8888"//too long, subset
                });
                Assert.Equal("123,700", context.Dogs.Last().AbkcNo);

                //already exists, use bullyId
                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "TEST5",
                    ModifiedBy = "TEST2",
                    AbkcNo = "123,400"//already exists
                });
                Assert.Equal(context.Dogs.Last().BullyId.ToString("000,000"), context.Dogs.Last().AbkcNo);
            }
        }

        [Fact(DisplayName = "A Bully ID must exist for a Dog and by 1 > last in database")]
        public async Task BullyIdForNewDogMustByOneGreaterThanLargestInDatabase()
        {
            var moq = new Moq.Mock<IOwnerService>();
            int dogId = -1;
            CoreDAL.Models.Dogs firstDog = new CoreDAL.Models.Dogs()
            {
                DogName = "Dog1",
                ModifiedBy = "TEST",
                BullyId = 100
            };
            CoreDAL.Models.Dogs secondDog = new CoreDAL.Models.Dogs()
            {
                DogName = "Dog2",
                ModifiedBy = "TEST",
                BullyId = 1
            };
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();

                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await context.Dogs.AddAsync(firstDog);
                await context.Dogs.AddAsync(secondDog);
                await context.SaveChangesAsync();
                dogId = await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "Dog3",
                    ModifiedBy = "TEST",
                });

                Dogs foundDog = await dogService.GetById(dogId);
                Assert.NotNull(foundDog);
                Assert.NotEqual(0, foundDog.BullyId);
                Assert.NotEqual(secondDog.BullyId + 1, foundDog.BullyId); //want to find the largest, not the last
                Assert.Equal(firstDog.BullyId + 1, foundDog.BullyId);
            }
        }
        [Fact(DisplayName = "Date Created and Date Last modified should be set to UTC time on Dog Add")]
        public async Task LastModifiedAndLastUpdatedDatesSetOnDogAdd()
        {
            DateTime dateInserted;
            using (var context = GetABKCContext("LastModifiedAndLastUpdatedDatesSetOnDogAdd"))
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await dogService.AddDog(new CoreDAL.Models.Dogs()
                {
                    DogName = "Dog3",
                    ModifiedBy = "TEST",
                });
                dateInserted = DateTime.UtcNow;

                Dogs foundDog = await context.Dogs.FirstAsync();
                Assert.NotNull(foundDog.DateCreated);
                Assert.NotNull(foundDog.LastModified);
                Assert.NotEqual(DateTime.MinValue, foundDog.DateCreated);
                Assert.NotEqual(DateTime.MinValue, foundDog.LastModified);
                Assert.Equal(foundDog.DateCreated, foundDog.LastModified);//happened at the same time!
                var diff = dateInserted.Subtract(foundDog.DateCreated.Value);
                Assert.True(diff.TotalMilliseconds < 500); //happened within .5sec

            }
        }
        [Fact(DisplayName = "Color List should be distinct and ordered ascending")]
        public async Task ColorListShouldBeDistinctAndAscending()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                context.Dogs.Add(new Dogs
                {
                    Color = "C"
                });
                context.Dogs.Add(new Dogs
                {
                    Color = "B"
                });
                context.Dogs.Add(new Dogs
                {
                    Color = "C"
                });
                context.Dogs.Add(new Dogs
                {
                    Color = "A"
                });
                await context.SaveChangesAsync();
                ICollection<string> colors = await dogService.GetAllColorsAsync();
                Assert.NotNull(colors);
                Assert.NotEmpty(colors);
                Assert.Equal(3, colors.Count);
                Assert.Equal("A", colors.First());
                Assert.Equal("C", colors.Last());
            }
        }

        [Fact(DisplayName = "Dog names longer than 50 characters will be truncated")]
        public async Task TruncateDogName()
        {
            var dogName = "A really long dog name with more than 50 characters";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await dogService.AddDog(new Dogs
                {
                    DogName = dogName,
                    ModifiedBy = "TEST"
                });
                Assert.NotEqual(dogName, context.Dogs.First().DogName);
                Assert.Equal(50, context.Dogs.First().DogName.Length);
            }
        }

        [Fact(DisplayName = "Dog names less than 51 characters will NOT be truncated")]
        public async Task DoNotTruncateDogName()
        {
            var dogName = "A really long dog name with less than 51";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await dogService.AddDog(new Dogs
                {
                    DogName = dogName,
                    ModifiedBy = "TEST"
                });
                Assert.Equal(dogName, context.Dogs.First().DogName);
            }
        }

        [Fact(DisplayName = "Breed names longer than 30 characters will be truncated")]
        public async Task TruncateBreedName()
        {
            var breedName = "A really long breed name with more than 30 characters";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await dogService.AddDog(new Dogs
                {
                    DogName = "TEST",
                    Breed = breedName,
                    ModifiedBy = "TEST"
                });
                Assert.NotEqual(breedName, context.Dogs.First().Breed);
                Assert.Equal(30, context.Dogs.First().Breed.Length);
            }
        }
        [Fact(DisplayName = "Breed names shorter than 31 characters will NOT be truncated")]
        public async Task DoNotTruncateBreedName()
        {
            var breedName = "A really long breed name with ";
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                await dogService.AddDog(new Dogs
                {
                    DogName = "TEST",
                    Breed = breedName,
                    ModifiedBy = "TEST"
                });
                Assert.Equal(breedName, context.Dogs.First().Breed);
            }
        }
        [Fact(DisplayName = "Dog Search should return dogs that have names that start with the search string")]
        public async Task GetDogsNameStartsWith()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                context.Dogs.Add(new Dogs
                {
                    DogName = "TEST",
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "test",
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "tExt",
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "Apple",
                });
                await context.SaveChangesAsync();

                IQueryable<Dogs> result = dogService.GetDogsQueryStartsWith("te");
                IList<Dogs> found = await result.ToListAsync();
                Assert.Equal(3, found.Count());
                int count2 = await dogService.GetDogsQueryStartsWith("TE").CountAsync();
                Assert.Equal(3, count2);
                int count3 = await dogService.GetDogsQueryStartsWith("TEs").CountAsync();
                Assert.Equal(2, count3);
                int count4 = await dogService.GetDogsQueryStartsWith("a").CountAsync();
                Assert.Equal(1, count4);
                int count5 = await dogService.GetDogsQueryStartsWith("z").CountAsync();
                Assert.Equal(0, count5);
            }
        }
        [Fact(DisplayName = "Dog Search with numeric text should check names, ids, and abkc #")]
        public async Task GetDogsNumericSearch()
        {
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                var moq = new Moq.Mock<IOwnerService>();
                IDogService dogService = new DogService(context, Mapper.Instance, moq.Object);

                context.Dogs.Add(new Dogs
                {
                    DogName = "TEST",
                    Id = 1,
                    BullyId = 1,
                    AbkcNo = "212,111"
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "1# Dog",
                    Id = 2,
                    BullyId = 2,
                    AbkcNo = "211,111"
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "Dog #1",
                    Id = 3,
                    BullyId = 13,
                    AbkcNo = "234,567"
                });
                context.Dogs.Add(new Dogs
                {
                    DogName = "Apple",
                    Id = 211,
                    BullyId = 211,
                    AbkcNo = "111,111"
                });
                await context.SaveChangesAsync();

                IQueryable<Dogs> result = dogService.GetDogsQueryStartsWith("1");
                IList<Dogs> found = await result.ToListAsync();
                Assert.Equal(4, found.Count());
                int count2 = await dogService.GetDogsQueryStartsWith("1#").CountAsync();
                Assert.Equal(1, count2);
                int count3 = await dogService.GetDogsQueryStartsWith("13").CountAsync();
                Assert.Equal(1, count3);
                int count4 = await dogService.GetDogsQueryStartsWith("211").CountAsync();
                Assert.Equal(2, count4);
                int count5 = await dogService.GetDogsQueryStartsWith("111").CountAsync();
                Assert.Equal(1, count5);
            }
        }

        public void Dispose()
        {
            // Mapper.Reset();
        }
    }
}

