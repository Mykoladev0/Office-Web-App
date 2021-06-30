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
    public class ShowServiceTests : BaseTestClass
    {
        public ShowServiceTests()
        {

        }

        [Fact(DisplayName = "UTC Now should be used for start date if no date is provided for upcoming show retrieval")]
        public async Task UpcomingShowsShouldUseUTCNowIfNoDateProvided()
        {
            //SETUP
            using (var context = GetABKCContext("UpcomingShowsShouldUseUTCNowIfNoDateProvided"))
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = null;
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "1",
                    ShowDate = DateTime.UtcNow.AddDays(1)
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "Old",
                    ShowDate = DateTime.UtcNow.AddDays(-1)
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "2",
                    ShowDate = DateTime.UtcNow.AddDays(5)
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "3",
                    ShowDate = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                IQueryable<Shows> query = showService.GetUpcomingShows();
                var list = await query.ToListAsync();
                Assert.NotEmpty(list);
                Assert.Equal(3, list.Count);
            }
        }


        [Fact(DisplayName = "Date passed should be used for start date if provided for upcoming show retrieval")]
        public async Task UpcomingShowsShouldUseDateIfProvided()
        {
            //SETUP
            DateTime initialDate = DateTime.UtcNow.AddDays(30);
            using (var context = GetABKCContext("UpcomingShowsShouldUseDateIfProvided"))
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = null;
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "1",
                    ShowDate = initialDate.AddDays(1)//should return
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "Old",
                    ShowDate = DateTime.UtcNow.AddDays(-1)//should skip
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "2",
                    ShowDate = DateTime.UtcNow.AddDays(5)//should skip
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "3",
                    ShowDate = DateTime.UtcNow//should skip
                });
                await context.SaveChangesAsync();
                IQueryable<Shows> query = showService.GetUpcomingShows(initialDate);
                var list = await query.ToListAsync();
                Assert.NotEmpty(list);
                Assert.Single(list);
            }
        }

        [Fact(DisplayName = "Shows with no date will not be returned in upcoming show query")]
        public async Task UpcomingShowsIgnoreShowsWithoutDate()
        {
            //SETUP
            using (var context = GetABKCContext("UpcomingShowsIgnoreShowsWithoutDate"))
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = null;
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "Old",
                    ShowDate = null//skip
                });
                context.Shows.Add(new Shows
                {
                    ShowName = "2",
                    ShowDate = DateTime.UtcNow.AddDays(5)//should skip
                });
                await context.SaveChangesAsync();
                IQueryable<Shows> query = showService.GetUpcomingShows();
                var list = await query.ToListAsync();
                Assert.NotEmpty(list);
                Assert.Single(list);
            }
        }

        [Fact(DisplayName = "If result does not have an id, it should be considered a new result and will be added")]
        public async Task AddResultToShow()
        {
            ClassTemplates tmpClass = new ClassTemplates()
            {
                Name = "TEST Class",
                Gender = "",
                StyleId = 1
            };
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = null;
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "1",
                    ShowDate = DateTime.Now
                });

                context.ClassTemplates.Add(tmpClass);
                await context.SaveChangesAsync();
                ShowResults result = new ShowResults()
                {
                    ArmbandNumber = 1,
                    DogId = 1,
                    Breed = "Sample Breed",
                    ClassTemplate = tmpClass,
                    ShowId = context.Shows.First().ShowId

                };
                await showService.SaveShowResult(result);
                ShowResults found = await context.ShowResults.FirstAsync();
                Assert.NotNull(found);
                Assert.Equal(1, found.Id);
                Assert.Equal(tmpClass.Name, found.Class);//for backwards compatibility
                Assert.Equal("-----------", found.Style);//for backwards compatibility, no style defined
            }
        }
        [Fact(DisplayName = "If a result without an id (new) but with an existing armband/event details should throw an exception")]
        public async Task IfArmbandExistsForEventThatDoesntHaveIdThrowException()
        {
            ClassTemplates tmpClass = new ClassTemplates()
            {
                Name = "TEST Class",
                Gender = "",
                StyleId = 1
            };
            using (var context = GetABKCContext())
            {
                context.Database.EnsureCreated();
                IJudgeService judgeService = null;
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "1",
                    ShowDate = DateTime.Now
                });

                context.ClassTemplates.Add(tmpClass);
                await context.SaveChangesAsync();
                ShowResults result = new ShowResults()
                {
                    ArmbandNumber = 1,
                    DogId = 1,
                    Breed = "Sample Breed",
                    ClassTemplate = tmpClass,
                    ShowId = context.Shows.First().ShowId

                };
                await showService.SaveShowResult(result);
                ShowResults collisionExpected = new ShowResults()
                {
                    ArmbandNumber = 1,
                    DogId = 1,
                    Breed = "Sample Breed",
                    ClassTemplate = tmpClass,
                    ShowId = context.Shows.First().ShowId

                };
                await Assert.ThrowsAsync<DataCollisionException<ShowResults>>(() => showService.SaveShowResult(collisionExpected));
            }

        }
        [Fact(DisplayName = "If a result has an id, it should update")]
        public async Task ResultWithIdShouldUpdate()
        {
            int existingId = 0;
            ClassTemplates tmpClass = new ClassTemplates()
            {
                Name = "TEST Class",
                Gender = "",
                StyleId = 1
            };
            IJudgeService judgeService = null;

            using (var context = GetABKCContext("ResultWithIdShouldUpdate"))
            {
                context.Database.EnsureCreated();
                IShowService showService = new ShowService(context, judgeService);
                context.Shows.Add(new Shows
                {
                    ShowName = "1",
                    ShowDate = DateTime.Now
                });

                context.ClassTemplates.Add(tmpClass);
                await context.SaveChangesAsync();
                ShowResults result = new ShowResults()
                {
                    ArmbandNumber = 1,
                    DogId = 1,
                    Breed = "Sample Breed",
                    ClassTemplate = tmpClass,
                    ShowId = context.Shows.First().ShowId

                };
                await showService.SaveShowResult(result);
                existingId = result.Id;
                // }
                // using (var context = GetABKCContext("ResultWithIdShouldUpdate"))
                // {
                //     IShowService showService = new ShowService(context, judgeService);

                ShowResults disconnectedResult = new ShowResults()
                {
                    Id = existingId,
                    ArmbandNumber = 1,
                    DogId = 1,
                    Breed = "Sample Breed",
                    ClassTemplate = tmpClass,
                    Points = 10,
                    ShowId = context.Shows.First().ShowId

                };
                await showService.SaveShowResult(disconnectedResult);

                Assert.Equal(disconnectedResult.Points, context.ShowResults.AsNoTracking().Where(r => r.Id == existingId).First().Points);
            }

        }
    }
}

