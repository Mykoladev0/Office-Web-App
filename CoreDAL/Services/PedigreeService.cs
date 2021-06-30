using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2;
using static CoreDAL.Models.v2.BaseDogModel;
using ABKCCommon.Models.DTOs.Pedigree;

namespace CoreDAL.Services
{
    public class PedigreeService : IPedigreeService
    {
        private const int PEDIGREEDEPTH = 4;

        private readonly IDogService _dogService;
        private readonly IOwnerService _ownerService;
        private readonly ILitterService _litterService;
        private readonly IMapper _autoMapper;
        private int _pedigreeDepth;

        public PedigreeService(IDogService dogService, IOwnerService ownerService, ILitterService litterService, IMapper autoMapper)
        {
            _dogService = dogService;
            _ownerService = ownerService;
            _litterService = litterService;
            _autoMapper = autoMapper;
            _pedigreeDepth = PEDIGREEDEPTH;
        }

        public async Task<PedigreeDTO> GeneratePedigreeDataFromABKCNo(string abkcNumber, bool useOldSystem = false, int pedigreeDepth = -1)
        {
            PedigreeDTO pedigree = null;
            _pedigreeDepth = pedigreeDepth > -1 ? pedigreeDepth : PEDIGREEDEPTH;
            if (useOldSystem)
            {
                Models.Dogs found = await _dogService.GetByABKCNo(abkcNumber);
                pedigree = await generatePedigreeFromDogInOldSystem(found);
            }
            else
            {
                throw new NotImplementedException("New Dog ABKC Number retrieval not implemented");
            }

            if (pedigree == null)
            {
                throw new InvalidOperationException($"Pedigree for abkc number {abkcNumber} could not be generated for an unknown reason");
            }

            pedigree.NumberOfPups = await _litterService.NumberOfPups(pedigree.DogId, pedigree.Gender == GenderEnum.Male.ToString());
            pedigree.PedigreeGeneratedDate = DateTime.UtcNow;
            return pedigree;
        }

        private async Task<PedigreeDTO> generatePedigreeFromDogInOldSystem(Models.Dogs dog)
        {
            if (dog == null)
            {
                return null;
            }
            PedigreeDTO pedigree = _autoMapper.Map<PedigreeDTO>(dog);
            Models.Owners dogOwner = await _ownerService.GetById(dog.OwnerId);
            if (dogOwner != null)
            {
                pedigree.Address1 = dogOwner.Address1;
                pedigree.Address2 = dogOwner.Address2;
                pedigree.Address3 = dogOwner.Address3;
            }
            if (dog.SireNo.HasValue)
            {
                Models.Dogs sire = await _dogService.GetById(dog.SireNo.Value.ToString());
                if (sire != null)
                {
                    //get owner
                    Models.Owners owner = await _ownerService.GetById(sire.OwnerId);
                    pedigree.SireOwnerName = owner != null ? owner.FullName : "";
                    pedigree.Sire = await getAncestryFromPreviousDogModel(sire, 1);
                }
            }
            if (dog.DamNo.HasValue)
            {
                Models.Dogs dam = await _dogService.GetById(dog.DamNo.Value.ToString());
                if (dam != null)
                {
                    Models.Owners owner = await _ownerService.GetById(dam.OwnerId);
                    pedigree.DamOwnerName = owner != null ? owner.FullName : "";
                    pedigree.Dam = await getAncestryFromPreviousDogModel(dam, 1);
                }
            }
            return pedigree;
        }
        public async Task<PedigreeDTO> GeneratePedigreeData(int dogId, bool useOldSystem = false, int pedigreeDepth = -1)
        {
            PedigreeDTO pedigree = null;
            _pedigreeDepth = pedigreeDepth > -1 ? pedigreeDepth : PEDIGREEDEPTH;
            if (useOldSystem)
            {
                Models.Dogs found = await _dogService.GetById(dogId);
                pedigree = await generatePedigreeFromDogInOldSystem(found);
            }
            else
            {
                IQueryable<BaseDogModel> q = _dogService.GetDogQuery(dogId);
                if (!q.Any())
                {
                    throw new InvalidOperationException($"Cannot generate a pedigree for {dogId}, that record not found in the system");
                }
                pedigree = _autoMapper.Map<PedigreeDTO>(q.First());

                pedigree.Dam = await getAncestryFromABKCDogModel(q.FirstOrDefault()?.Dam, 1);
                pedigree.Sire = await getAncestryFromABKCDogModel(q.FirstOrDefault()?.Sire, 1);

            }
            if (pedigree == null)
            {
                throw new InvalidOperationException($"Pedigree for dog id {dogId} could not be generated for an unknown reason");
            }
            pedigree.NumberOfPups = await _litterService.NumberOfPups(pedigree.DogId, pedigree.Gender == GenderEnum.Male.ToString());
            pedigree.PedigreeGeneratedDate = DateTime.UtcNow;
            return pedigree;
        }

        private async Task<PedigreeAncestorDTO> getAncestryFromPreviousDogModel(Models.Dogs child, int curDepth)
        {
            if (child == null)
            {
                return null;
            }
            PedigreeAncestorDTO rtn = _autoMapper.Map<PedigreeAncestorDTO>(child);
            bool isMale = false;
            if (Enum.TryParse<GenderEnum>(child.Gender, true, out GenderEnum genderParse))
            {
                isMale = genderParse == GenderEnum.Male;
            }
            rtn.NumberOfPups = await _litterService.NumberOfPups(child.Id, isMale);
            if (curDepth <= _pedigreeDepth)
            {
                if (child.SireNo.HasValue)
                {
                    //find dog
                    Models.Dogs sire = await _dogService.GetById(child.SireNo.Value.ToString());
                    if (sire != null)
                    {
                        rtn.Sire = await getAncestryFromPreviousDogModel(sire, curDepth + 1);
                    }

                }
                if (child.DamNo.HasValue)
                {
                    //find dog
                    Models.Dogs dam = await _dogService.GetById(child.DamNo.Value.ToString());
                    if (dam != null)
                    {
                        rtn.Dam = await getAncestryFromPreviousDogModel(dam, curDepth + 1);
                    }
                }
            }
            return rtn;
        }

        private async Task<PedigreeAncestorDTO> getAncestryFromABKCDogModel(BaseDogModel child, int curDepth)
        {
            if (child == null)
            {
                return null;
            }
            PedigreeAncestorDTO rtn = _autoMapper.Map<PedigreeAncestorDTO>(child);
            rtn.NumberOfPups = await _litterService.NumberOfPups(child.Id, child.Gender == GenderEnum.Male);
            if (curDepth <= _pedigreeDepth)
                if (child.Sire != null)
                {
                    rtn.Sire = await getAncestryFromABKCDogModel(child.Sire, curDepth + 1);
                }
            if (child.Dam != null)
            {
                rtn.Dam = await getAncestryFromABKCDogModel(child.Dam, curDepth + 1);
            }
            return rtn;
        }
    }
}