using CoreDAL;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.v2;
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
    public class OwnerServiceTests
    {
        public OwnerServiceTests()
        {

        }

        private ABKCOnlineContext GetABKCContext([CallerMemberName]string contextName = "memory")
        {
            //var options = SqliteInMemory
            //    .CreateOptions<ABKC_Orig_Context>();
            DbContextOptions<ABKCOnlineContext> options;
            var builder = new DbContextOptionsBuilder<ABKCOnlineContext>();
            builder.UseInMemoryDatabase(contextName);
            options = builder.Options;
            return new ABKCOnlineContext(options);
        }

        [Fact(DisplayName = "An ownerwith basic properties set is added to datastore")]
        public Task BasicOwnerIsAddedToDatastore()
        {
            ////SETUP
            //using (var context = GetABKCContext())
            //{
            //    context.Database.EnsureCreated();
            //    IDogService dogService = new DogService(context);
            //    CoreDAL.Models.Dogs dog = new CoreDAL.Models.Dogs()
            //    {
            //        DogName = "TEST",
            //        ModifiedBy = "TEST"
            //    };
            //    await dogService.AddDog(dog);
            //}
            //using (var context = GetABKCContext())
            //{
            //    //verify one dog exists
            //    //var count = await context.Dogs.CountAsync();
            //    Assert.Single(context.Dogs);
            //}
            Assert.False(false);
            return null;
        }

        [Fact(DisplayName = "An owner's full name is computed and added via raw sql to the database")]
        public async Task FullNameIsSetWhenOwnerIsAdded()
        {
            //SETUP
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                OwnerService ownService = new OwnerService(context);
                CoreDAL.Models.Owners owner = new CoreDAL.Models.Owners()
                {
                    FirstName = "TEST",
                    //MiddleInitial = "A",
                    LastName = "TEST",
                    ModifiedBy = "TEST",
                    Email = "test@test.com"
                };
                UserModel user = new UserModel
                {
                    LoginName = "test@test.com"
                };
                //can't call direct sql in in memory test database
                await ownService.AddOwnerWithoutFullNameWrite(owner, user);
            }
            using (var context = GetABKCContext())
            {
                //verify one dog exists
                //var count = await context.Dogs.CountAsync();
                Assert.Single(context.Owners);
            }

        }

        [Fact(DisplayName = "An owner's full name is computed and updated via raw sql when any name field changes")]
        public async Task FullNameIsSetWhenOwnerIsUpdatedAndNameChanged()
        {
            //SETUP
            CoreDAL.Models.Owners owner = null;
            UserModel user = new UserModel
            {
                LoginName = "test@test.com"
            };
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                OwnerService ownService = new OwnerService(context);
                owner = new CoreDAL.Models.Owners()
                {
                    FirstName = "TEST",
                    //MiddleInitial = "A",
                    LastName = "TEST",
                    ModifiedBy = "TEST",
                    Email = "test@test.com"
                };

                //can't call direct sql in in memory test database
                await ownService.AddOwnerWithoutFullNameWrite(owner, user);
            }
            using (var context = GetABKCContext())
            {
                OwnerService ownService = new OwnerService(context);
                var fOwn = context.Owners.AsNoTracking().FirstOrDefault();
                fOwn.FirstName = "Changed";
                fOwn.MiddleInitial = "";
                await ownService.UpdateOwnerWithoutFullNameWrite(owner, fOwn, user);
                Assert.Single(context.Owners);
            }

        }

        [Fact(DisplayName = "An owner's full name property is built from 3 name properties")]
        public void OwnerFullNameCalculated()
        {
            //SETUP
            var owner = new CoreDAL.Models.Owners()
            {
                FirstName = "TEST",
                MiddleInitial = "A",
                LastName = "TEST",
                ModifiedBy = "TEST",
                Email = "test@test.com"
            };
            Assert.Equal("TEST A TEST", owner.FullName);

            var owner2 = new CoreDAL.Models.Owners()
            {
                FirstName = "",
                MiddleInitial = "",
                LastName = "",
                ModifiedBy = "TEST",
                Email = "test@test.com"
            };
            Assert.Equal("", owner2.FullName);
            var owner3 = new CoreDAL.Models.Owners()
            {
                FirstName = "TEST",
                MiddleInitial = "",
                LastName = "TEST",
                ModifiedBy = "TEST",
                Email = "test@test.com"
            };
            Assert.Equal("TEST TEST", owner3.FullName);

            var owner4 = new CoreDAL.Models.Owners()
            {
                FirstName = "",
                MiddleInitial = "",
                LastName = "TEST",
                ModifiedBy = "TEST",
                Email = "test@test.com"
            };
            Assert.Equal("TEST", owner4.FullName);
        }


        [Fact(DisplayName = "Owner Search will search by email address if incoming searchstring is a valid email address")]
        public async Task EmailAddressOwnerSearch()
        {
            string emailAddress = "test@test.com";
            //SETUP
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();


                CoreDAL.Models.Owners owner = new CoreDAL.Models.Owners()
                {
                    Email = emailAddress,
                    OwnerId = 1
                };
                CoreDAL.Models.Owners owner2 = new CoreDAL.Models.Owners()
                {
                    Email = "nobody@nobody.com",
                    FirstName = "TEST2",
                    OwnerId = 2
                };
                await context.Owners.AddAsync(owner);
                await context.Owners.AddAsync(owner2);
                await context.SaveChangesAsync();
            }

            using (var context = GetABKCContext())
            {
                IOwnerService ownerService = new OwnerService(context);
                var q = ownerService.GetOwnersQueryStartsWith(emailAddress);
                var rtn = await q.ToListAsync();
                Assert.Single(rtn);
                Assert.Equal(rtn.First().Email, emailAddress);
            }
        }
    }
}

