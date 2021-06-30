using CoreDAL.Models;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsBluffCore.Mappers.Sieve
{
    public class CustomSieveProcessor: SieveProcessor
    {
        //public CustomSieveProcessor(IOptions<SieveOptions> options,
        //    ISieveCustomSortMethods customSortMethods,
        //    ISieveCustomFilterMethods customFilterMethods): base(options, customSortMethods, customFilterMethods)
        //{
        //}
        public CustomSieveProcessor(IOptions<SieveOptions> options) : base(options)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            mapper.Property<Dogs>(p => p.DogName)
                .CanFilter()
                .CanSort();
            //.HasName("a_different_query_name_here");

            mapper.Property<Dogs>(p => p.Gender)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.Birthdate)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.DateRegistered)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.Breed)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.BullyId)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.SaveBully)
                .CanSort()
                .CanFilter();
            mapper.Property<Dogs>(p => p.AbkcNo)
                .CanSort()
                .CanFilter();
            return mapper;
        }
    }
}
