using CoreDAL;
using CoreDAL.Interfaces;
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
    public class JudgeServiceTests
    {
        public JudgeServiceTests()
        {

        }

        private ABKCOnlineContext GetABKCContext([CallerMemberName]string contextName = "memory")
        {
            DbContextOptions<ABKCOnlineContext> options;
            var builder = new DbContextOptionsBuilder<ABKCOnlineContext>();
            builder.UseInMemoryDatabase(contextName);
            options = builder.Options;
            return new ABKCOnlineContext(options);
        }

        [Fact(DisplayName = "A name string with no spaces will search by last name")]
        public async Task JudgeSearchByLastName()
        {
            //SETUP
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = new JudgeService(context);
                context.Judges.Add(new Judges()
                {
                    FirstName = "Test",
                    LastName = "Judgey"
                });
                context.Judges.Add(new Judges()
                {
                    FirstName = "Test",
                    LastName = "Judge"
                });

                await context.SaveChangesAsync();
                Judges found = await judgeService.FindByName("judge");
                Assert.NotNull(found);
                Assert.Same(context.Judges.Last(), found);
            }
        }
        [Fact(DisplayName = "A full name string will search by exact first name and starts with last name")]
        public async Task JudgeSearchByFullName()
        {
            //SETUP
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = new JudgeService(context);
                context.Judges.Add(new Judges()
                {
                    FirstName = "Test",
                    LastName = "Judge"
                });
                context.Judges.Add(new Judges()
                {
                    FirstName = "Tes",
                    LastName = "Judge"
                });
                context.Judges.Add(new Judges()
                {
                    FirstName = "Testey",
                    LastName = "Judge"
                });
                context.Judges.Add(new Judges()
                {
                    FirstName = "Test",
                    LastName = "Judgey"
                });
                await context.SaveChangesAsync();
                Judges found = await judgeService.FindByName("test judge");
                Assert.NotNull(found);
                Assert.Same(context.Judges.First(), found);
            }
        }

    }
}

