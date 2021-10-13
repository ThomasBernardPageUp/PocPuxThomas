using System;
using System.Collections.Generic;
using PocPuxThomas.Models.DTO.Down;
using PocPuxThomas.Models.Entities;

namespace PocPuxThomas.Converters
{
    public class CharacterDownToCharacterEntity
    {
        public static List<CharacterEntity> ConvertCharacterDownToCharacterEntity(List<CharacterDownDTO> charactersDownDTOs)
        {
            List<CharacterEntity> charactersEntities = new List<CharacterEntity>();

            foreach(CharacterDownDTO characterDownDTO in charactersDownDTOs)
            {
                CharacterEntity characterEntity = new CharacterEntity(characterDownDTO.Id, 0, characterDownDTO.Name, characterDownDTO.Image, characterDownDTO.Species, characterDownDTO.Gender ,characterDownDTO.Origin.Name);
                charactersEntities.Add(characterEntity);
            }


            return charactersEntities;
        }
    }
}
