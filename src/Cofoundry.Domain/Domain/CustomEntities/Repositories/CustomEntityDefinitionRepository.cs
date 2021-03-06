﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class CustomEntityDefinitionRepository : ICustomEntityDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, ICustomEntityDefinition> _customEntityDefinitions;

        public CustomEntityDefinitionRepository(
            IEnumerable<ICustomEntityDefinition> customEntityDefinitions
            )
        {
            DetectInvalidDefinitions(customEntityDefinitions);
            _customEntityDefinitions = customEntityDefinitions.ToDictionary(k => k.CustomEntityDefinitionCode);
        }

        private void DetectInvalidDefinitions(IEnumerable<ICustomEntityDefinition> definitions)
        {
            var nullName = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.CustomEntityDefinitionCode))
                .FirstOrDefault();

            if (nullName != null)
            {
                var message = nullName.GetType().Name + " does not have a definition code specified.";
                throw new InvalidCustomEntityDefinitionException(message, nullName, definitions);
            }

            var dulpicateCode = definitions
                .GroupBy(e => e.CustomEntityDefinitionCode)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateCode != null)
            {
                var message = "Duplicate ICustomEntityDefinition.CustomEntityDefinitionCode: " + dulpicateCode.Key;
                throw new InvalidCustomEntityDefinitionException(message, dulpicateCode.First(), definitions);
            }

            var dulpicateName = definitions
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateName != null)
            {
                var message = "Duplicate ICustomEntityDefinition.Name: " + dulpicateName.Key;
                throw new InvalidCustomEntityDefinitionException(message, dulpicateName.First(), definitions);
            }

            var nameNot6Chars = definitions
                .Where(d => d.CustomEntityDefinitionCode.Length != 6)
                .FirstOrDefault();

            if (nameNot6Chars != null)
            {
                var message = nameNot6Chars.GetType().Name + " has a definition code that is not 6 characters in length. All custom entity definition codes must be 6 characters.";
                throw new InvalidCustomEntityDefinitionException(message, nameNot6Chars, definitions);
            }
        }

        #endregion

        public ICustomEntityDefinition GetByCode(string code)
        {
            return _customEntityDefinitions.GetOrDefault(code);
        }


        public IEnumerable<ICustomEntityDefinition> GetAll()
        {
            return _customEntityDefinitions.Select(p => p.Value);
        }
    }
}
